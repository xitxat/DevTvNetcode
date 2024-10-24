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
    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>(new FixedString32Bytes(""));
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
                // All necessary objects exist, proceed to get user data
                userData = ServerSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            }

            // Because we are server, set Value now
            // & trigger network sync
            PlayerName.Value = userData.userName;
            TeamIndex.Value = userData.teamIndex;
            Debug.Log($"<color=yellow>Server set PlayerName for ClientId: {OwnerClientId} to {userData.userName}</color>");

            // Start coroutine to ensure player data is fully synchronized before using it
            StartCoroutine(WaitForSync());

                //moved into coroutine
                //OnPlayerSpawned?.Invoke(this);

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

    private IEnumerator WaitForSync()
    {
        int retries = 0;
        int maxRetries = 100; // 10 secs
        float retryDelay = 0.1f; //  (100 ms)

        // Wait until PlayerName is not empty and properly synced or retry limit is reached
        while (string.IsNullOrEmpty(PlayerName.Value.ToString()) && retries < maxRetries)
        {
            Debug.Log($"<color=yellow>Attempt {retries + 1}/{maxRetries}: Waiting for PlayerName to sync...</color>");

            retries++;
            yield return new WaitForSeconds(retryDelay); // Wait 100 ms before checking again
        }

        if (!string.IsNullOrEmpty(PlayerName.Value.ToString()))
        {
            Debug.Log($"Player name synced: {PlayerName.Value}");
            // Now that sync is complete, invoke the OnPlayerSpawned event
            OnPlayerSpawned?.Invoke(this);
        }
        else
        {
            Debug.LogError("Failed to sync PlayerName within the retry limit.");
            // Handle the failure case, e.g., notify the player, retry connection, etc.
        }
    }



}
