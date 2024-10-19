using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// .main  : the top module of PS 

[RequireComponent(typeof(ParticleSystem))]
public class ParticleAligner : MonoBehaviour
{
    private ParticleSystem.MainModule psMain;

    private void Start()
    {
        psMain = GetComponent<ParticleSystem>().main;
    }

    private void Update()
    {
        // convert to radians
        psMain.startRotation = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
    }
}
