using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DayNightSwitcher : MonoBehaviour
{
    public delegate void SwitchDayNightDelegate();
    public SwitchDayNightDelegate SwitchDatNightEvent;

    public static DayNightSwitcher Instance;

    public enum DayNightEnum { day, night };
    public DayNightEnum currentDayNight;

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
    }

    public DayNightEnum SwitchDayNight()
    {
        if (currentDayNight == DayNightEnum.day)
        {
            currentDayNight = DayNightEnum.night;
        }
        else
        {
            currentDayNight = DayNightEnum.day;
        }

        SwitchDatNightEvent?.Invoke();

        return currentDayNight;
    }

    public void SwitchDayNightSpecific(DayNightEnum dayNight)
    {
        SwitchDatNightEvent?.Invoke();
        currentDayNight = dayNight;
    }
}
