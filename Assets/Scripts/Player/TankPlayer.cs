using System;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

//          CUSTOMISE .THIS PLAYER
//      NAME
//      CINEMACHINE
public class TankPlayer : NetworkBehaviour
{

    [Header("Refs")]
    [SerializeField] private CinemachineCamera virtualCamera;

    // expose private property in inspector with field:
    [field: SerializeField] public Health Health { get; private set; }


    [Header("Settings")]
    [SerializeField] private int ownerPriority = 15;

    //  Sync names. Cant sync normal strings.
    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

    // Server only invocation
    public static event Action<TankPlayer> OnPlayerSpawned;
    public static event Action<TankPlayer> OnPlayerDespawned;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Get Players name (Pass in OwnersId & get name) returns User data obj
            UserData userData = 
                HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);

            // Bexause we are server, set Value now
            // & trigger network sync
            PlayerName.Value =  userData.userName;

            OnPlayerSpawned?.Invoke(this);
        }

        if (IsOwner)
        {
            virtualCamera.Priority = ownerPriority;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
    }
}
