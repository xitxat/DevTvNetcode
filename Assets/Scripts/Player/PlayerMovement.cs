using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//   Run Checks in OnNetworkSpawn()
//   Run Destroys in OnNetworkDespawn()

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform; // obj with ClientNetworkTransform
    [SerializeField] private Rigidbody2D rb;
    //[SerializeField] private ParticleSystem dustTrail;

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float turningRate = 300f; // per second
    //[SerializeField] private float emissionRate = 10f;

    private ParticleSystem.EmissionModule emissionModule;
    private Vector2 previousMovementInput;
    private Vector3 previousPos;

    private const float ParticleStopThreshhold = 0.005f;

    private void Awake()
    {
        //emissionModule = dustTrail.emission;

        // Ensure bodyTransform is assigned to correct BODY
        if (bodyTransform == null)
        {
            Debug.LogError("bodyTransform is not assigned.");
            return;
        }

        // Check if bodyTransform contains the ClientNetworkTransform component
        ClientNetworkTransform clientNetworkTransform = bodyTransform.GetComponent<ClientNetworkTransform>();

        if (clientNetworkTransform == null)
        {
            Debug.LogError($"{bodyTransform.name} does not contain a ClientNetworkTransform component.");
        }


        // Check if  PREFAB has a Rigidbody2D 
        Rigidbody2D rigidbody2D = bodyTransform.GetComponentInParent<Rigidbody2D>();
        if (rigidbody2D == null)
        {
            Debug.LogError($"{bodyTransform.name} or any of its parents not contain a Rigidbody2D component.");
        }
        else
        {
            // Check if Z-axis rotation is frozen (Freeze Rotation Z axis)
            if ((rigidbody2D.constraints & RigidbodyConstraints2D.FreezeRotation) == 0)
            {
                Debug.LogError($"{bodyTransform.name}parent's Rigidbody2D does not have the Z-axis rotation frozen.");
            }

        }

    }


    public override void OnNetworkSpawn()
    {
        // only owner runs
        if (!IsOwner) { return; }

        inputReader.MoveEvent += HandleMove;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }

        inputReader.MoveEvent -= HandleMove;
    }

    private void Update()
    {
        if (!IsOwner) { return; }

        //  BODY ROTATION (A & D keys)
        //  NEG -turningRate: turn, not move
        float zRotation = previousMovementInput.x * -turningRate * Time.deltaTime;
        bodyTransform.Rotate(0f, 0f, zRotation);
    }

    private void FixedUpdate()
    {
        //   * Time.deltaTime is included in FixedUpdate

        //if ((transform.position - previousPos).sqrMagnitude > ParticleStopThreshhold)
        //{
        //    emissionModule.rateOverTime = emissionRate;
        //}
        //else
        //{
        //    emissionModule.rateOverTime = 0;
        //}

        //previousPos = transform.position;

        if (!IsOwner) { return; }


        //  MOVEMENT (W & S keys)
        // cast to 2d fwd movement (x) . (y) = (A/D keys)
        rb.linearVelocity = (Vector2)bodyTransform.up * previousMovementInput.y * movementSpeed; 
    }

    private void HandleMove(Vector2 movementInput)
    {
        previousMovementInput = movementInput;
    }

}
