using UnityEngine;

// Store ID of team that shot projectile
// Friendly fire senerios.
// Assign in ProjectileLauncher script
public class Projectile : MonoBehaviour
{

    public int TeamIndex {get; private set;}


public  void Initialise(int teamIndex)
    {
        TeamIndex = teamIndex;
    }
}
