using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

// Server logic
// Created on  gameplay scene start
public class RespawnHandler : NetworkBehaviour
{

    [SerializeField] private NetworkObject playerPrefab;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        // Player may exist before scene creation (ie Self Host)
        // if so get any tanks that already exist...
        TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
        
        foreach(TankPlayer player in players)
        {
            HandlePlayerSpawned(player);
        }

        // if player joining after SPAWN via these Events...
        TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
    }


    public override void OnNetworkDespawn()
    {
        TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
    }

    private void HandlePlayerSpawned(TankPlayer player)
    {
        // Start listening to OnDie event
        //  OnDie passes thru health
        // Force pass to Event
        player.Health.OnDie += (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDespawned(TankPlayer player)
    {
        player.Health.OnDie -= (health) => HandlePlayerDie(player);

    }

    //  DIE
    private void HandlePlayerDie(TankPlayer player)
    {
        Destroy(player.gameObject);

        // wait for next frame to respawn
        StartCoroutine(RespawnPlayer(player.OwnerClientId));
    }

    //  RESPAWN
    private IEnumerator RespawnPlayer(ulong ownerClientID)
    {
        yield return null;

        // store
        NetworkObject playerInstance = Instantiate(
            playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);

        // Assign playerInstanc to this ownerClientID
        playerInstance.SpawnAsPlayerObject(ownerClientID);

    }

}
