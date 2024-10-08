using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer 
{
    private NetworkManager networkManager;

    // constructor
public NetworkServer( NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        // triggered when Server is Connected to.[contains data]
        networkManager.ConnectionApprovalCallback += ApprovalCheck;
    }

    private void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request, 
        NetworkManager.ConnectionApprovalResponse response)
    {
        //  Package: JSON <=> Byte Array | Serialization/DeSerialization
        //  returns Object with data
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);

        // Retrieve Data <Our custom filter>
        UserData userData = JsonUtility.FromJson<UserData>(payload);

        Debug.Log(userData.userName);

        // Finish connection to server
        response.Approved = true;
    }
}
