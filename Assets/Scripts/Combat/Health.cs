using System;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    // field: allows private property to be visible in inspct.
    [field: SerializeField] public int MaxHealth { get; private set; } = 100;

    //  Because it is a class, it must be instantiated in order to work correctly. default val () 0.
    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>();

    // dont get hit when dead & waiting for update
    private bool isDead;

    // when we die, identify self
    // this Health class
    public Action<Health> OnDie; 


    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        // Network vars accesed via .Value
        CurrentHealth.Value = MaxHealth;
    }

    public void TakeDamage(int damageValue)
    {
        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        ModifyHealth(healValue);

    }

    private void ModifyHealth(int value)
    {
        if(isDead) { return; }

        // if value is -ive it will subtract

        int newHealth = CurrentHealth.Value + value;
        CurrentHealth.Value = Mathf.Clamp(newHealth, 0, MaxHealth);

        if(CurrentHealth.Value == 0)
        {
            // pass in ourself
            // OnDie? if someone has subscribed to it
            OnDie?.Invoke(this); //  broadcast event to all  methods / subscribers
            isDead = true;
            Debug.Log("<color=orange>   DEAD</color>");
        }
    }

}
