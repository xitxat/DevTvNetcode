using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

// br 5.04
// The ApplicationController is responsible for initializing the application in different modes:
// - Dedicated Server Mode
// - Host Mode
// It handles the loading of scenes and the initialization of singletons like ServerSingleton and HostSingleton.
public class ApplicationController : MonoBehaviour
{

    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;
    [SerializeField] private ServerSingleton serverPrefab;
    [SerializeField] private NetworkObject playerPrefab;


    private ApplicationData appData;

    private const string GameSceneName = "Game";


    // The entry point of the application.
    // Determines if the application is running as a dedicated server or client/host.
    async void  Start()
    {
        DontDestroyOnLoad(gameObject);

        // HEADLESS DEDICATED SERVER (bool)
        bool isDedicatedServer = SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;

        // Launch the application in the appropriate mode.
        await LaunchInMode(isDedicatedServer);
    }

    // Launches the application in either dedicated server mode or client/host mode.
    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
            // Throttle Frame Rate
            Application.targetFrameRate = 60;

            // Instanciate Application Data. runs AD constructor; returns cmd line IP, Port queries
            appData = new ApplicationData();

            // Load the Game Scene first
            await LoadGameSceneAsync();

            // Spawn in Prefab
            ServerSingleton serverSingleton = Instantiate(serverPrefab);

            // Initialize the server before loading the scene
            await serverSingleton.CreateServer(playerPrefab);

            // Ensure GameManager is initialized
            if (serverSingleton.GameManager == null)
            {
                Debug.LogError("GameManager is null after CreateServer, aborting server startup.");
                return;
            }


                    // Load the Game Scene // Connect UGS
                    //StartCoroutine(LoadGameSceneAsync(serverSingleton));

            // Start the game server after the scene is loaded
            await serverSingleton.GameManager.StartGameServerAsync();

        }
        else
        {
            // Host mode (client and server running in the same process).

            // Instantiate the HostSingleton and create the host (server).
            HostSingleton hostSingleton = Instantiate(hostPrefab);
            hostSingleton.CreateHost(playerPrefab);

            ClientSingleton clientSingleton =  Instantiate(clientPrefab);
            bool authenticated = await clientSingleton.CreateClient();
            //  wait for Auth, then:

            // Go to menu
            if (authenticated)
            {
                clientSingleton.GameManager.GoToMenu();
            }
        }
    }

    // Load client game scene  before Server
    // Create D. Server , yield, create scene
    // (not async together: sync issues like Leaderboard no display)
    private async Task LoadGameSceneAsync()
    {
        // Start loading the Game scene asynchronously.
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(GameSceneName);

        // Wait until the scene loading is complete.
        while (!asyncOperation.isDone)
        {
            // Yield control back to the calling context to prevent blocking.
            await Task.Yield();
        }

        // Optionally, you can add a delay to ensure all scene objects are initialized.
        // await Task.Delay(100); // Delay for 100 milliseconds.
    }
}
