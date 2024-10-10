using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// HostGameManager creates this class
public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;

    private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();

    // constructor
    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        // triggered when Server is Connected to.[contains data]
        networkManager.ConnectionApprovalCallback += ApprovalCheck;

        networkManager.OnServerStarted += OnNetworkReady;
    }



    // Handle requests when Players connect
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
        Debug.Log(userData.userName);

        // Finish connection to server
        response.Approved = true;

        //  Spawn in Players
        response.CreatePlayerObject = true;
    }

    private void OnNetworkReady()
    {
        //  Called when Player Disconnects
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        // Remove ID of Disconnected client from BOTH Dictionaries
        if (clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            clientIdToAuth.Remove(clientId);

            authIdToUserData.Remove(authId);
        }


        // Store ID of Disconnected client to save score 

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