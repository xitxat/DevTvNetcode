using System.Threading.Tasks;
using UnityEngine;

// Singletons: Global access, no cached refs needed.

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

    //  No need to await to create
    public void CreateServer()
    {
        GameManager = new ServerGameManager();

    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
