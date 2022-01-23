using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

public class OwlAI : DayNightComponent
{
    public Transform route;

    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    protected Rigidbody rb;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        transform.position = route.GetChild(0).position;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected CancellationTokenSource backRouteCancelSource;
    protected CancellationTokenSource flyRouteCancelSource;
    protected override void OnDayNightSwitch(bool isDay)
    {
        CancelAllRoutes();

        if (isDay)
        {
            animator.Play("sleep");
        }
        else
        {
            animator.Play("usual");

            BackRoute();
        }
    }

    protected void BackRoute()
    {
        if (backRouteCancelSource != null)
        {
            backRouteCancelSource.Cancel();
            backRouteCancelSource.Dispose();
            backRouteCancelSource = null;
        }
        backRouteCancelSource = new CancellationTokenSource();
        BackToRoute(backRouteCancelSource.Token);
    }

    protected void FlyRoute()
    {
        if (flyRouteCancelSource != null)
        {
            flyRouteCancelSource.Cancel();
            flyRouteCancelSource.Dispose();
            flyRouteCancelSource = null;
        }
        flyRouteCancelSource = new CancellationTokenSource();
        FlyToRoute(flyRouteCancelSource.Token);
    }

    protected void CancelAllRoutes()
    {
        if (backRouteCancelSource != null)
        {
            backRouteCancelSource.Cancel();
            backRouteCancelSource.Dispose();
            backRouteCancelSource = null;
        }
        if (flyRouteCancelSource != null)
        {
            flyRouteCancelSource.Cancel();
            flyRouteCancelSource.Dispose();
            flyRouteCancelSource = null;
        }
    }

    private int currentFlyingPoint = 0;
    private async void BackToRoute(CancellationToken token)
    {
        try
        {
            float closestPoint = Vector3.Distance(route.GetChild(0).position, transform.position);
            int point = 0;
            for (int i = 0; i < route.childCount; i++)
            {
                if (Vector3.Distance(route.GetChild(i).position, transform.position) < closestPoint)
                {
                    closestPoint = Vector3.Distance(route.GetChild(i).position, transform.position);
                    point = i;
                }
            }
            currentFlyingPoint = point;
            Vector3 direction = (route.GetChild(point).position - transform.position).normalized;

            if (route.GetChild(point).position.x > transform.position.x)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }

            while (Vector3.Distance(route.GetChild(point).position, transform.position) > 2)
            {
                rb.velocity = direction * 6;
                await Task.Delay(100, token);
            }
            FlyRoute();
        }
        catch (System.OperationCanceledException) when (token.IsCancellationRequested)
        {
            rb.velocity = Vector3.zero;
            return;
        }
    }

    private async void FlyToRoute(CancellationToken token)
    {
        try
        {
            while (true)
            {
                if (route.GetChild(currentFlyingPoint).position.x > transform.position.x)
                {
                    spriteRenderer.flipX = true;
                }
                else
                {
                    spriteRenderer.flipX = false;
                }
                do
                {
                    Vector3 direction = (route.GetChild(currentFlyingPoint).position - transform.position).normalized;
                    rb.velocity = direction * 6;
                    await Task.Delay(100, token);
                } while (Vector3.Distance(route.GetChild(currentFlyingPoint).position, transform.position) > 2);
                currentFlyingPoint = (currentFlyingPoint + 1) % route.childCount;
                rb.isKinematic = true;
                await Task.Delay(2000, token);
                rb.isKinematic = false;
            }
        }
        catch (System.OperationCanceledException) when (token.IsCancellationRequested)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            return;
        }
    }
}
