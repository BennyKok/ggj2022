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
            spawnEggCancelSource = new CancellationTokenSource();
            SpawnEggs(seconds, spawnEggCancelSource.Token);
        }
    }

    private async void SpawnEggs(float seconds, CancellationToken token)
    {
        while (true)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
            GameObject temp = Instantiate(explosiveEggPrefab, transform.position, Quaternion.identity);
            temp.GetComponent<ExplosiveEgg>().seconds = eggExplodeSeconds;
            await Task.Delay((int)(1000 * seconds));
        }
    }
}
