using UnityEngine;

// Check / Add prefab to Network prefabs LIST Asset
//      (Scene: NetBootstrap > NetworkManager )

//   Spawned via coin wallet

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
