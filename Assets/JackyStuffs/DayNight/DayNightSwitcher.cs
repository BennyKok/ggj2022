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
    private GameObject dayNightRotator;
    public delegate void SwitchDayNightDelegate(bool isDay);

    public event SwitchDayNightDelegate SwitchDayNightEvent;

    private void Awake()
    {
        dayNightRotator = GameObject.Find("DayNightRotator");
        Instance = this;

        SwitchToSpecificDayNight(DayNightEnum.day);
        bottomRightPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
        bottomLeftPosition = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        middlePosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));

    }

    private Vector3 bottomRightPosition, bottomLeftPosition, middlePosition;
    public DayNightEnum SwitchDayNight()
    {
        if (currentDayNight == DayNightEnum.day)
        {

            //ChangeSkyGradient(nightBackground, new Color32(255, 255, 255, 255));
            RotateSky();
            currentDayNight = DayNightEnum.night;
            SunAndMoon(-200, 100);
        }
        else
        {
            //ChangeSkyGradient(nightBackground, new Color32(255, 255, 255, 0));
            RotateSky();
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

    public void ChangeSkyGradient(SpriteRenderer rend, Color32 color)
    {
        if (skyGradientCancelSource != null)
        {
            skyGradientCancelSource.Cancel();
            skyGradientCancelSource.Dispose();
            skyGradientCancelSource = null;
        }

        skyGradientCancelSource = new CancellationTokenSource();
        SkyGradient(rend, color, skyGradientCancelSource.Token);
    }

    public void SunAndMoon(int a, int b)
    {
        sun.transform.DOKill();
        sun.transform.DOMoveY(Screen.height - a, 2f);
        moon.transform.DOKill();
        moon.transform.DOMoveY(Screen.height - b, 2f);
    }

    private async void SkyGradient(SpriteRenderer rend, Color32 color, CancellationToken token)
    {
        byte r = (byte) (rend.color.r * 255);
        byte g = (byte) (rend.color.g * 255);
        byte b = (byte) (rend.color.b * 255);
        byte a = (byte) (rend.color.a * 255);
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

            rend.color = new Color32(r, g, b, a);
            await Task.Yield();
        }
    }

    private void RotateSky()
    {
        if (rotateSkyCancelSource != null)
        {
            rotateSkyCancelSource.Cancel();
            rotateSkyCancelSource.Dispose();
            rotateSkyCancelSource = null;
        }
        rotateSkyCancelSource = new CancellationTokenSource();
        RotateSkyAsync(rotateSkyCancelSource.Token);
    }

    private CancellationTokenSource rotateSkyCancelSource;
    private async void RotateSkyAsync(CancellationToken token)
    {
        for(int i = 0; i < 36; i++)
        {
            if (token.IsCancellationRequested)
            {
                if (currentDayNight == DayNightEnum.day)
                {
                    dayNightRotator.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    dayNightRotator.transform.rotation = Quaternion.Euler(0, 0, 180);
                }
                return;
            }
            dayNightRotator.transform.Rotate(Vector3.forward * 5);
            await Task.Delay(10, token);
        }
        if (currentDayNight == DayNightEnum.day)
        {
            dayNightRotator.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            dayNightRotator.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
    }
}