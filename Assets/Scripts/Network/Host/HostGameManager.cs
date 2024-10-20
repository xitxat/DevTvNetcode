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

//  Use Try Catch for Network Calls
//  Coroutine not in class HostGameManager.
//      Accessed via HostSingleton.Instance.StartCoroutine
public class HostGameManager : IDisposable
{

    public NetworkServer NetworkServer { get; private set; } // exposed to TankPlayer (Name extraction)

    private Allocation allocation;
    private NetworkObject playerPrefab;

    private string joinCode;
    private string lobbyId;
    private const int MaxConnections = 20;
    private const string GameSceneName = "Game";


    // Construct with playerPrefab to Delay PLayer Spawn (passed down from NetworkServer)
    public HostGameManager(NetworkObject playerPrefab)
    {
        this.playerPrefab = playerPrefab;
    }


    public async Task StartHostAsync(bool isPrivate)
    {

        //  CREATE, Assign & Store the allocation with ID# (Join Code)
        try
        {
            allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        //  Join Code
        try
        {
            joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"<color=teal>Join Code: {joinCode}</color>");
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        #region RELAY
        //  Unity Transport switch to RELAY MODE
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        //  Set connection type ("udp" User Data Protocol = RELAY) [IP,Port]
        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayServerData);
        #endregion


        #region LOBBY
        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();

            lobbyOptions.IsPrivate = isPrivate; // TOGGLE for player's lobby. Private need Jcode

            // Set screen readable Relay data (Alloc Jcode, visible to members of Lobby ) to Lobby 
            lobbyOptions.Data = new System.Collections.Generic.Dictionary<string, DataObject>()
            {
                { "JoinCode", new DataObject(
                    visibility: DataObject.VisibilityOptions.Member,
                    value: joinCode)}
            };

            // Ref Player Name to name scene with
            string playerName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "NoName");

            // CreateLobbyAsync returns a Lobby ID (used for Heartbeat ping)
            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(
                $"{playerName}'s Lobby", MaxConnections, lobbyOptions);

            lobbyId = lobby.Id;

            // Reroute keep awake Ping Coroutine thru a monobehaviour
            HostSingleton.Instance.StartCoroutine(HeartbeatLobby(15));
        }
        catch (LobbyServiceException exLSE)
        {
            Debug.Log(exLSE);
            // Don't start Host
            return;
        }
        #endregion

        #region NETWORK SERVER

        // Create
        NetworkServer = new NetworkServer(NetworkManager.Singleton, playerPrefab);

        #endregion

        #region HOST

        // Set Data from Network Server ApprovalCheck()
        UserData userData = new UserData
        {
            userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "NoNameO"),
            userAuthId = AuthenticationService.Instance.PlayerId

        };

        // Repackage Package: JSON <=> Byte Array
        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        // Send over network on Connecting to Server
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;


        //  Start
        NetworkManager.Singleton.StartHost();

        //  sub to Button "Exit Arena"
        NetworkServer.OnClientLeft += HandleClientLeft;


        //  Scene
        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);

        #endregion

    }

    // Heartbeat ping
    // coroutines need to be called from monobehaviours 
    // Reroute thru HostSingleton
    // Stops on GameShutdown
    private IEnumerator HeartbeatLobby(float waitTimeSeconds)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            // Debug.Log("PING");
            yield return delay;
        }
    }


    #region SHUTDOWN
    // Handle unexpected server shutdown & Exit Arena
    public void Dispose()
    {
         ShutDown();

    }

    public async void ShutDown()
    {

        // Stop Lobby
        if (string.IsNullOrEmpty(lobbyId)) { return; }
        
        //  Stop Heartbeat coroutine . out of class access
        // Use nameof(containing method)
        HostSingleton.Instance.StartCoroutine(nameof(HeartbeatLobby));

        try
        {
            await Lobbies.Instance.DeleteLobbyAsync(lobbyId);
        }
        catch (LobbyServiceException exLSE)
        {
            Debug.Log(exLSE);
        }

        // Won;t retry with empty str on a Fail
        lobbyId = string.Empty;
        

        NetworkServer.OnClientLeft -= HandleClientLeft;


        NetworkServer?.Dispose();
    }

    private async void HandleClientLeft(string authId)
    {
        try
        {
            // If you own the Lobby, Try:
           await  LobbyService.Instance.RemovePlayerAsync(lobbyId, authId);
        }
        catch (LobbyServiceException exLSE)
        {
            Debug.Log(exLSE);
        }
    }
    #endregion


}