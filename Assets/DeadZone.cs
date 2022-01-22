using System;
using System.Collections;
using System.Collections.Generic;
using BennyKok.Bootstrap;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    public GameObject player;


    public Transform respawn;

    void Start()
    {
        respawn = Bootstrap.Instance.transform;
        player = GameObject.FindWithTag("Player");
    }

    private void OnCollisionEnter(Collision collision)
    {
        player.transform.position = respawn.position;
    }
}