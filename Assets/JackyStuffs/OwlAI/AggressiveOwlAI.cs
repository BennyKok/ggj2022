using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

public class AggressiveOwlAI : OwlAI
{
    public GameObject warningPrefab;
    public float detectRange;
    private GameObject player;
    protected override void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void OnDayNightSwitch(bool isDay)
    {
        base.OnDayNightSwitch(isDay);

        if (detectPlayerCancelSource != null)
        {
            detectPlayerCancelSource.Cancel();
            detectPlayerCancelSource.Dispose();
            detectPlayerCancelSource = null;
        }
        if (chasePlayerCancelSource != null)
        {
            chasePlayerCancelSource.Cancel();
            chasePlayerCancelSource.Dispose();
            chasePlayerCancelSource = null;
        }
        if (warningSignHolder != null)
        {
            Destroy(warningSignHolder);
            warningSignHolder = null;
        }
        hasDetectPlayer = false;

        if (!isDay)
        {
            detectPlayerCancelSource = new CancellationTokenSource();
            DetectPlayer(detectPlayerCancelSource.Token);
        }
    }

    private CancellationTokenSource detectPlayerCancelSource;
    private GameObject warningSignHolder;
    private bool hasDetectPlayer;
    private async void DetectPlayer(CancellationToken token)
    {
        try
        {
            if (warningSignHolder != null)
            {
                Destroy(warningSignHolder);
                warningSignHolder = null;
            }

            while (true)
            {
                if (hasDetectPlayer == false)
                {
                    if (Vector3.Distance(transform.position, player.transform.position) < detectRange)
                    {
                        if (spriteRenderer.flipX == false && player.transform.position.x < transform.position.x ||
                            spriteRenderer.flipX == true && player.transform.position.x > transform.position.x)
                        {
                            Ray ray = new Ray(transform.position, (player.transform.position - transform.position).normalized);
                            RaycastHit hit;
                            if (Physics.Raycast(ray, out hit, detectRange))
                            {
                                hasDetectPlayer = true;
                                CancelAllRoutes();
                                if (warningSignHolder == null)
                                {
                                    warningSignHolder = Instantiate(warningPrefab, transform.position + Vector3.up * 3, Quaternion.identity, transform);
                                    warningSignHolder.SetActive(false);
                                    await Task.Delay(250, token);
                                    warningSignHolder.SetActive(true);
                                    await Task.Delay(250, token);
                                    warningSignHolder.SetActive(false);
                                    await Task.Delay(250, token);
                                    warningSignHolder.SetActive(true);
                                    await Task.Delay(250, token);
                                    Destroy(warningSignHolder);
                                    warningSignHolder = null;
                                }
                                if (DayNightSwitcher.Instance.currentDayNight == DayNightSwitcher.DayNightEnum.night)
                                {
                                    chasePlayerCancelSource = new CancellationTokenSource();
                                    ChasePlayer(chasePlayerCancelSource.Token);
                                }
                            }
                        }
                    }
                }
                if (token.IsCancellationRequested)
                {
                    return;
                }
                await Task.Yield();
            }
        }
        catch (System.OperationCanceledException) when (token.IsCancellationRequested)
        {
            return;
        }
    }

    CancellationTokenSource chasePlayerCancelSource;
    private async void ChasePlayer(CancellationToken token)
    {
        animator.Play("angry");
        while (true)
        {
            if (token.IsCancellationRequested)
            {
                if (DayNightSwitcher.Instance.currentDayNight == DayNightSwitcher.DayNightEnum.night)
                    animator.Play("usual");

                if (warningSignHolder != null)
                {
                    Destroy(warningSignHolder);
                    warningSignHolder = null;
                }

                return;
            }
            if (player.transform.position.x > transform.position.x)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
            Vector3 direction = (player.transform.position - transform.position).normalized;
            rb.velocity = direction * 6;
            if (Vector3.Distance(transform.position, player.transform.position) > detectRange)
            {
                if (chasePlayerCancelSource != null)
                {
                    chasePlayerCancelSource.Cancel();
                    chasePlayerCancelSource.Dispose();
                    chasePlayerCancelSource = null;
                }
                hasDetectPlayer = false;
                BackRoute();
            }
            await Task.Yield();
        }
    }
}
