using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class ExplosiveEgg : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer explosionEffectSpriteRenderer;
    public float explodeSeconds;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        explosionEffectSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

        explosionEffectSpriteRenderer.enabled = false;
        DayNightSwitcher.Instance.SwitchDayNightEvent += OnSwitch;
    }

    private void OnDestroy()
    {
        DayNightSwitcher.Instance.SwitchDayNightEvent -= OnSwitch;
    }

    private void OnSwitch(bool isLight)
    {
        if (isLight)
            WaitExplosion(explodeSeconds);
    }

    private async void WaitExplosion(float seconds)
    {
        float totalSecondWaited = 0;
        try
        {
            do
            {
                spriteRenderer.color = new Color32(230, 140, 70, 255);
                await Task.Delay(50);
                spriteRenderer.color = new Color32(220, 45, 60, 255);
                await Task.Delay(50);
                spriteRenderer.color = new Color32(255, 255, 255, 255);
                await Task.Delay(50);
                totalSecondWaited += 0.15f;
            } while (totalSecondWaited < seconds);
            spriteRenderer.enabled = false;
            explosionEffectSpriteRenderer.enabled = true;

            for (int i = 0; i < 10; i++)
            {
                explosionEffectSpriteRenderer.transform.localScale += new Vector3(1, 1, 0);
                explosionEffectSpriteRenderer.color -= new Color32(0, 0, 0, 25);
                explosionEffectSpriteRenderer.color += new Color32(0, 10, 0, 0);
                await Task.Delay(25);
            }
            Destroy(gameObject);
        }
        catch (MissingReferenceException e)
        {
            return;
        }
    }
}
