using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{

    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private Image healthBarImage;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        //  sub
        health.CurrentHealth.OnValueChanged += HandleHealthChanged;
        //  manual initialization to register setting maxH on Start
        HandleHealthChanged(0, health.CurrentHealth.Value);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }

        //  unsub 
        health.CurrentHealth.OnValueChanged -= HandleHealthChanged;
    }

    // receive penultimate / ultimate values
    private void HandleHealthChanged(int oldHealth, int newHealth)
    {
        //  get % and cast it
        healthBarImage.fillAmount = (float)newHealth / health.MaxHealth;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
