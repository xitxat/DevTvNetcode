using System;
using TMPro;
using UnityEngine;

public class PlayerColourDisplay : MonoBehaviour
{
    [SerializeField] private TeamColourLookup teamColourLookup;
    [SerializeField] private TankPlayer player;
    // Treads Body, Turret
    [SerializeField] private SpriteRenderer[] playerSprites;

    void Start()
    {
        // Force Update
        HandleTeamChanged(-1, player.TeamIndex.Value);

        player.TeamIndex.OnValueChanged += HandleTeamChanged;
    }

    void OnDestroy()
    {
        player.TeamIndex.OnValueChanged -= HandleTeamChanged;

    }

    private void HandleTeamChanged(int oldTeamIndex, int newTeamIndex)
    {
        Color teamColour = teamColourLookup.GetTeamColour(newTeamIndex);

        foreach(SpriteRenderer sprite in playerSprites)
        {
            sprite.color = teamColour;
        }
    }

}
