using Unity.Netcode;
using UnityEngine;


//  ADD CALL FROM PROJECTILE LAUNCHER SERVER RPC
//  ADD TO SERVER PROJECTILE PREFAB

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private Projectile projectile;
    [SerializeField] private int damage = 5;




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.attachedRigidbody == null) { return; }

        // If Playing TEAMS
        if(projectile.TeamIndex != -1)
        {
            //  Don't damage self & teammates
            if (collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player))
            {
                if (player.TeamIndex.Value == projectile.TeamIndex) { return; }
            }
        }



        //  Pass on damage value
        if (collision.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(damage);
        }
    }
}
