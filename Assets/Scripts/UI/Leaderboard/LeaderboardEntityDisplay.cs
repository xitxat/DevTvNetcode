using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

//  Handles each individual players stats onLeaderboard

public class LeaderboardEntityDisplay : MonoBehaviour
{

    // Exposed for Leaderboard
    public ulong ClientId { get; private set; }
    public int Coins { get; private set; }


    [SerializeField] private TMP_Text displayText;
    [SerializeField] private Color myColor;
    FixedString32Bytes playerName;


    //  Custom Initialise not Unity's InitialiZe
    public void Initialise(ulong clientId, FixedString32Bytes playerName, int coins)
    {
        ClientId = clientId; // static
        this.playerName = playerName;

        // If Me (colourise name)
        // Default Font Face HDR color in LeaderBoardEntity prefab. 
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            displayText.color = myColor;
        }

        UpdateCoins(coins); // Dynamic update()
    }

    public void UpdateCoins(int coins)
    {
        Coins = coins;
        UpdateText();
    }
  

    public void UpdateText()
    {
        // siblinIndex Array from HandleLeaderboardEntitiesChanged()
        displayText.text = $"¦{transform.GetSiblingIndex() + 1}¦ {playerName} [{Coins}]";
        
    }



}
