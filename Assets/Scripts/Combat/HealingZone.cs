using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


// https://www.gamedev.tv/dashboard/courses/22 4.14 , 4.15

public class HealingZone : NetworkBehaviour

{

    [Header("Refs")]
    [SerializeField] private Image healPowerBar;

    [Header("Settings")]
    [SerializeField] int maxHealPower = 30;
    [SerializeField] float healCoolDown = 30f;
    [SerializeField] float healTickRate = 1f;
    [SerializeField] int coinsPerTick = 10;
    [SerializeField] int healthPerTick = 10;

    // Power of the healing bar. Clients Un/Sub to HealPower
    private NetworkVariable<int> HealPower = new NetworkVariable<int>();


    private List<TankPlayer> playersInZone = new List<TankPlayer>();


    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged += HandleHealPowerChanged;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged -= HandleHealPowerChanged;

            // On server connect, mats may not have full charge.
            // manual reset to update UI to Initial value
            HandleHealPowerChanged(0, HealPower.Value);
        }


    }



    #region LIST
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) { return; }

        // Get Player that Collided with Health mat
        // Bypass null check 
        // TryGetComponent checks ROOT
        // Collider not on root (tank treads)
        // Access via rb on root
        if (!collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) { return; }

        playersInZone.Add(player);

       // Debug.Log($"<color=green>On Health mat: {player.PlayerName.Value}</color>");

    }    
    
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsServer) { return; }

        if (!collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) { return; }

        playersInZone.Remove(player);

       // Debug.Log($"<color=green>Left Health mat: {player.PlayerName.Value}</color>");

    }
    #endregion


    private void HandleHealPowerChanged(int oldHealPower, int newHealPower)
    {
        // Update (float) Fill amt of hp bar
        // only need to cast 1 of the ints
        healPowerBar.fillAmount = (float)newHealPower / maxHealPower;
    }
}
