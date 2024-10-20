using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{

    [SerializeField] private GameObject prefab;

    private void OnDestroy()
    {
        // prevent objects spilling over into Memu from Game
        if (!gameObject.scene.isLoaded) { return; }

        Instantiate(prefab, transform.position, Quaternion.identity);
    }
}
