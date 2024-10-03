using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{

    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform turretTransform; // turret pivot


    // INTERPOLATED MOVEMENT: USE:
    private void LateUpdate()
    {
        // move tank first then aim. reduced jitter, snap barrel to cursor

        if (!IsOwner) { return; }

        Vector2 aimScreenPosition = inputReader.AimPosition; // pixel coordinates
        Vector2 aimWorldPosition =  Camera.main.ScreenToWorldPoint(aimScreenPosition); // convert into world space coordinates

        // Turret face cursor
        turretTransform.up = new Vector2(
            aimWorldPosition.x - turretTransform.position.x,
            aimWorldPosition.y - turretTransform.position.y);

    }

}
