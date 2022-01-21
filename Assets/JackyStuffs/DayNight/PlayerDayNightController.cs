using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class PlayerDayNightController : MonoBehaviour
{
    public static PlayerDayNightController Instance;
    public bool isCooldowning;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isCooldowning)
        {
            isCooldowning = true;
            WaitCooldown(3);
            DayNightSwitcher.Instance.SwitchDayNight();
        }
    }

    private async void WaitCooldown(float seconds)
    {
        await Task.Delay((int)(seconds * 1000));
        isCooldowning = false;
    }
}
