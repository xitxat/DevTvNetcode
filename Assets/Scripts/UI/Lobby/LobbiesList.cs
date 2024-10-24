using System;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

//      COMPONENT OF MAINMENU/CANVAS/LOBBIESBACKGROUND  
public class LobbiesList : MonoBehaviour
{
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private Transform lobbyItemParent;
    [SerializeField] private LobbyItem lobbyItemPrefab; // NEEDS Lobby Item component!

    private bool isRefreshing;

    private void OnEnable()
    {
        RefreshList();
    }

    // Call from Button "Refresh"
    public async void RefreshList()
    {
        if (isRefreshing) { return; }

        isRefreshing = true;

        try
        {
            // Get Lobby List
            QueryLobbiesOptions options = new QueryLobbiesOptions();

            // can be set higher & displayed with pageination
            options.Count = 25;

            // Show custom lobbies
            options.Filters = new List<QueryFilter>()
            {
                    //  Show (non-full) lobbies in avail slot if room (< options.Count)
                new QueryFilter(
                    QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT, // >
                    value: "0"),
                    //  Private / Locked Lobby
                new QueryFilter(
                    QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ, // ==
                    value: "0")
            };

            // Get Lobbies
            // This await returns a QueryResponse
            QueryResponse lobbies =  await Lobbies.Instance.QueryLobbiesAsync(options);

            // Destroy old Lobby buttons and spawn in new
            foreach(Transform child in lobbyItemParent)
            {
                Destroy(child.gameObject);
            }


            // Spawn Lobby Container item prefab
            foreach(Lobby lobby in lobbies.Results)
            {
                LobbyItem lobbyItem = Instantiate(lobbyItemPrefab, lobbyItemParent);
                lobbyItem.Initialise(this, lobby);
            }


        }
        catch(LobbyServiceException exLSE)
        {
            Debug.Log(exLSE);
        }

        isRefreshing = false;
    }

    public  void JoinAsync(Lobby lobby)
    {
         mainMenu.JoinAsync(lobby);
    }
}
