using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    // SP list updated by spawnPoints in scene
    // Host creates the SP's. Host initial spawns in at .Zero
public class SpawnPoint : MonoBehaviour
{
    private static List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    // List Access  
    // Static (instance not needed, just ret V3
    public  static Vector3 GetRandomSpawnPos()
    {
        if(spawnPoints.Count == 0)
        {
            return Vector3.zero;
        }

        //  Get a random index between 0 (inclusive) and myList.Count (exclusive)
        //int randomIndex = Random.Range(0, spawnPoints.Count);
        //return  spawnPoints[randomIndex].transform.position;

         return  spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position;
    }

    public void OnEnable()
    {
        spawnPoints.Add(this);
    }

    public void OnDisable()
    {
        spawnPoints.Remove(this);
    }


    // Custom Editor scene view icon
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position, 1);
    }

}
