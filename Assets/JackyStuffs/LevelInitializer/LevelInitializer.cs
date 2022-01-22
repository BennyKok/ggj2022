using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder (1)]
public class LevelInitializer : MonoBehaviour
{
    public DayNightSwitcher.DayNightEnum initialDayNight;

    private void Start()
    {
        DayNightSwitcher.Instance.SwitchToSpecificDayNight(initialDayNight);
    }
}
