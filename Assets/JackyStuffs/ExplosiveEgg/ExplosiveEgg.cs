using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

public class ExplosiveEgg : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer explosionEffectSpriteRenderer;
    public float seconds;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        explosionEffectSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

        explosionEffectSpriteRenderer.enabled = false;
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
            spriteRenderer.enabled = false;
            explosionEffectSpriteRenderer.enabled = true;
            return;
        }
    }

    private CancellationTokenSource explodeCancelSource = null;
    private async void OnCollisionEnter(Collision collision)
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
        }
    }
}
