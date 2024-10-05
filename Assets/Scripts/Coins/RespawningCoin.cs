using System;
using UnityEngine;

public class RespawningCoin : Coin
{

    //  Event called OnCollected
    public event Action<RespawningCoin> OnCollected;

    private Vector3 previousPosition;

    public void Update()
    {
        //  If coin has moved
        if(previousPosition != transform.position)
        {
            // Render
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

        OnCollected?.Invoke(this);

        return coinValue;
    }

    public void Reset()
    {
        isAlreadyCollected = false;
    }
}
