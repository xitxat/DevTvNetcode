using System;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{

    [SerializeField] private ClientSingleton clientPrefab;

    async void  Start()
    {
        DontDestroyOnLoad(gameObject);

        // Headless server (bool)
       await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);

    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {

        }
        else
        {
            ClientSingleton clientSingleton =  Instantiate(clientPrefab);

            await clientSingleton.CreateClient();
                //  wait for Auth, then:

            // Go to menu
        }
    }
}
