using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneWall : DayNightComponent
{
    protected override void OnDayNightSwitch(bool isLight)
    {
        gameObject.SetActive(isLight);
    }

    protected override void Start()
    {
        // your own start method
        base.Start();
    }

    protected override void OnDestroy()
    {
        // you own destroy method
        base.OnDestroy();
    }
}