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

 // br 5.04
// serverPort : game
// queryPort: analyitics
public class ServerGameManager : IDisposable
{
    private string serverIP;
    private int serverPort;
    private int queryPort;
    private NetworkServer networkServer;    // Approve connections, 
    private MultiplayAllocationService multiplayAllocationService; // Server health ping  via cmd
    public ServerGameManager(string serverIP, int serverPort, int queryPort, NetworkManager manager)
    {
        this.serverIP = serverIP;
        this.serverPort = serverPort;
        this.queryPort = queryPort;
        networkServer = new NetworkServer(manager);
        multiplayAllocationService = new MultiplayAllocationService();
    }


    // Enter when sucessfully conntected to UGS / BEGIN:
    public async Task StartGameServerAsync()
    {
        // Server status. Players in, Server Health
        await multiplayAllocationService.BeginServerCheck();

        if(!networkServer.OpenConnection(serverIP, serverPort))
        {
            Debug.LogWarning("NetworkServer did not start. :( ");
            return;
        }




    }


    public void Dispose()
    {

    }
}