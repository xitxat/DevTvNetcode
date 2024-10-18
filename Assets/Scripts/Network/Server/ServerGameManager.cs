using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Collections;
using System.Text;
using Unity.Services.Authentication;
using Unity.Services.Matchmaker.Models;

// br 5.04
// serverPort : game
// queryPort: analyitics
public class ServerGameManager : IDisposable
{
    private string serverIP;
    private int serverPort;
    private int queryPort;

    private MatchplayBackfiller backfiller;
    private NetworkServer networkServer;    // Approve connections, 
    private MultiplayAllocationService multiplayAllocationService; // Server health ping  via cmd

    private const string GameSceneName = "Game";


    public ServerGameManager(string serverIP, int serverPort, int queryPort, NetworkManager manager)
    {
        this.serverIP = serverIP;
        this.serverPort = serverPort;
        this.queryPort = queryPort;
        networkServer = new NetworkServer(manager);
        multiplayAllocationService = new MultiplayAllocationService();
    }


    // Enter when sucessfully conntected to UGS / BEGIN:
    // Match made. spin up server
    public async Task StartGameServerAsync()
    {
        // Server status. Players in, Server Health
        await multiplayAllocationService.BeginServerCheck();

        // API calls
        try
        {
            // return matchmaking data
            MatchmakingResults matchmakerPayload =  await GetMatchmakerPayload();

            if(matchmakerPayload != null)
            {
                //  START BACKFILLING br5.09
                // allow players to enter after start
                await StartBackfill(matchmakerPayload);
            }
            else
            {
            Debug.LogWarning("Matchmaker payload timed out ;(");

            }
        }
        catch(Exception e)
        {
            Debug.LogWarning(e);
        }

        // Open Server on this IP & Port
        if(!networkServer.OpenConnection(serverIP, serverPort))
        {
            Debug.LogWarning("NetworkServer did not start. :( ");
            return;
        }

        //  Load Scene
        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);

    }



    private async Task<MatchmakingResults> GetMatchmakerPayload()
    {
        // Handle matchmking failure, force timeout, avoid server hang via shutdown
            // timer + await countdown Task

        // Event subs + return Payload matchmaking data
            //Store this task in var, (don't run yet)
        Task<MatchmakingResults> matchmakerPayloadTask = 
            multiplayAllocationService.SubscribeAndAwaitMatchmakerAllocation(); // f12

        // if True matchmakerPayloadTask enter {} , F; return null
        if (await Task.WhenAny(matchmakerPayloadTask, Task.Delay(20000)) == matchmakerPayloadTask)
        {
            return matchmakerPayloadTask.Result;
        }

        return null;
    }


    private async Task StartBackfill(MatchmakingResults payload)
    {
        // connection str (ip:port)
        backfiller = new MatchplayBackfiller($"{serverIP}:{serverPort}", 
            payload.QueueName, 
            payload.MatchProperties, 
            20);

        // when match starts
        if (backfiller.NeedsPlayers())
        {
        await backfiller.BeginBackfilling();

        }

    }


    private void UserJoined(UserData user)
    {
        backfiller.AddPlayerToMatch(user);
        // manual update analytic service (no user data)
        multiplayAllocationService.AddPlayer();

        // If have MAX players
        if(!backfiller.NeedsPlayers() && backfiller.IsBackfilling)
        {
            // _= we don't need to await data return
            _ = backfiller.StopBackfill();
        }
    }    
    
    private void UserLeft(UserData user)
    {
        // returns # players left after removal
        int playerCount = backfiller.RemovePlayerFromMatch(user.userAuthId);
        multiplayAllocationService.RemovePlayer();

        if(playerCount <= 0)
        {
            CloseServer();
            return;
        }

        // Empty slots; refill
        if(backfiller.NeedsPlayers() && !backfiller.IsBackfilling)
        {
            _ = backfiller.BeginBackfilling();
        }
    }


    private async void CloseServer()
    {
        await backfiller.StopBackfill();
        Dispose();
        Application.Quit(); // Server
    }

    public void Dispose()
    {
        backfiller?.Dispose();
        multiplayAllocationService?.Dispose();
        networkServer?.Dispose();
    }
}