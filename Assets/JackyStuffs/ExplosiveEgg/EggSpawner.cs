using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggSpawner : MonoBehaviour
{
    public GameObject explosiveEggPrefab;

    public float explodeSeconds, spawnSeconds;
    public int eggLimited;

    [SerializeField]private int eggSpawned = 0;

    public List<GameObject> eggs;

    private void Start()
    {
        eggs = new List<GameObject>();
        DayNightSwitcher.Instance.SwitchDayNightEvent += OnSwitch;
    }

    private void OnDestroy()
    {
        DayNightSwitcher.Instance.SwitchDayNightEvent -= OnSwitch;
    }

    private void OnSwitch(bool isLight)
    {
        StopAllCoroutines();

        if (!isLight)
        {
            foreach (GameObject g in eggs)
            {
                if (g != null)
                    Destroy(g);
            }
            eggs.Clear();
            eggSpawned = 0;

            StartCoroutine(SpawnExplosiveEgg());
        }
    }

    private IEnumerator SpawnExplosiveEgg()
    {
        while (true)
        {
            if (eggSpawned < eggLimited)
            {
                GameObject temp = Instantiate(explosiveEggPrefab, transform.position, Quaternion.identity);
                eggs.Add(temp);
                temp.GetComponent<ExplosiveEgg>().explodeSeconds = explodeSeconds;
                eggSpawned++;
            }
            else
            {
                yield return new WaitUntil(() => eggSpawned < eggLimited);
            }
            yield return new WaitForSeconds(spawnSeconds);
        }
    }
}
