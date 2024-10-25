using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

//  Handles each individual players stats onLeaderboard

public class LeaderboardEntityDisplay : MonoBehaviour
{

    // Exposed for Leaderboard
    public int TeamIndex { get; private set; } //br6.06
    public ulong ClientId { get; private set; }
    public int Coins { get; private set; }


    [SerializeField] private TMP_Text displayText;
    FixedString32Bytes displayName;


    //  Custom Initialise not Unity's InitialiZe
    public void Initialise(ulong clientId, FixedString32Bytes displayName, int coins)
    {
        Debug.Log($"<color=orange>Initialising Leaderboard Entity Display for ClientId: {clientId}, PlayerName: {displayName}, Coins: {coins}</color>");

        ClientId = clientId; // static
        this.displayName = displayName;

        UpdateCoins(coins); // Dynamic update()
    }

    public void Initialise(int teamIndex, FixedString32Bytes displayName, int coins)
    {
        TeamIndex = teamIndex;
        this.displayName = displayName;

        UpdateCoins(coins); // Dynamic update()
    }

    // Player & Team text colours
    public void SetColour(Color colour)
    {
        displayText.color = colour;
    }

    public void UpdateCoins(int coins)
    {
        Coins = coins;
        UpdateText();
    }
  

    public void UpdateText()
    {
        Debug.Log($"<color=orange>Updating Leaderboard Text: Rank: {transform.GetSiblingIndex() + 1}, PlayerName: {displayName}, Coins: {Coins}</color>");
        // siblinIndex Array from HandleLeaderboardEntitiesChanged()
        displayText.text = $"{transform.GetSiblingIndex() + 1}: {displayName} [{Coins}]";
        
    }



}
