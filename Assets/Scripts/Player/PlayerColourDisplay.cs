using System;
using TMPro;
using UnityEngine;

public class PlayerColourDisplay : MonoBehaviour
{
    [SerializeField] private TeamColourLookup teamColourLookup;
    [SerializeField] private TankPlayer player;
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

    private void HandleTeamChanged(int previousValue, int newValue)
    {
        throw new NotImplementedException();
    }
}
