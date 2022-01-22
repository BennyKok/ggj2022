using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class DayNightSwitcher : MonoBehaviour
{
    public static DayNightSwitcher Instance;
    public enum DayNightEnum { day, night };
    public DayNightEnum currentDayNight;

    public UnityEvent DayNightSwitchEvent;

    private Camera cam;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentDayNight = DayNightEnum.day;
        cam = Camera.main;
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

        DayNightSwitchEvent?.Invoke();

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
