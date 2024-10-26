using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

//          CUSTOMISE .THIS PLAYER
//      NAME
//      CINEMACHINE
//      COLOURS [Team index]
public class TankPlayer : NetworkBehaviour
{
    //  Sync names, Team index[colours]. Cant sync normal strings.  .VALUE
    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>(
        writePerm: NetworkVariableWritePermission.Server,
        readPerm: NetworkVariableReadPermission.Everyone);
    public NetworkVariable<int> TeamIndex = new NetworkVariable<int>(0);


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



    // Server only invocation
    public static event Action<TankPlayer> OnPlayerSpawned;
    public static event Action<TankPlayer> OnPlayerDespawned;

    public override void OnNetworkSpawn()
    {
        Debug.Log($"<color=teal>[OnNetworkSpawn] ClientId: {OwnerClientId}, IsServer: {IsServer}, IsOwner: {IsOwner}</color>");

        if (IsServer)
        {
            Debug.Log($"<color=teal>[Server] OnNetworkSpawn for player with ClientId: {OwnerClientId}</color>");
            
            UserData userData = null;

            // HOST
            if (IsHost)
            {
                // Get Players name (Pass in OwnersId & get name) returns User data obj
                userData =
                   HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            }

            // DEDICATED SERVER
            else
            {
                Debug.Log("<TankPlayer> Running as Dedicated Server, getting UserData from ServerSingleton.");

                // All necessary objects exist, proceed to get user data
                userData = ServerSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            }

            // Because we are server, set Value now
            // & trigger network sync
            if (userData != null)
            {
                PlayerName.Value = userData.userName;
                TeamIndex.Value = userData.teamIndex;
                Debug.LogWarning($"<TankPlayer>[Server] Set PlayerName: {userData.userName} for ClientId: {OwnerClientId}");
            }
            else
            {
                Debug.LogError($"<TankPlayer>[Server] Failed to get UserData for ClientId: {OwnerClientId}");
            }

            // We don't call OnPlayerSpawned immediately; instead, let it be triggered by the OnValueChanged event

            // Immediately invoke the OnPlayerSpawned event after setting PlayerName and TeamIndex
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

            //Debug.LogWarning($"<color=teal>[Owner] Player is owner, ClientId: {OwnerClientId}. Camera and crosshair set.</color>");
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
