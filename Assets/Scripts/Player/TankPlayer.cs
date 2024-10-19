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
    [SerializeField] private SpriteRenderer minimapIconRenderer;
    [SerializeField] private Texture2D crosshair;

    // expose private property in inspector with field:
    [field: SerializeField] public Health Health { get; private set; }

    // Exposed to update the Leader board coun count
    [field: SerializeField] public CoinWallet Wallet { get; private set; }


    [Header("Settings")]
    [SerializeField] private int ownerPriority = 15;
    [SerializeField] private Color ownerColor;


    //  Sync names. Cant sync normal strings.
    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

    // Server only invocation
    public static event Action<TankPlayer> OnPlayerSpawned;
    public static event Action<TankPlayer> OnPlayerDespawned;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {

            UserData userData = null;

            if (IsHost)
            {
                // Get Players name (Pass in OwnersId & get name) returns User data obj
                userData =
                   HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            }
            else
            {
                // DEDICATED SERVER
                userData = 
                    ServerSingleton.Instance.GameManager.Networkserver
            }




            // Bexause we are server, set Value now
            // & trigger network sync
            PlayerName.Value =  userData.userName;

            OnPlayerSpawned?.Invoke(this);
        }

        if (IsOwner)
        {

            virtualCamera.Priority = ownerPriority;

            minimapIconRenderer.color = ownerColor;

            // V2: middle of Cursor circle
            Cursor.SetCursor(crosshair, new Vector2(
                crosshair.width / 2, 
                crosshair.height / 2),
                CursorMode.Auto);
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
