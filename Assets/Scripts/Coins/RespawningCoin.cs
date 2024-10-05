using UnityEngine;

public class RespawningCoin : Coin
{
    public override int Collect()
    {
        if (!IsServer) 
        {
            Show(false);
            return 0; 
        }

        if (isAlreadyCollected) { return 0; }

        isAlreadyCollected = true;

        return coinValue;
    }


}
