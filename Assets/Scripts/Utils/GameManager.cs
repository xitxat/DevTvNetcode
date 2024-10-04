using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Input Settings")]
    public InputReader inputReader; 

    private void Awake()
    {
        // Implement Singleton pattern for global access
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps GameManager alive between scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one GameManager instance
        }
    }
}
