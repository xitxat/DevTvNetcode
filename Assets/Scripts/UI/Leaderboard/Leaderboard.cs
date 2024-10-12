using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

//   In Scene on GO GameHUD
//   SERVER : Track & Sync all leaderboard Data (custom networklist) < LeaderboardEntityState >
// NB: Initialise() is defined in LeaderboardEntityDisplay.cs NOTR Unity API Initialize()

public class Leaderboard : NetworkBehaviour
{

    [SerializeField] private Transform leaderboardEntityHolder;
    [SerializeField] private LeaderboardEntityDisplay leaderboardEntityPrefab;

    // Server Stores & Syncs a LIST of all PLAYER DATA: Ids, names, coins
    // Custom Network List <T> Server syncs clients
    private NetworkList<LeaderboardEntityState> leaderboardEntities; // init in Awake (Network obj)

    // List of lb prefabs for use in SWITCH
    // init here (!Network obj)
    private List<LeaderboardEntityDisplay> entityDisplays = new List<LeaderboardEntityDisplay>();

    private void Awake()
    {
        leaderboardEntities = new NetworkList<LeaderboardEntityState>();
    }

    //  SPAWN LEADERBOARD
    public override void OnNetworkSpawn()
    {
        // GET CLIENTS TO lISTEN
        if (IsClient)
        {
            leaderboardEntities.OnListChanged += HandleLeaderboardEntitiesChanged;

            // Already added Self check
            foreach(LeaderboardEntityState entity in leaderboardEntities)
            {
                HandleLeaderboardEntitiesChanged( new NetworkListEvent<LeaderboardEntityState>
                    {
                    // Add other players
                    Type = NetworkListEvent<LeaderboardEntityState>.EventType.Add,
                    Value = entity
                });
            }
        }

        if (IsServer) 
        { 
            TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
        
            foreach (TankPlayer player in players)
            {
                // Makes sure host is on LEaderboard
                HandlePlayerSpawned(player);
            }

            // if player joining after SPAWN via these Events...
            TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
            
        }
    }


    //  DESPAWN LEADERBOARD
    public override void OnNetworkDespawn()
    {
        // GET CLIENTS TO lISTEN
        if (IsClient)
        {
            leaderboardEntities.OnListChanged -= HandleLeaderboardEntitiesChanged;
        }

        if (IsServer)
        {
            TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
        }
    }

    private void HandleLeaderboardEntitiesChanged(NetworkListEvent<LeaderboardEntityState> changeEvent)
    {
        // query changeEvent if vale changed & how;ie: added, removed, updated
        // Add
        switch (changeEvent.Type)
        {
            // ADD
            case NetworkListEvent<LeaderboardEntityState>.EventType.Add:
                // Don't reAdd ourselves from the changeEvent
                // Sort / Filter Linq. where => [foreach oneliner]
                if(entityDisplays.Any(x => x.ClientId == changeEvent.Value.ClientId))
                {
                    // store to List entityDisplays & spawn it
                   LeaderboardEntityDisplay leaderboardEntity = 
                        Instantiate(leaderboardEntityPrefab, leaderboardEntityHolder);
                    leaderboardEntity.Initialise(
                        changeEvent.Value.ClientId,
                        changeEvent.Value.PlayerName,
                        changeEvent.Value.Coins);

                    entityDisplays.Add(leaderboardEntity);
                }
                break;

            // REMOVE
            case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                // Find if Dead, leave game, etc
                // Default == null
                // Get 1st element that matches this condition
                LeaderboardEntityDisplay displayToRemove =
                    entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);

                // if found REMOVE
                //1st remove parent GO [container] / detach from leaderboard, minimise bugs
                if (displayToRemove != null)
                {
                    displayToRemove.transform.SetParent(null);
                    Destroy(displayToRemove.gameObject);
                    entityDisplays.Remove(displayToRemove);
                }
                break;

            // UPDATE score / coins
            case NetworkListEvent<LeaderboardEntityState>.EventType.Value:
                LeaderboardEntityDisplay displayToUpdate =
                    entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
                if(displayToUpdate != null)
                {
                    displayToUpdate.UpdateCoins(changeEvent.Value.Coins);
                }
                break;
        }


    }

    private void HandlePlayerSpawned(TankPlayer player)
    {
        // Publish to Leaderboard
        // Add to LIst
        leaderboardEntities.Add(new LeaderboardEntityState
        {
            ClientId = player.OwnerClientId,
            PlayerName = player.PlayerName.Value,
            Coins = 0
        });
    }

    private void HandlePlayerDespawned(TankPlayer player)
    {
        // Unity6.x may empty out leaderboard on game stop
        if(leaderboardEntities == null) { return; }

        // Find the matching player
        foreach(LeaderboardEntityState entity in leaderboardEntities)
        {
            if(entity.ClientId != player.OwnerClientId) { continue; }

            leaderboardEntities.Remove(entity);
            break;
        }
    }
}
