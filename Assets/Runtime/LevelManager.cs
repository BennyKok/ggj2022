using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private void Start()
    {
        CoreRef.Instance.playerEntity.onDestroyEvent.AddListener(() =>
        {
            if (CoreRef.Instance.playerEntity.hp <= 0)
            {
                // CoreRef.Instance.playerEntity.ResetHealth();
            }
        });
    }
}