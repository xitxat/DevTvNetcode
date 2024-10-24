using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;

// This is the PREFAB ServerManager.
//      refed by [SerFld] NetBootstrap Application Controller
// Singletons: Global access, no cached refs needed.
// QPort: Query port

public class ServerSingleton : MonoBehaviour
{

    public static ServerSingleton Instance { get; private set; }
    public ServerGameManager GameManager { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    //  UGS
    public async Task CreateServer(NetworkObject playerPrefab)
    {
        try
        {
            await UnityServices.InitializeAsync();

            if (NetworkManager.Singleton == null)
            {
                Debug.LogError("NetworkManager Singleton is null, server cannot start.");
                return;
            }

            GameManager = new ServerGameManager(
                ApplicationData.IP(),
                ApplicationData.Port(),
                ApplicationData.QPort(),
                NetworkManager.Singleton,
                playerPrefab
            );

            Debug.Log("ServerGameManager created successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error initializing the server: {e.Message}");
            GameManager = null; // Explicitly set to null in case of failure
        }

        if (GameManager == null)
        {
            Debug.LogError("GameManager is null after CreateServer. Check initialization.");
        }
    }


    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
