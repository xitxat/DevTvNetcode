using System;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour

{
    [Header("References")]
    //[SerializeField] private InputReader inputReader;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject clientProjectilePrefab;
    [SerializeField] private GameObject serverProjectilePrefab;

    private InputReader inputReader;
    private bool shouldFire;

    [Header("Settings")]
    [SerializeField] private float projectileSpeed;


    private void Start()
    {
        inputReader = GameManager.Instance.inputReader;

        if (inputReader == null)
        {
            Debug.LogError("InputReader not found in GameManager.");
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        // Assign inputReader in OnNetworkSpawn to avoid timing issues
        inputReader = GameManager.Instance.inputReader;

        if (inputReader == null)
        {
            Debug.LogError("InputReader not found in GameManager.");
        }
        else
        {
            // Subscribe to input events after network spawn
            inputReader.PrimaryFireEvent += HandlePrimaryFire;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }

        inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }


    void Update()
    {
        if (!IsOwner) { return; }
        if (!shouldFire) { return; } // !LMB

        //  make visable THIS clients projectiles to all other clients
        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
        //   This client see's its proj
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(
            clientProjectilePrefab, 
            spawnPos, 
            Quaternion.identity);

        projectileInstance.transform.up = direction; // same as barrel
     }

    private void HandlePrimaryFire(bool shouldFireInput)
    {
        shouldFire = shouldFireInput;
    }


    // RPC's
    // inform SERVER & swap in server projectile
    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(
            serverProjectilePrefab,
            spawnPos,
            Quaternion.identity);

        projectileInstance.transform.up = direction; // same as barrel

        SpawnDummyProjectileClientRpc( spawnPos,  direction);
    }

    // Update CLIENTs & swap in dummy projectile
    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 direction)
    {
        // Owner already has the dummy obj
        if (IsOwner) { return; }
        
        SpawnDummyProjectile(spawnPos, direction); 

    }



}
