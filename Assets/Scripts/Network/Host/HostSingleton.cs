using System.Threading.Tasks;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{

    private static HostSingleton instance;
    private HostGameManager gameManager;

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
    public void CreateHost()
    {
        gameManager = new HostGameManager();

    }
}
