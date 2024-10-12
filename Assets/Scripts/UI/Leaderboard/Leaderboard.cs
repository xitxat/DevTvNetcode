using Unity.Netcode;
using UnityEngine;

//   In Scene on GO GameHUD
//   SERVER : Track & Sync all leaderboard Data (custom networklist) < LeaderboardEntityState >

public class Leaderboard : NetworkBehaviour
{

    [SerializeField] private Transform leaderboardEntityHolder;
    [SerializeField] private LeaderboardEntityDisplay leaderboardEntityPrefab;

    // Server Stores & Syncs a LIST of all Ids, names, coins
    // Custom Network List <T> Server syncs clients
    private NetworkList<LeaderboardEntityState> leaderboardEntities; // init in Awake

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
            // Player has spawned so add it to the leaderboard
            case NetworkListEvent<LeaderboardEntityState>.EventType.Add:
                Instantiate(leaderboardEntityPrefab, leaderboardEntityHolder);
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
