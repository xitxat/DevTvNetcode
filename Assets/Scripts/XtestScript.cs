using System;
using UnityEngine;

public class XtestScript : MonoBehaviour
{

    [SerializeField] private InputReader inputReader;

    void Start()
    {
        inputReader.MoveEvent += HandleMove;
    }

    private void OnDestroy()
    {
        inputReader.MoveEvent -= HandleMove;
        
    }

    private void HandleMove(Vector2 vector)
    {
        Debug.Log(vector);
    }

    void Update()
    {
        
    }
}
