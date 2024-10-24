using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

// ADD TO GameHUD G.O.

// Button: "Exit Arena" : ref this.LeaveGame
// Access Network variables with .Value
public class GameHUD : NetworkBehaviour
{
    [SerializeField] private TMP_Text lobbyCodeText;

    // TODO User Player name display on Menu scene
    //  [SerializeField] private TMP_Text ~Player name text; NameSelector , ClientGameManager

    // Store Join Code text [init () with a default value ""]
    private NetworkVariable<FixedString32Bytes> lobbyCode =
        new NetworkVariable<FixedString32Bytes>("");

    // GET JOIN CODE
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            lobbyCode.OnValueChanged += HandleLobbyCodeChanged;
            //Force sync, resync, update, refresh
            HandleLobbyCodeChanged(string.Empty, lobbyCode.Value);
        }

        if (!IsHost) { return; }
        lobbyCode.Value = HostSingleton.Instance.GameManager.JoinCode;
    }


    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            lobbyCode.OnValueChanged -= HandleLobbyCodeChanged;
        }
    }


        public void LeaveGame()
    {
        // For Self Hosting
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.GameManager.Shutdown();
        }

        // For Client wanting to Leave
        ClientSingleton.Instance.GameManager.Disconnect();
    }

    private void HandleLobbyCodeChanged(FixedString32Bytes oldCode, FixedString32Bytes newCode)
    {
        lobbyCodeText.text = newCode.ToString();
    }

}
