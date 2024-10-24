using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProjectileLauncher : NetworkBehaviour

{
    [Header("References")]
    [SerializeField] private TankPlayer player; // ref team index
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CoinWallet wallet;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject clientProjectilePrefab;
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Collider2D playerCollider;


    //private InputReader inputReader;
    //private CoinWallet wallet;
    private bool shouldFire;

    // Turn off UI element's Raycast target. to allow click thru
    // + Leaderboard entities prefab[TMP extra settings]
    private bool isPointerOverUI; 

    private float shotTimer;
    private float muzzleFlashTimer;


    [Header("Settings")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float fireRate;
    [SerializeField] private float muzzleFlashDuration;
    [SerializeField] private int costToFire; // Spend coins to fire

    private float timeBetweenShots; // Fire Rate cooldown timer


    private void Start()
    {
        timeBetweenShots = 1 / fireRate;

    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        inputReader.PrimaryFireEvent += HandlePrimaryFire;
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

        // Clicking "Exit Arena" button
        isPointerOverUI = EventSystem.current.IsPointerOverGameObject();

        if (shotTimer > 0) {shotTimer -= Time.deltaTime;}

        if (!shouldFire) { return; } // !LMB

        if (shotTimer > 0) { return; }

        // Have Anno?
        if(wallet.TotalCoins.Value < costToFire) { return; }


        //  FIRE
        //  make visable THIS clients projectiles to all other clients
        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
        //   This client see's its proj
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up, player.TeamIndex.Value);

        //  Reset Fire rate
        shotTimer = timeBetweenShots;

    }


    // INPUT SHOOT
    private void HandlePrimaryFire(bool shouldFireInput)
    {
        // LMB
        if (shouldFireInput) 
        {
            if (isPointerOverUI) { return; }
        }

        shouldFire = shouldFireInput;
    }


    // RPC's
    // inform SERVER & swap in server projectile
    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
    {
        // Have Anno?
        if (wallet.TotalCoins.Value < costToFire) { return; }
        wallet.SpendCoins(costToFire);


        GameObject projectileInstance = Instantiate(
            serverProjectilePrefab,
            spawnPos,
            Quaternion.identity);

        projectileInstance.transform.up = direction; // same as barrel


        // Dont shoot self
        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());


        //  FRIENDLY FIRE
        if (projectileInstance.TryGetComponent<Projectile>(out Projectile projectile))
        {
            // set owner
            projectile.Initialise(player.TeamIndex.Value);
        }


        ////  DEAL DAMAGE
        //if (projectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamage))
        //{
        //    // this scripts owner Id / ie; Parent
        //    dealDamage.SetOwner(OwnerClientId);
        //}


        // MOVE PROJECTILE / access via out
        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = rb.transform.up * projectileSpeed;
        }

        SpawnDummyProjectileClientRpc( spawnPos,  direction, player.TeamIndex.Value);
    }

    // Update CLIENTs & swap in dummy projectile
    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 direction, int teamIndex)
    {
        // Owner already has the dummy obj
        if (IsOwner) { return; }
        
        SpawnDummyProjectile(spawnPos, direction, teamIndex); 

    }

    // VISUALS
    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction, int teamIndex)
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

        //  FRIENDLY FIRE
        if (projectileInstance.TryGetComponent<Projectile>(out Projectile projectile))
        {
            // set owner
            projectile.Initialise(teamIndex);
        }

        // Move Projectile / access via out. 2D:.up, 3D:forward
        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = rb.transform.up * projectileSpeed;
        }

    }

}
