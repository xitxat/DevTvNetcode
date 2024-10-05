using Unity.Netcode;
using UnityEngine;

//   ABSTRACT : ONLY DERIVITIVES INSTANTIATED
public abstract class Coin : NetworkBehaviour
{

    //  To hide client side immidently upon collision
    [SerializeField] private SpriteRenderer spriteRenderer;

    //  Inheritable / semi public
    protected int coinValue = 10;
    protected bool isAlreadyCollected;


    //  abstract forces implimentation of method on inheritance
    //  RespawningCoin
    public abstract int Collect();

    public void SetValue(int value)
    {
        coinValue = value;
    }

    //  Visibility
    protected void Show(bool show)
    {
        spriteRenderer.enabled = show;
    }

}
