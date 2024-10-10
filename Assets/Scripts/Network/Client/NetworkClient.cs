using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;


//  IDisposable: gain access to monobehaviours
public class NetworkClient : IDisposable 
{

    private NetworkManager networkManager;

    private const string MenuSceneName = "Menu";


    // constructor
    public NetworkClient(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        // triggered when Server is Connected to.[contains data]
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }



    private void OnClientDisconnect(ulong clientId)
    {
        // Allows rejoin game without reopening app.
        // Server shutdown sends clients back to Menu

        // Check if we are a client that hosts a server
        //   Don't disconnect
        if (clientId != 0 && clientId != networkManager.LocalClientId) { return; }

        // Change scene for client stranded in "GAME" upon an error, etc,
        //  Go back to main menu
        if (SceneManager.GetActiveScene().name != MenuSceneName)
        {
            SceneManager.LoadScene(MenuSceneName);
        }

        // If already on Main "MENU" & error / timeout
        if (networkManager.IsConnectedClient)
        {
            networkManager.Shutdown();
        }


    }

    public void Dispose()
    {
        if (networkManager != null)
        {
            networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        }


    }

}