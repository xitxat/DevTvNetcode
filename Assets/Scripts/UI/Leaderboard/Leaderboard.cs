using Unity.Netcode;
using UnityEngine;

//   In Scene on GO GameHUD
//   Track & Sync all leaderboard Data (custom networklist) < LeaderboardEntityState >

public class Leaderboard : NetworkBehaviour
{

    [SerializeField] private Transform leaderboardEntityHolder;
    [SerializeField] private LeaderboardEntityDisplay leaderboardEntityPrefab;

    // Server Syncs a LIST of all Ids, names, coins
    // Custom Network List <T> Server syncs clients
    private NetworkList<LeaderboardEntityState> leaderboardEntities; // init in Awake

    private void Awake()
    {
        leaderboardEntities = new NetworkList<LeaderboardEntityState>();
    }
}
