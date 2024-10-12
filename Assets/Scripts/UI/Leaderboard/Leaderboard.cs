using Unity.Netcode;
using UnityEngine;

//   In Scene on GO GameHUD
//   Track & Sync all leaderboard Data (custom networklist) < LeaderboardEntityState >

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

    public override void OnNetworkSpawn()
    {

    }
    public override void OnNetworkDespawn()
    {

    }

    private void HandlePLayerSpawned(TankPlayer player)
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

    private void HandlePLayerDespawned(TankPlayer player)
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
