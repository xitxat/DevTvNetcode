using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawningCoin : Coin
{
    // Coin event: once collected, respawn triggered from coin listener
    public event Action<RespawningCoin> OnCollected;

    private Vector3 previousPosition;

    private void Update()
    {
        if (previousPosition != transform.position)
        {
            Show(true);
        }

        previousPosition = transform.position;
    }

    public override int Collect()
    {
        if (!IsServer)
        {
            Show(false);
            return 0;
        }

        if (isAlreadyCollected) { return 0; }

        isAlreadyCollected = true;

        //  Broadcast
        OnCollected?.Invoke(this);

        return coinValue;
    }

    public void Reset()
    {
        isAlreadyCollected = false;
    }
}
