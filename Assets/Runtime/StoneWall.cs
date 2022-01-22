using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneWall : MonoBehaviour
{
    private void Start()
    {
        DayNightSwitcher.Instance.SwitchDayNightEvent += isShow;
    }

    void isShow()
    {
        if (DayNightSwitcher.Instance.currentDayNight == DayNightSwitcher.DayNightEnum.day)
            gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        DayNightSwitcher.Instance.SwitchDayNightEvent -= isShow;
    }
}