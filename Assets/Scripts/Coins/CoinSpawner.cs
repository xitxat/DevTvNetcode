using Unity.Netcode;
using UnityEngine;


//  PHYSICS2D MATRIX COINS ARE ON PICKUPS WHICH INTERACTS WITH DEFAULT (PLAYER/TANK)
public class CoinSpawner : NetworkBehaviour
{

    [SerializeField] private RespawningCoin coinPrefab;

    [Header("Settings")]
    [SerializeField] private int maxCoins = 20;
    [SerializeField] private int coinValue = 10;
    [SerializeField] private Vector2 xSpawnRange;
    [SerializeField] private Vector2 ySpawnRange;
    [SerializeField] private LayerMask layerMask;


    private float coinRadius;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        // Get actual coin size
        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;

        //  Spawn Initial Coins
        for (int i = 0; i < maxCoins; i++)
        {
            SpawnCoin();
        }
    }

    private void SpawnCoin()
    {
        // In Clients
        RespawningCoin coinInstance = Instantiate(
            coinPrefab,
            GetSpawnPoint(),
            Quaternion.identity);

        coinInstance.SetValue(coinValue);
        // no client owner empty() Spawn(). just for server
        coinInstance.GetComponent<NetworkObject>().Spawn();

        //  SUBSCRIBE to the Respawner
        coinInstance.OnCollected += HandleCoinCollected;
    }

    private void HandleCoinCollected(RespawningCoin coin)
    {
        //  Get new point
        coin.transform.position = GetSpawnPoint();

        coin.Reset();
    }

    private Vector2 GetSpawnPoint()
    {
        float x = 0;
        float y = 0;
        while (true)
        {
            x = Random.Range(xSpawnRange.x, xSpawnRange.y);
            y = Random.Range(ySpawnRange.x, ySpawnRange.y);
            Vector2 spawnPoint = new Vector2(x, y);
            // Use Physics2D.OverlapCircle to check if the area is free
            Collider2D hitCollider = Physics2D.OverlapCircle(spawnPoint, coinRadius, layerMask);
            if (hitCollider == null)
            {
                return spawnPoint; // No colliders found, return spawn point
            }
        }
    }

}
