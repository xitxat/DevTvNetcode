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

//  Use Try Catch for Network Calls
public class HostGameManager 
{

    private Allocation allocation;
    private NetworkServer networkServer;
    private string joinCode;
    private string lobbyId;
    private const int MaxConnections = 20;
    private const string GameSceneName  = "Game";

    public async Task StartHostAsync()
    {

            //  CREATE, Assign & Store the allocation with ID# (Join Code)
        try
          {
           allocation =  await Relay.Instance.CreateAllocationAsync(MaxConnections);
        }
        catch(Exception e)
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
            lobbyOptions.IsPrivate = false; // Add Check box for player's lobby. Private need Jcode
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
        catch(LobbyServiceException exLSE)
        {
            Debug.Log(exLSE);
            // Don't start Host
            return;
        }
        #endregion

        #region NETWORK SERVER

        // Create
        networkServer = new NetworkServer(NetworkManager.Singleton);

        #endregion

        #region HOST

        // Set Data from Network Server ApprovalCheck()
        UserData userData = new UserData
        {
            userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "NoNameO")
        };

        // Repackage Package: JSON <=> Byte Array
        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        // Send over network on Connecting to Server
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;


        //  Start
        NetworkManager.Singleton.StartHost();


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



}
