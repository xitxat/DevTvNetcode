using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{

    [SerializeField] private TankPlayer player;
    [SerializeField] private TMP_Text playerNameText;

    void Start()
    {
        // Once the player name is synced, update the display
            // HandlePlayerNameChanged(string.Empty, player.PlayerName.Value);
        HandlePlayerNameChanged("OldName", player.PlayerName.Value);

        // Sub to change value event of Name to handle any further updates
        player.PlayerName.OnValueChanged += HandlePlayerNameChanged;
    }

    private void HandlePlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        Debug.Log($"<color=yellow>Player name updated: {oldName.ToString()}</color>");

        // Set name
        playerNameText.text = newName.ToString();
        Debug.Log($"<color=yellow>Player name set to: {newName.ToString()}</color>");
    }


    void OnDestroy()
    {
        player.PlayerName.OnValueChanged -= HandlePlayerNameChanged;
    }



}
