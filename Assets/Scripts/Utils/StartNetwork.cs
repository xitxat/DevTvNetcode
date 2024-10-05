using Unity.Netcode;
using UnityEngine;

public class StartNetwork : MonoBehaviour
{
    [SerializeField] private Canvas serverCanvas;

    private void Start()
    {
        if (serverCanvas != null)
        {
            serverCanvas.gameObject.SetActive(false);
        }

        // Check whether to start as Host or Client based on a simple key press or condition
        if (Application.isEditor)
        {
            Debug.Log("<color=orange>+++ Running in Multiplayer Play Mode +++</color>");
            Debug.Log("<color=cyan>Press 'H' to Start as Host,'C' as Client or NB: 'V' as Server</color>");
        }
    }

    private void Update()
    {
        // Simple input to decide between Host and Client for testing purposes
        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                StartHost();
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                StartClient();
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                StartServer();
            }
        }
    }

    private void StartHost()
    {
        Debug.Log("<color=green>Starting as Host</color>");
        NetworkManager.Singleton.StartHost();
    }

    private void StartClient()
    {
        Debug.Log("<color=green>Starting as Client</color>");
        NetworkManager.Singleton.StartClient();
    }

    private void StartServer()
    {
        Debug.Log("<color=green>Starting as Server</color>");
        NetworkManager.Singleton.StartServer();

        // Toggle the ServerCanvas when starting the server
        //if (serverCanvas != null)
        //{
        //    serverCanvas.gameObject.SetActive(true); // Show the server canvas when the server starts
        //}
    }

}
