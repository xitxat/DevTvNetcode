using System;
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
    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>("¦|");
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
        if (IsServer)
        {
            Debug.Log("OnNetworkSpawn called on the server");

            UserData userData = null;

            // HOST
            if (IsHost)
            {
                if (HostSingleton.Instance == null || HostSingleton.Instance.GameManager == null || HostSingleton.Instance.GameManager.NetworkServer == null)
                {
                    Debug.LogError("HostSingleton, GameManager, or NetworkServer is null in OnNetworkSpawn on server.");
                    return;
                }

                // Get Players name (Pass in OwnersId & get name) returns User data obj
                userData =
                   HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            }

            // DEDICATED SERVER
            else
            {
                // Enhanced null check with detailed logging for dedicated server
                if (ServerSingleton.Instance == null)
                {
                    Debug.LogError("ServerSingleton.Instance is null in OnNetworkSpawn on server.");
                    return;
                }
                else if (ServerSingleton.Instance.GameManager == null)
                {
                    Debug.LogError("ServerSingleton.Instance.GameManager is null in OnNetworkSpawn on server.");
                    return;
                }
                else if (ServerSingleton.Instance.GameManager.NetworkServer == null)
                {
                    Debug.LogError("ServerSingleton.Instance.GameManager.NetworkServer is null in OnNetworkSpawn on server.");
                    return;
                }
                else
                {
                    // All necessary objects exist, proceed to get user data
                    userData = ServerSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
                }
            }




            // Because we are server, set Value now
            // & trigger network sync
            //PlayerName.Value =  userData.userName;
            //TeamIndex.Value = userData.teamIndex;
            //Debug.Log($"<color=yellow>Server set PlayerName for ClientId: {OwnerClientId} to {userData.userName}</color>");

            if (userData == null)
            {
                Debug.LogError($"UserData for ClientId {OwnerClientId} is null on the server.");
                return;
            }

            // Check if PlayerName or TeamIndex is null before accessing them
            if (PlayerName == null)
            {
                Debug.LogError($"PlayerName NetworkVariable is null for ClientId {OwnerClientId} on the server.");
            }
            else
            {
                try
                {
                    PlayerName.Value = userData.userName;
                    Debug.Log($"<color=yellow>Server set PlayerName for ClientId {OwnerClientId} to {userData.userName}</color>");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Exception setting PlayerName for ClientId {OwnerClientId} on server: {ex.Message}");
                }
            }

            if (TeamIndex == null)
            {
                Debug.LogError($"TeamIndex NetworkVariable is null for ClientId {OwnerClientId} on the server.");
            }
            else
            {
                try
                {
                    TeamIndex.Value = userData.teamIndex;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Exception setting TeamIndex for ClientId {OwnerClientId} on server: {ex.Message}");
                }
            }



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
