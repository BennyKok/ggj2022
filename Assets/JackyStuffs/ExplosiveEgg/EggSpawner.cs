using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

public class EggSpawner : DayNightComponent
{
    public GameObject explosiveEggPrefab;
    public float seconds;
    public float eggExplodeSeconds;
    public Transform toPositionSInceDrop;
    public Vector3 offset;

    private CancellationTokenSource spawnEggCancelSource;

    private void Awake()
    {
        if (seconds < 0.1f)
            seconds = 0.1f;
    }

    protected override void OnDayNightSwitch(bool isDay)
    {
        if (spawnEggCancelSource != null)
        {
            spawnEggCancelSource.Cancel();
            spawnEggCancelSource.Dispose();
            spawnEggCancelSource = null;
        }

        if (!isDay)
        {
            if (toPositionSInceDrop == null)
            {
                spawnEggCancelSource = new CancellationTokenSource();
                SpawnEggs(seconds, spawnEggCancelSource.Token);

            }
            else
            {
                spawnEggCancelSource = new CancellationTokenSource();
                SpawnEggs(seconds, spawnEggCancelSource.Token, toPositionSInceDrop);
            }
        }
    }

    private async void SpawnEggs(float seconds, CancellationToken token, Transform toPosition = null)
    {
        while (true)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
            GameObject temp = Instantiate(explosiveEggPrefab, transform.position, Quaternion.identity);
            temp.GetComponent<ExplosiveEgg>().seconds = eggExplodeSeconds;
            if (toPosition != null)
            {
                temp.GetComponent<ExplosiveEgg>().originalPosition = transform.position;
                temp.transform.position = toPosition.position + offset;
            }
            await Task.Delay((int)(1000 * seconds));
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (spawnEggCancelSource != null)
        {
            spawnEggCancelSource.Cancel();
            spawnEggCancelSource.Dispose();
            spawnEggCancelSource = null;
        }
    }
}
