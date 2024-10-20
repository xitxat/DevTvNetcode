using TMPro;
using Unity.Netcode;
using UnityEngine;

// ADD TO GameHUD G.O.

// Button: "Exit Arena" : ref this.LeaveGame
public class GameHUD : NetworkBehaviour
{
    [SerializeField] private TMP_Text lobbyCodeText;
    //  [SerializeField] private TMP_Text ~Player name text; NameSelector , ClientGameManager

    public void LeaveGame()
    {
        // For Self Hosting
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.GameManager.ShutDown();
        }

        // For Client wanting to Leave
        ClientSingleton.Instance.GameManager.Disconnect();
    }
}
