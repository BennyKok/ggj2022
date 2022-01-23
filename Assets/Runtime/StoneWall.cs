using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneWall : MonoBehaviour
{
    private void Start()
    {
        DayNightSwitcher.Instance.SwitchDayNightEvent += IsShow;
    }

    void IsShow(bool isLight)
    {
        gameObject.SetActive(!isLight);
    }

    private void OnDestroy()
    {
        DayNightSwitcher.Instance.SwitchDayNightEvent -= IsShow;
    }
}