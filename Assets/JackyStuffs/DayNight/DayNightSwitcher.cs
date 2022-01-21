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

    public Skybox day, night;

    public UnityEvent DayNightSwitchEvent;

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
            RenderSettings.skybox = night.material;
            currentDayNight = DayNightEnum.night;
        }
        else
        {
            RenderSettings.skybox = day.material;
            currentDayNight = DayNightEnum.day;
        }

        DayNightSwitchEvent?.Invoke();

        return currentDayNight;
    }

    public void SwitchDayNightSpecific(DayNightEnum dayNight)
    {
        if (currentDayNight == DayNightEnum.day)
        {
            RenderSettings.skybox = night.material;
            currentDayNight = DayNightEnum.night;
        }
        else
        {
            RenderSettings.skybox = day.material;
            currentDayNight = DayNightEnum.day;
        }

        DayNightSwitchEvent?.Invoke();
        currentDayNight = dayNight;
    }

}
