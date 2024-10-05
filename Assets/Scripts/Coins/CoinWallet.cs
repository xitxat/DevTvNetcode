using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{


    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    private int coinVal;


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Coin>(out Coin coin)) { return; }

        //  if client, hide coin renderer
            coinVal = coin.Collect();
        
        if (!IsServer) { return; }

        if (IsServer)
        {
           TotalCoins.Value += coinVal;
        }


    }
}
