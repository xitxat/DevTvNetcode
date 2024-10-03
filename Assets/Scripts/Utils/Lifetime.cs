using UnityEngine;

public class Lifetime : MonoBehaviour
{

    [SerializeField] private float lifetime = 1f;

    void Start()
    {
        Destroy(gameObject, lifetime);

    }

}
