using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

public class PlayerDayNightController : MonoBehaviour
{
    public static PlayerDayNightController Instance;
    public Animator staffAnimator;
    public SpriteRenderer orbSpriteRenderer;
    public Sprite dayOrb, nightOrb;
    public bool isCooldowning;

    private void Awake()
    {
        Instance = this;
        DayNightSwitcher.Instance.SwitchToSpecificDayNight(DayNightSwitcher.DayNightEnum.day);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isCooldowning)
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
            staffAnimCancelSource = new CancellationTokenSource();
            PerformStaffAnimation(staffAnimCancelSource.Token);

            WaitCooldown(1.5f);
        }
    }

    private async void WaitCooldown(float seconds)
    {
        await Task.Delay((int)(seconds * 1000));
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
