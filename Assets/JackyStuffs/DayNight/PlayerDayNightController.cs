using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine.Events;

public class PlayerDayNightController : MonoBehaviour
{
    public static PlayerDayNightController Instance;
    public Animator staffAnimator;
    public SpriteRenderer orbSpriteRenderer;
    public Sprite dayOrb, nightOrb;
    public bool isCooldowning;

    public UnityEvent onSwitch;

    private void Awake()
    {
        Instance = this;
    }

    [HideInInspector]public Vector3 topPosition;
    void Update()
    {
        topPosition = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        if ((Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.E) ) && !isCooldowning)
        {
            isCooldowning = true;
            DayNightSwitcher.DayNightEnum dayNightEnum = DayNightSwitcher.Instance.SwitchDayNight();
            if (dayNightEnum == DayNightSwitcher.DayNightEnum.day)
            {
                orbSpriteRenderer.sprite = dayOrb;
            }
            else
            {
                orbSpriteRenderer.sprite = nightOrb;
            }
            if (staffAnimCancelSource != null)
            {
                staffAnimCancelSource.Cancel();
                staffAnimCancelSource.Dispose();
                staffAnimCancelSource = null;
            }
            onSwitch.Invoke();
            staffAnimCancelSource = new CancellationTokenSource();
            PerformStaffAnimation(staffAnimCancelSource.Token);

            WaitCooldown(0.5f);
        }
    }

    private async void WaitCooldown(float seconds)
    {
        orbSpriteRenderer.color = new Color32(255, 255, 255, 50);
        await Task.Delay((int)(seconds * 1000));
        orbSpriteRenderer.color = new Color32(255, 255, 255, 255);
        isCooldowning = false;
    }

    private CancellationTokenSource staffAnimCancelSource = null;
    private async void PerformStaffAnimation(CancellationToken token)
    {
        try
        {
            staffAnimator.SetBool("isWanding", true);
            await Task.Delay((int)(staffAnimator.GetCurrentAnimatorStateInfo(0).length * 1000), token);
            staffAnimator.SetBool("isWanding", false);
        }
        catch (System.OperationCanceledException) when (token.IsCancellationRequested)
        {
            staffAnimator.SetBool("isWanding", false);
            return;
        }
    }

    
}
