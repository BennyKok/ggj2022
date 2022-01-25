using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Threading;
using DG.Tweening;

public class DayNightSwitcher : MonoBehaviour
{
    public static DayNightSwitcher Instance;
    public GameObject sun;
    public GameObject moon;

    public enum DayNightEnum
    {
        day,
        night
    };

    [System.NonSerialized] public DayNightEnum currentDayNight;
    private SpriteRenderer paperBackground;

    public delegate void SwitchDayNightDelegate(bool isDay);

    public event SwitchDayNightDelegate SwitchDayNightEvent;

    private void Awake()
    {
        paperBackground = GameObject.Find("PaperBackground").GetComponent<SpriteRenderer>();
        Instance = this;

        SwitchToSpecificDayNight(DayNightEnum.day);
    }

    public DayNightEnum SwitchDayNight()
    {
        if (currentDayNight == DayNightEnum.day)
        {
            ChangeSkyGradient(new Color32(65, 80, 100, 255));
            currentDayNight = DayNightEnum.night;
            SunAndMoon(-200, 100);
        }
        else
        {
            ChangeSkyGradient(new Color32(255, 255, 255, 255));
            currentDayNight = DayNightEnum.day;
            SunAndMoon(100, -200);
        }

        SwitchDayNightEvent?.Invoke(currentDayNight == DayNightEnum.day);

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

    private CancellationTokenSource skyGradientCancelSource = null;

    public void ChangeSkyGradient(Color32 color)
    {
        if (skyGradientCancelSource != null)
        {
            skyGradientCancelSource.Cancel();
            skyGradientCancelSource.Dispose();
            skyGradientCancelSource = null;
        }

        skyGradientCancelSource = new CancellationTokenSource();
        SkyGradient(color, skyGradientCancelSource.Token);
    }

    public void SunAndMoon(int a, int b)
    {
        sun.transform.DOKill();
        sun.transform.DOMoveY(Screen.height - a, 2f);
        moon.transform.DOKill();
        moon.transform.DOMoveY(Screen.height - b, 2f);
    }

    private async void SkyGradient(Color32 color, CancellationToken token)
    {
        byte r = (byte) (paperBackground.color.r * 255);
        byte g = (byte) (paperBackground.color.g * 255);
        byte b = (byte) (paperBackground.color.b * 255);
        byte a = (byte) (paperBackground.color.a * 255);
        while (true)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            if (r > color.r)
            {
                r -= 1;
            }
            else if (r < color.r)
            {
                r += 1;
            }

            if (g > color.g)
            {
                g -= 1;
            }
            else if (g < color.a)
            {
                g += 1;
            }

            if (b > color.b)
            {
                b -= 1;
            }
            else if (b < color.b)
            {
                b += 1;
            }

            if (a > color.a)
            {
                a -= 1;
            }
            else if (a < color.a)
            {
                a += 1;
            }

            if (r == color.r && g == color.g && b == color.b && a == color.a)
                return;

            paperBackground.color = new Color32(r, g, b, a);
            await Task.Yield();
        }
    }
}