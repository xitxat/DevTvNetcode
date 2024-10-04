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
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Collider2D playerCollider;


    private InputReader inputReader;
    private bool shouldFire;
    private float previousFireTime;
    private float muzzleFlashTimer;

    [Header("Settings")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float fireRate;
    [SerializeField] private float muzzleFlashDuration;


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
        // everyone can see muzflash
        if(muzzleFlashTimer > 0f)
        {
            muzzleFlashTimer -= Time.deltaTime;

            if(muzzleFlashTimer <= 0f)
            {
                muzzleFlash.SetActive(false);
            }
        }

        if (!IsOwner) { return; }
        if (!shouldFire) { return; } // !LMB

        // fire rate cooldown before updating server
        float timeBetweenShots = 1 / fireRate;
        if (Time.time < previousFireTime + timeBetweenShots)
            // Can't fire yet, still in cooldown
            { return; }

            previousFireTime = Time.time;

        //  make visable THIS clients projectiles to all other clients
        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
        //   This client see's its proj
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

    }

    // Visuals
    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;

        GameObject projectileInstance = Instantiate(
            clientProjectilePrefab, 
            spawnPos, 
            Quaternion.identity);

        projectileInstance.transform.up = direction; // same as barrel

        // Dont shoot self
        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        // Move Projectile / access via out. 2D:.up, 3D:forward
        if(projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = rb.transform.up * projectileSpeed;
        }

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

        // Dont shoot self
        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        // Move Projectile / access via out
        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = rb.transform.up * projectileSpeed;
        }

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
