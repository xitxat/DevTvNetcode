using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;

// Singletons: Global access, no cached refs needed.
// QPort: Query port

public class ServerSingleton : MonoBehaviour
{

    public ServerGameManager GameManager { get; private set; }

    private static ServerSingleton instance;

    public static ServerSingleton Instance
    {
        get
        {
            if (instance != null) { return instance; }

            instance = FindAnyObjectByType<ServerSingleton>();

            if (instance == null)
            {
                Debug.LogError("<color=orange>No <ServerSingleton> in Scene!</color>");
                return null;
            }

            return instance;
        }
    }


    void Start()
    {
        DontDestroyOnLoad(gameObject);

    }

    //  UGS
    public async Task CreateServer()
    {
        await UnityServices.InitializeAsync();

        // Read in Networking> Application data from cmd
        // Send to ServerGameManager
        GameManager = new ServerGameManager(
            ApplicationData.IP(),
            ApplicationData.Port(),
            ApplicationData.QPort(),
            NetworkManager.Singleton
            );

    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
