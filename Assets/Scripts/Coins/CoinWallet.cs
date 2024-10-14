using System;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{


    [Header("Refs")]
    [SerializeField] private BountyCoin coinPrefab;
    [SerializeField] private Health health; // Health component triggers the spawn

    [Header("Settings")]
    [SerializeField] private float coinSpread = 3f; 
    [SerializeField] private int bountyCoinCount = 10; 
    [SerializeField] private float bountyPercentage = 50f; 
    [SerializeField] private int minBountyCoinValue = 5; // only drop if Pl coins => amount
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
        int bountyCalc = (int)(bountyPercentage / 100f);
        int bountyValue = TotalCoins.Value * bountyCalc;
        int bountyCoinValue = bountyValue / bountyCoinCount;

        // Excess coin check
        if (bountyCoinValue < minBountyCoinValue) { return; }

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

}
