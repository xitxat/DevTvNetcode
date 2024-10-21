using System;
using System.Collections;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{

    [SerializeField] private TankPlayer player;
    [SerializeField] private TMP_Text playerNameText;

    void Start()
    {
        StartCoroutine(WaitForPlayerNameSync());
    }

    private void HandlePlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        Debug.Log($"<color=yellow>Player name updated: {newName.ToString()}</color>");

        // Set name
        playerNameText.text = newName.ToString();
    }


    // DELAY 4 NAME SYNC
    private IEnumerator WaitForPlayerNameSync()
    {
        // Wait until the PlayerName is not empty (fully synced)
        while (string.IsNullOrEmpty(player.PlayerName.Value.ToString()))
        {
            yield return null; // Wait for the next frame
        }

        // Once the player name is synced, update the display
        HandlePlayerNameChanged(string.Empty, player.PlayerName.Value);

        // Sub to change value event of Name to handle any further updates
        player.PlayerName.OnValueChanged += HandlePlayerNameChanged;
    }

    void OnDestroy()
    {
        
        player.PlayerName.OnValueChanged -= HandlePlayerNameChanged;
    }



}
