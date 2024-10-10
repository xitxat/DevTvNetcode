using Unity.Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

//          CUSTOMISE .THIS PLAYER
//      CINEMACHINE
public class TankPlayer : NetworkBehaviour
{

    [Header("Refs")]
    [SerializeField] private CinemachineCamera virtualCamera;


    [Header("Settings")]
    [SerializeField] private int ownerPriority = 15;

    //  Sync names. Cant sync normal strings.
    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

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


        }

        if (IsOwner)
        {
            virtualCamera.Priority = ownerPriority;
        }
    }
}
