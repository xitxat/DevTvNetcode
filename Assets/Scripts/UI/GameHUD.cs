using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameHUD : MonoBehaviour
{

public void LeaveGame()
    {
        // For Self Hosting
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.GameManager.ShutDown();
        }
    }
}
