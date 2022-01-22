using System.Collections;
using System.Collections.Generic;
using BennyKok.Bootstrap;
using BennyKok.CombatSystem;
using UnityEngine;

public class CoreRef : Singleton<CoreRef>
{
    public GameObject player;
    public CombatEntity playerEntity;
    public bool enemy;
    public override void OnAwake()
    {
        
    }
}
