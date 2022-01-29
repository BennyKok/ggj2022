using System;
using System.Collections;
using System.Collections.Generic;
using BennyKok.Bootstrap;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadZone : MonoBehaviour
{
    public GameObject player;


    public Transform respawn;

    void Start()
    {
        respawn = GameObject.Find("LevelInitializer").transform.GetChild(0).transform;
        player = GameObject.FindWithTag("Player");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (gameObject.tag == "Owl" && DayNightSwitcher.Instance.currentDayNight == DayNightSwitcher.DayNightEnum.day) return;
        // player.transform.position = respawn.position;

        CoreRef.Instance.playerEntity.OnDamage(1);

        // if (CoreRef.Instance.playerEntity.hp == 0)
        // {
        // reload unity scene
        Debug.Log("hey");
        // SceneManager.LoadScene(gameObject.scene.name);
        
        DeathScreen.Instance.gameObject.SetActive(true);
        DeathScreen.Instance.OK(gameObject);
        // }
    }
}