using System;
using Unity.Netcode;
using UnityEngine;

//  https://www.gamedev.tv/dashboard/courses/22
// BOUNTY COINS: less amount but higher value

public class CoinWallet : NetworkBehaviour
{


    [Header("Refs")]
    [SerializeField] private BountyCoin coinPrefab;
    [SerializeField] private Health health; // Health component triggers the spawn

    [Header("Settings")]
    [SerializeField] private float coinSpread = 3f; 
    [SerializeField] private int bountyCoinCount = 5; // # coins that will be spawned when the player dies. Fewer coins to increase value per coin
    [SerializeField] private float bountyPercentage = 50f; //* Loot drop half of wallet 50%
    [SerializeField] private int minBountyCoinValue = 5; //The minimum value of each bounty coin that is allowed to spawn.. only drop if Pl coins => * current amount in wallet. 
    [SerializeField] private LayerMask layerMask;


    private float coinRadius;
    private int coinVal;

    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        // Get actual coin size
        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;

        //  Listen & Spawn Bounty Coins
        health.OnDie += HandleDie;
    }    
    
    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }


        health.OnDie -= HandleDie;
    }

    private void HandleDie(Health health)
    {

        // Calculate Bounty Coin value
        //int bountyCalc = (int)(bountyPercentage / 100f);
        //int bountyValue = TotalCoins.Value * bountyCalc;
        //int bountyCoinValue = bountyValue / bountyCoinCount;

        int bountyValue = (int)(TotalCoins.Value * (bountyPercentage / 100f));
        int bountyCoinValue = bountyValue / bountyCoinCount;

        Debug.Log($"Bounty Coin Value: {bountyCoinValue}");
        Debug.Log($"TotalCoins: {TotalCoins.Value}, Bounty Value: {bountyValue}, Bounty Coin Value: {bountyCoinValue}");



        // Excess coin check
        if (bountyCoinValue < minBountyCoinValue) { return; }

        // Spawn Bounty coin loop
        for (int i = 0; i < bountyCoinCount; i++)
        {
            // Store first, to be able to set value
           BountyCoin coinInstance = Instantiate(coinPrefab, GetSpawnPoint(), Quaternion.identity);
            coinInstance.SetValue(bountyCoinValue);

            // Spawned objects require NetworkObject component
            //      Inherited from BASE Coin NetworkObject component
            // No ID , no owner on Spawn
            coinInstance.NetworkObject.Spawn();
        }

    }


    //  COLLECT COINS
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Coin>(out Coin coin)) { return; }

        //  if client, hide coin renderer
            coinVal = coin.Collect();
        
        if (!IsServer) { return; }

        if (IsServer)
        {
           TotalCoins.Value += coinVal;
        }

    }

    public void SpendCoins(int costToFire)
    {
        TotalCoins.Value -= costToFire;
    }


    // Via dead player spread radius for Bounty Coin
    private Vector2 GetSpawnPoint()
    {

        while (true)
        {
            // insideUnitCircle is 1 unit, cast
            Vector2 spawnPoint = (Vector2)transform.position +
                UnityEngine.Random.insideUnitCircle * coinSpread;

            // Use Physics2D.OverlapCircle to check if the area is free
            Collider2D hitCollider = Physics2D.OverlapCircle(spawnPoint, coinRadius, layerMask);
            if (hitCollider == null)
            {
                return spawnPoint; // No colliders found, return spawn point
            }
        }
    }
}
