using UnityEngine;

// Check / Add prefab to Network prefabs LIST Asset
//      (Scene: NetBootstrap > NetworkManager )

//   Spawned via coin wallet
//   Bounty Coins spawned around dead  Player
//      restrict out of Bounds bCoins: increase arena wall collider size.

public class BountyCoin : Coin
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

        Destroy(gameObject);

        return coinValue;
    }
}
