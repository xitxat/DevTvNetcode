using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

// HostGameManager creates this class
public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;
    private NetworkObject playerPrefab;

    public Action<UserData> OnUserJoined; // backfilling
    public Action<UserData> OnUserLeft;

    public Action<string> OnClientLeft;  // authId is  a string

    private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();

    // constructor
    public NetworkServer(NetworkManager networkManager, NetworkObject playerPrefab)
    {
        this.networkManager = networkManager;
        this.playerPrefab = playerPrefab; // used to delay player spawn

        // triggered when Server is Connected to.[contains data]
        networkManager.ConnectionApprovalCallback += ApprovalCheck;

        networkManager.OnServerStarted += OnNetworkReady;
    }

    public bool OpenConnection(string ip, int port)
    {  // called by BeginServerCheck(); receives return from this t/f
        // set connection Data via transport component
        UnityTransport transport = networkManager.gameObject.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, (ushort)port);
        return networkManager.StartServer();
    }

    // Handle requests when Players connect / User join
    private void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        //  Package: JSON <=> Byte Array | Serialization/DeSerialization
        //  returns Object with data
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);

        // Retrieve Data <Our custom filter>
        UserData userData = JsonUtility.FromJson<UserData>(payload);

        //  Store User ID and data
        //  Request to set Value:    Key         | Value
        //  Auto .add ~ update if already exists
        clientIdToAuth[request.ClientNetworkId] = userData.userAuthId;
        authIdToUserData[userData.userAuthId] = userData;
        OnUserJoined?.Invoke(userData); // add to backfiller

        // Use discard (`_`) for async call
        _ = SpawnPlayerDelayed(request.ClientNetworkId, userData);

        // Finish connection to server
        response.Approved = true;
        //  Do NOt Spawn in Players, delay action
        response.CreatePlayerObject = false;
    }

    // DELAY SPAWN
    // sync time, avoid racing spawner
    private async Task SpawnPlayerDelayed(ulong clientId, UserData userData)
    {
        await Task.Delay(1500);

        // Assign a random spawn position 
        Vector3 randomSpawnPos = SpawnPoint.GetRandomSpawnPos();

        // access MonoBehavior via GameObject & assign to client
        NetworkObject playerInstance =
             GameObject.Instantiate(playerPrefab, randomSpawnPos, Quaternion.identity);

        playerInstance.SpawnAsPlayerObject(clientId);
        Debug.Log($"<color=yellow>Spawned: {userData.userName} at: {randomSpawnPos}</color>");

    }


    private void OnNetworkReady()
    {
        //  Called when Player Disconnects
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    // Remove player data from List
    private void OnClientDisconnect(ulong clientId)
    {
        // Remove ID of Disconnected client from BOTH Dictionaries
        if (clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            clientIdToAuth.Remove(clientId);
            // Remove user from backfiller
            OnUserLeft?.Invoke(authIdToUserData[authId]); 

            authIdToUserData.Remove(authId);

            // Remove Leaving player from Lobby
            // Using authId to interact with Lobby Service
            OnClientLeft?.Invoke(authId);

        }


        // Store ID of Disconnected client to save score 

    }

    // Retrrieve Player name from UserData Dictionary
    // if1: clientID from dict clientIdToAuth passed to Dict UserData
    // if2: UserData out  contains name
    public UserData GetUserDataByClientId(ulong clientId)
    {
        if(clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            if(authIdToUserData.TryGetValue(authId, out UserData data))
            {
                // contains Player Name
                return data;
            }

            return null;
        }

        return null;
    }

    public void Dispose()
    {
        if(networkManager == null) { return; }

        //  Unsub from all NetworkServer events
        networkManager.ConnectionApprovalCallback -= ApprovalCheck;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        networkManager.OnServerStarted -= OnNetworkReady;

        if (networkManager.IsListening)
        {
            networkManager.Shutdown();
        }

    }
}