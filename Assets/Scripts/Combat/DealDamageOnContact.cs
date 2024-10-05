using Unity.Netcode;
using UnityEngine;

//  ADD CALL FROM PROJECTILE LAUNCHER SERVER RPC
//  ADD TO SERVER PROJECTILE PREFAB
public class DealDamageOnContact : MonoBehaviour
{

    [SerializeField] private int damage = 5;


    private ulong ownerClientId;

    //  Currently we own: Ourself and Projectiles
    public void SetOwner(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.attachedRigidbody == null) { return; }

        //  Don't damage self via ClientId ulong
        if (collision.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject netObj))
        {
            if (ownerClientId == netObj.OwnerClientId) { return; }
        }

        //  Pass on damage value
        if (collision.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(damage);
        }
    }
}
