using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

public class ExplosiveEgg : DayNightComponent
{
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer explosionEffectSpriteRenderer;
    public float seconds;
    [HideInInspector]public Vector3 originalPosition;

    private void Awake()
    {
        originalPosition = transform.position;
    }

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        explosionEffectSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

        explosionEffectSpriteRenderer.enabled = false;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (explodeCancelSource != null)
        {
            explodeCancelSource.Cancel();
            explodeCancelSource.Dispose();
            explodeCancelSource = null;
        }
        if (backCancelSource != null)
        {
            backCancelSource.Cancel();
            backCancelSource.Dispose();
            backCancelSource = null;
        }
    }

    protected override void OnDayNightSwitch(bool isDay)
    {
        if (explodeCancelSource != null)
        {
            explodeCancelSource.Cancel();
            explodeCancelSource.Dispose();
            explodeCancelSource = null;
        }
        if (backCancelSource != null)
        {
            backCancelSource.Cancel();
            backCancelSource.Dispose();
            backCancelSource = null;
        }

        if (!isDay)
        {
            explodeCancelSource = new CancellationTokenSource();
            WaitExplosion(seconds, true, explodeCancelSource.Token);
        }
        else
        {
            backCancelSource = new CancellationTokenSource();
            BackToOriginalPosition(backCancelSource.Token);
        }
    }

    private CancellationTokenSource backCancelSource = null;
    private async void BackToOriginalPosition(CancellationToken token)
    {
        try
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            await Task.Delay(1000, token);
            rb.isKinematic = false;
            CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
            capsuleCollider.radius = 2;
            capsuleCollider.height = 2;

            rb.velocity = Vector3.zero;
            Vector3 direction = Vector3.zero;
            while (true)
            {
                if (Vector3.Distance(originalPosition, transform.position) < 3)
                {
                    if (backCancelSource != null)
                    {
                        backCancelSource.Cancel();
                        backCancelSource.Dispose();
                        backCancelSource = null;
                    }
                    Destroy(gameObject);
                }
                direction = (originalPosition - transform.position).normalized;
                rb.velocity = direction * 6;
                await Task.Yield();
            }
        }
        catch (System.Exception)
        {
            return;
        }
    }

    private async void WaitExplosion(float seconds, bool explodeImediately, CancellationToken token)
    {
        try
        {
            if (!explodeImediately)
                await Task.Delay((int)(1000 * seconds), token);
            spriteRenderer.enabled = false;
            explosionEffectSpriteRenderer.enabled = true;

            for (int i = 0; i < 10; i++)
            {
                explosionEffectSpriteRenderer.transform.localScale += new Vector3(1, 1, 0);
                explosionEffectSpriteRenderer.color -= new Color32(0, 0, 0, 25);
                explosionEffectSpriteRenderer.color += new Color32(0, 10, 0, 0);
                await Task.Delay(25, token);
            }
            Destroy(gameObject);
        }
        catch (System.OperationCanceledException) when (token.IsCancellationRequested)
        {
            spriteRenderer.enabled = true;
            explosionEffectSpriteRenderer.enabled = false;
            explosionEffectSpriteRenderer.transform.localScale = Vector3.one * 5;
            return;
        }
    }

    private CancellationTokenSource explodeCancelSource = null;
    private void OnCollisionEnter(Collision collision)
    {
        if (DayNightSwitcher.Instance.currentDayNight == DayNightSwitcher.DayNightEnum.night)
        {
            if (seconds < 0.1f)
                seconds = 0.1f;

            if (explodeCancelSource == null && collision.gameObject.tag != "Egg" && collision.gameObject.tag != "Player")
            {
                explodeCancelSource = new CancellationTokenSource();
                WaitExplosion(seconds, false, explodeCancelSource.Token);
            }
            else if (collision.gameObject.tag == "Player")
            {
                if (explodeCancelSource != null)
                {
                    explodeCancelSource.Cancel();
                    explodeCancelSource.Dispose();
                    explodeCancelSource = null;
                }
                explodeCancelSource = new CancellationTokenSource();
                WaitExplosion(seconds, true, explodeCancelSource.Token);
                CoreRef.Instance.playerEntity.OnDamage(1);
            }
        }
    }
}
