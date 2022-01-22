using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DayNightSwitcher : MonoBehaviour
{
    public static DayNightSwitcher Instance;
    public enum DayNightEnum { day, night };
    [System.NonSerialized]public DayNightEnum currentDayNight;
    private Camera cam;

    public delegate void SwitchDayNightDelegate();
    public event SwitchDayNightDelegate SwitchDayNightEvent;

    private void Awake()
    {
        cam = Camera.main;
        Instance = this;

        SwitchToSpecificDayNight(DayNightEnum.day);
    }

    public DayNightEnum SwitchDayNight()
    {
        if (currentDayNight == DayNightEnum.day)
        {
            cam.backgroundColor = new Color32(20, 17, 51, 255);
            currentDayNight = DayNightEnum.night;
        }
        else
        {
            cam.backgroundColor = new Color32(130, 160, 210, 255);
            currentDayNight = DayNightEnum.day;
        }

        SwitchDayNightEvent?.Invoke();

        return currentDayNight;
    }

    public void SwitchToSpecificDayNight(DayNightEnum dayNight)
    {
        if (dayNight == DayNightEnum.day)
        {
            currentDayNight = DayNightEnum.night;
        }
        else
        {
            currentDayNight = DayNightEnum.day;
        }
        SwitchDayNight();
    }
}
