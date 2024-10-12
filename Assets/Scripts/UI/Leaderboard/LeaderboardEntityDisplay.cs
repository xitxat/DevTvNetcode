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
        ClientId = clientId; // static
        this.playerName = playerName;

        UpdateCoins(coins); // Dynamic update()
    }

    public void UpdateCoins(int coins)
    {
        Coins = coins;
        UpdateText();
    }

    private void UpdateText()
    {
        displayText.text = $"1. {playerName} [{Coins}]";
        
    }



}
