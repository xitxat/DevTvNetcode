using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

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


}
