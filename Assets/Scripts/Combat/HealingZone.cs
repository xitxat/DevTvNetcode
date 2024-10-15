using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

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

    private List<TankPlayer> playersInZone = new List<TankPlayer>();

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) { return; }

        // Get Player that Collided with Health mat
        // Bypass null check 
        if(!collision.TryGetComponent<TankPlayer>(out TankPlayer player)) { return; }

        playersInZone.Add(player);

        Debug.Log($"<color=green>On Health mat: {player.PlayerName.Value}</color>");

    }    
    
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsServer) { return; }

        if (!collision.TryGetComponent<TankPlayer>(out TankPlayer player)) { return; }

        playersInZone.Remove(player);
    }


}
