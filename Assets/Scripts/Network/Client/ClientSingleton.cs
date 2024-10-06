using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{

    private static ClientSingleton instance;
    private ClientGameManager gameManager;

    public static ClientSingleton Instance
    {
        get
        {
            if(instance != null) { return instance; }

            instance = FindAnyObjectByType<ClientSingleton>();

            if(instance == null)
            {
                Debug.LogError("<color=orange>No <ClientSingleton> in Scene!</color>");
                return null;
            }

            return instance;
        }
    }


    void Start()
    {
        DontDestroyOnLoad(gameObject);

    }

    public async Task CreateClient()
    {
        gameManager = new ClientGameManager();

        // Go Authenticate before running other code
        await gameManager.InitAsync();
    }


}
