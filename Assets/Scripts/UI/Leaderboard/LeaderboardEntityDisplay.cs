using TMPro;
using Unity.Collections;
using UnityEngine;

//  Handles each individual players stats onLeaderboard

public class LeaderboardEntityDisplay : MonoBehaviour
{

    // Exposed for Leaderboard
    public ulong ClientId { get; private set; }
    public int Coins { get; private set; }


    [SerializeField] private TMP_Text displayText;
    FixedString32Bytes playerName;


    public void Initialise(ulong clientId, FixedString32Bytes playerName, int coins)
    {
        ClientId = clientId;
        this.playerName = playerName;
        Coins = coins; // Dynamic update()

        UpdateText();
    }

    private void UpdateText()
    {
        displayText.text = $"1. {playerName} [{Coins}]";
        
    }

    public void UpdateCoins()
    {

    }

}
