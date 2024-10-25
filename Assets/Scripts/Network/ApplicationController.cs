using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

// br 5.04
// Called by Client / Host / Server to establish Singleton
// On Client Auth GoTo Menu scene

// Refkr: Move Scene Loading after Server Initialization
// 1. Instantiate the ServerSingleton Prefab
// 2. Load the Scene Asynchronously
// 3. Create and Start the Server After Scene Load
public class ApplicationController : MonoBehaviour
{

    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;
    [SerializeField] private ServerSingleton serverPrefab;
    [SerializeField] private NetworkObject playerPrefab;


    private ApplicationData appData;

    private const string GameSceneName = "Game";


    // Entry to Dedicated server
    async void  Start()
    {
        DontDestroyOnLoad(gameObject);

        // HEADLESS DEDICATED SERVER (bool)
       await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);

    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
            // Throttle Frame Rate
            Application.targetFrameRate = 60;

            // Instanciate Application Data. runs AD constructor; returns cmd line IP, Port queries
            appData = new ApplicationData();

            // Instantiate the server prefab to use after the scene is loaded
            ServerSingleton serverSingleton = Instantiate(serverPrefab);

            // Start scene loading AFTER instantiating the server prefab
            StartCoroutine(LoadGameSceneAsync(serverSingleton));

            // Ensure GameManager is initialized
            if (serverSingleton.GameManager == null)
            {
                Debug.LogError("GameManager is null after CreateServer, aborting server startup.");
                return;
            }




        }
        else
        {
            //  Spawn HostSingleton first otherwise Client will start game before 
            //  Host(server) ~ throwing null HostSingleton  in Game Scene
            //  Both HostManager & ClientManager prefabs need to be in Scene's DontDestroyOnLoad
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
    private IEnumerator LoadGameSceneAsync(ServerSingleton serverSingleton)
    {
        //  LOAD SCENE 
        // From Find Match go straight to Game scene , not menu
        AsyncOperation   asyncOperation =  SceneManager.LoadSceneAsync(GameSceneName);

        // scene not loaded
        while (!asyncOperation.isDone)
        {
            // skip to next frame untill scene loaded
            yield return null;
        }

        // Connect UGS
        //  CREATE dedicated server AFTER the game scene is loaded
        Task createServerTask =  serverSingleton.CreateServer(playerPrefab);
        // Not asyny so Task & yield
        yield return new WaitUntil(() => createServerTask.IsCompleted);


        // & START
        Task startServerTask =  serverSingleton.GameManager.StartGameServerAsync();
        yield return new WaitUntil(() => startServerTask.IsCompleted);

        Debug.Log("¦| Server fully initialized and started.");
    }
}
