using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


// https://www.gamedev.tv/dashboard/courses/22 4.14 , 4.15
// Costs coins to Heal

public class HealingZone : NetworkBehaviour

{

    [Header("Refs")]
    [SerializeField] private Image healPowerBar;

    [Header("Settings")]
    [SerializeField] int maxHealPower = 30;
    [SerializeField] float healCoolDown = 30f; // Seconds
    // How long till next tick rate  // use (1/ hTR) to convert to seconds
    [SerializeField] float healTickRate = 1f;
    [SerializeField] int coinsPerTick = 10;
    [SerializeField] int healthPerTick = 10;

    private float remainingCooldown;
    private float tickTimer;    // tickRate Update()

    // Power of the healing bar. Clients Un/Sub to HealPower
    private NetworkVariable<int> HealPower = new NetworkVariable<int>();

    private List<TankPlayer> playersInZone = new List<TankPlayer>();


    #region SPAWN
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
    #endregion


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

        Debug.Log($"<color=green>On Health mat: {player.PlayerName.Value}</color>");

    }    
    
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsServer) { return; }

        if (!collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) { return; }

        playersInZone.Remove(player);

      Debug.Log($"<color=green>Left Health mat: {player.PlayerName.Value}</color>");

    }
    #endregion

    //  Heal Power Transfer
    private void Update()
    {
        if (!IsServer) { return; }

        // HP Mat cooldown
        if(remainingCooldown > 0f)
        {
            remainingCooldown -= Time.deltaTime;

            if(remainingCooldown < 0f)
            {
                HealPower.Value = maxHealPower;
            }
            else
            {
                return;
            }
        }

        tickTimer += Time.deltaTime;
        // Ticks per second
        {
            foreach(TankPlayer player in playersInZone)
            {
                // Stop if out of power
                if(HealPower.Value == 0) { break; }

                // Stop Mat actions if Player health == max HP
                if(player.Health.CurrentHealth.Value == player.Health.MaxHealth)
                // Go to next player in List
                { continue; }

                // Player No coins == No Heal
                if(player.Wallet.TotalCoins.Value < coinsPerTick) { continue; }

                // Calc Cost & give HP
                player.Wallet.SpendCoins(coinsPerTick);
                player.Health.RestoreHealth(healthPerTick);

                // Update HP Mat
                HealPower.Value -= 1;

                if (HealPower.Value == 0)
                {
                    // Don't heal players & enter coolDow2n
                    remainingCooldown = healCoolDown;

                    // TODO cloak player, despawn Mat
                }
            }

            // DONE tick, reset timer 
            // ISSUE reset via Time.deltaTime maynot be exactly "0"
            // Use Modulo for accuracy
            tickTimer = tickTimer % (1 / healTickRate);

        }

    }

    private void HandleHealPowerChanged(int oldHealPower, int newHealPower)
    {
        // Update (float) Fill amt of hp bar
        // only need to cast 1 of the ints
        healPowerBar.fillAmount = (float)newHealPower / maxHealPower;
    }
}
