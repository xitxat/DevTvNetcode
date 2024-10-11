using System;
using Unity.Netcode;
using UnityEngine;

// Server logic
// this is inside gameplay scene
public class RespawnHandler : NetworkBehaviour
{

    [SerializeField] private NetworkObject playerPrefab;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        // Player may exist before scene creation (ie Self Host)
        TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);

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
        throw new NotImplementedException();
    }

    private void HandlePlayerDespawned(TankPlayer player)
    {
        throw new NotImplementedException();
    }


}
