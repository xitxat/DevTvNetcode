using System;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{

    [SerializeField] private TankPlayer player;
    [SerializeField] private TMP_Text playerNameText;

    void Start()
    {
        // Force reread / update of name incase of late init
        HandlePlayerNameChanged(string.Empty, player.PlayerName.Value);

        // Sub to change vale event of Name
        player.PlayerName.OnValueChanged += HandlePlayerNameChanged;

    }

    private void HandlePlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        // Set name
        playerNameText.text = newName.ToString();
    }

    void OnDestroy()
    {
        
        player.PlayerName.OnValueChanged -= HandlePlayerNameChanged;
    }
}
