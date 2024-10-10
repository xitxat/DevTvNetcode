using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

//          CUSTOMISE .THIS PLAYER
//      CINEMACHINE
public class TankPlayer : NetworkBehaviour
{

    [Header("Refs")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;


    [Header("Settings")]
    [SerializeField] private int ownerPriority = 15;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            virtualCamera.Priority = ownerPriority;
        }
    }
}
