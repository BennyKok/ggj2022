using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class ExplosiveEgg : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer explosionEffectSpriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        explosionEffectSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

        explosionEffectSpriteRenderer.enabled = false;
    }

    private async void WaitExplosion()
    {
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

    private void OnCollisionEnter(Collision collision)
    {
        WaitExplosion();
    }
}
