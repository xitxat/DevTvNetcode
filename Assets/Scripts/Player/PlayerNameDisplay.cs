using System.Collections;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNameDisplay : NetworkBehaviour
{

    [SerializeField] private TankPlayer player;
    [SerializeField] private TMP_Text playerNameText;



    private IEnumerator WaitForNameUpdate()
    {
        yield return new WaitForSeconds(2f); // Small delay to ensure server-side synchronization
        HandlePlayerNameChanged(string.Empty, player.PlayerName.Value);
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            player.PlayerName.OnValueChanged += HandlePlayerNameChanged;
            StartCoroutine(WaitForNameUpdate());
        }
    }


    private void HandlePlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        // Update the text with the new player name
        if (!string.IsNullOrEmpty(newName.ToString()))
        {
            playerNameText.text = newName.ToString();
            Debug.Log($"<color=yellow>Player name updated to: {newName}</color>");
        }
        else
        {
            Debug.LogWarning("<color=yellow>Player name not yet synchronized.</color>");
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            player.PlayerName.OnValueChanged -= HandlePlayerNameChanged;
        }
    }



}
