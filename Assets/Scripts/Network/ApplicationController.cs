using System;
using System.Threading.Tasks;
using UnityEngine;

// br 5.04
// Called by Client / Host / Server to establish Singleton
// On Client Auth GoTo Menu scene
public class ApplicationController : MonoBehaviour
{

    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;
    [SerializeField] private ServerSingleton serverPrefab;

    private ApplicationData appData;

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
            // Instanciate Application Data. runs AD constructor; returns cmd line IP, Port queries
            appData = new ApplicationData();

            // Spawn in Prefab
            ServerSingleton serverSingleton = Instantiate(serverPrefab);
            // Connect UGS
            await serverSingleton.CreateServer();

            await serverSingleton.GameManager.StartGameServerAsync();
        }
        else
        {
            //  Spawn HostSingleton first otherwise Client will start game before 
            //  Host(server) ~ throwing null HostSingleton  in Game Scene
            //  Both HostManager & ClientManager prefabs need to be in Scene's DontDestroyOnLoad
            HostSingleton hostSingleton = Instantiate(hostPrefab);
            hostSingleton.CreateHost();

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
}
