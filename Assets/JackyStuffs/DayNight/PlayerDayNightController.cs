using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDayNightController : MonoBehaviour
{
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            DayNightSwitcher.Instance.SwitchDayNight();
        }
    }
}
