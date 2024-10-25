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
        HandlePlayerNameChanged(string.Empty, player.PlayerName.Value);

        // Sub to change value event of Name to handle any further updates
        player.PlayerName.OnValueChanged += HandlePlayerNameChanged;
    }

    private void HandlePlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        Debug.Log($"<color=yellow>Player name updated: {newName.ToString()}</color>");

        // Set name
        playerNameText.text = newName.ToString();
    }


    void OnDestroy()
    {
        player.PlayerName.OnValueChanged -= HandlePlayerNameChanged;
    }



}
