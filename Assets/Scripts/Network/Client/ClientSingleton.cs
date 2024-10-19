using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientSingleton : MonoBehaviour
{

    public ClientGameManager GameManager { get; private set; }

    private static ClientSingleton instance;

    public static ClientSingleton Instance
    {
        get
        {
            if(instance != null) { return instance; }

            instance = FindAnyObjectByType<ClientSingleton>();

            if(instance == null)
            {
               // Debug.LogError("<color=orange>No <ClientSingleton> in Scene!</color>");
                return null;
            }

            return instance;
        }
    }


    void Start()
    {
        DontDestroyOnLoad(gameObject);

    }

    //  Client creator knows who has been sucessful thru returned bool
    //  then ApplicationController LaunchInMode triggers MENU scene
    public async Task<bool> CreateClient()
    {
        GameManager = new ClientGameManager();

        // Go Authenticate before running other code
        return await GameManager.InitAsync();
    }

    //  Closing Game cleanly
    //  When this obj is Destroyed call & unsub from
    //   1: ClientGameManager : IDisposable & --> 2: NetworkClient : IDisposable
    private void OnDestroy()
    {
        GameManager?.Dispose();
    }



}
