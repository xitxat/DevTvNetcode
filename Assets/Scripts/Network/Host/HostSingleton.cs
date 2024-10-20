using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

// Singletons: Global access, no cached refs needed.

public class HostSingleton : MonoBehaviour
{

    public HostGameManager GameManager { get; private set; }

    private static HostSingleton instance;

    public static HostSingleton Instance
    {
        get
        {
            if (instance != null) { return instance; }

            instance = FindAnyObjectByType<HostSingleton>();

            if (instance == null)
            {
                Debug.LogError("<color=orange>No <HostSingleton> in Scene!</color>");
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
    public void CreateHost(NetworkObject playerPrefab)
    {
        GameManager = new HostGameManager(playerPrefab);

    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
