using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;


//  Which Lobby are we.
//  Update UI

public class LobbyItem : MonoBehaviour
{

    [SerializeField] private TMP_Text lobbyNameText;
    [SerializeField] private TMP_Text lobbyPlayerText;

    private LobbiesList lobbiesList;
    private Lobby lobby;

    public void Initialise(LobbiesList lobbiesList,  Lobby lobby)
    {
        this.lobbiesList = lobbiesList; // Store locally / pass to private
        this.lobby = lobby;

        lobbyNameText.text = lobby.Name;
        lobbyPlayerText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    // button
    public void Join()
    {
        lobbiesList.JoinAsync(lobby);
    }
}
