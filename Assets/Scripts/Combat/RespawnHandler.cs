using System;
using Unity.Netcode;
using UnityEngine;

// Server logic
public class RespawnHandler : NetworkBehaviour
{

    [SerializeField] private NetworkObject playerPrefab;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

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
