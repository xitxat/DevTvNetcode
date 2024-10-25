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
using System.Collections.Generic;

// br 5.04
// serverPort : game
// queryPort: analyitics
// Team ID
public class ServerGameManager : IDisposable
{


    private string serverIP;
    private int serverPort;
    private int queryPort;
    private MatchplayBackfiller backfiller;
    private MultiplayAllocationService multiplayAllocationService; // Server health ping  via cmd

    // str Matchmaker id / int out Team Index 94 colours)
    private Dictionary<string, int> teamIdToTeamIndex = new Dictionary<string, int>();
    public NetworkServer NetworkServer { get; private set; }    // Approve connections,

    public ServerGameManager(string serverIP, int serverPort, int queryPort, 
                                NetworkManager manager, NetworkObject playerPrefab)
    {
        this.serverIP = serverIP;
        this.serverPort = serverPort;
        this.queryPort = queryPort;
        NetworkServer = new NetworkServer(manager, playerPrefab);
        multiplayAllocationService = new MultiplayAllocationService();
    }


    // Enter when sucessfully conntected to UGS / BEGIN:
    // Match made. spin up server
    public async Task StartGameServerAsync()
    {
        // API calls
        try
        {
            // Server status. Players in, Server Health
            await multiplayAllocationService.BeginServerCheck();

            // return matchmaking data
            MatchmakingResults matchmakerPayload =  await GetMatchmakerPayload();

            if(matchmakerPayload != null)
            {
                //  START BACKFILLING br5.09
                // allow players to enter after start
                await StartBackfill(matchmakerPayload);
                NetworkServer.OnUserJoined += UserJoined;
                NetworkServer.OnUserLeft += UserLeft;
            }
            else
            {
                Debug.LogWarning("¦| <ServerGameManager> Matchmaker payload timed out. Proceeding without backfill. ¦|");
            }
            if (!NetworkServer.OpenConnection(serverIP, serverPort))
            {
                Debug.LogError("¦| <ServerGameManager> NetworkServer did not start as expected. Please check configuration. ¦|");
                return;
            }

            Debug.Log("¦| <ServerGameManager> Server successfully started and awaiting connections. ¦|");
        }
        catch(Exception e)
        {
            Debug.LogError($"¦| <ServerGameManager> Error starting game server: {e.Message} ¦|");
        }

    }



    private async Task<MatchmakingResults> GetMatchmakerPayload()
    {
        // Handle matchmking failure, force timeout, avoid server hang via shutdown
            // timer + await countdown Task

        // Event subs + return Payload matchmaking data
            //Store this task in var, (don't run yet)
        Task<MatchmakingResults> matchmakerPayloadTask = 
            multiplayAllocationService.SubscribeAndAwaitMatchmakerAllocation(); 

        // if True matchmakerPayloadTask enter {} , F; return null
        if (await Task.WhenAny(matchmakerPayloadTask, Task.Delay(20000)) == matchmakerPayloadTask)
        {
            return matchmakerPayloadTask.Result;
        }

        Debug.LogWarning("¦| <ServerGameManager> Timed out waiting for matchmaker allocation. ¦|");
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
                //backfiller.AddPlayerToMatch(user);

        // ADD Player to Team when Joining
        Team team = backfiller.GetTeamByUserId(user.userAuthId);

        // check if user joining team is already in Dict/ or in game
        if(!teamIdToTeamIndex.TryGetValue(team.TeamId, out int teamIndex))
        {
            // first person to join, add to dict
            teamIndex = teamIdToTeamIndex.Count;
            teamIdToTeamIndex.Add(team.TeamId, teamIndex);
        }
        // Do find team, assign player
        user.teamIndex = teamIndex;


                //Debug.Log($"<color=yellow>user{user.userAuthId} : tID{team.TeamId}</color>");

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
        if (backfiller != null)
        {
            await backfiller.StopBackfill();
        }
        Dispose();
        Application.Quit(); // Server
    }

    public void Dispose()
    {
        NetworkServer.OnUserJoined -= UserJoined;
        NetworkServer.OnUserLeft -= UserLeft;

        backfiller?.Dispose();
        multiplayAllocationService?.Dispose();
        NetworkServer?.Dispose();
    }
}