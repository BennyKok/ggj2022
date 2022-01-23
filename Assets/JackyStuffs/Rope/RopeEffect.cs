using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeEffect : MonoBehaviour
{
    public GameObject ropePrefab;
    public GameObject[] knotsPrefabs;

    private GameObject ropeHolder;
    private GameObject knotHolder;
    private SpriteRenderer ropeSpriteRenderer;
    private void Awake()
    {
        ropeSpriteRenderer = ropePrefab.GetComponent<SpriteRenderer>();
        ropeHolder = Instantiate(ropePrefab, transform.position, Quaternion.identity, transform);
        knotHolder = Instantiate(knotsPrefabs[Random.Range(0, knotsPrefabs.Length)], transform.position + Vector3.up + Vector3.forward, Quaternion.identity, transform);
        knotHolder.transform.localScale = Vector3.one * 2;
    }

    private void FixedUpdate()
    {
        float ropeY = PlayerDayNightController.Instance.topPosition.y - transform.position.y;
        if (ropeY < 1) return;
        Vector3 midpoint = (PlayerDayNightController.Instance.topPosition + transform.position) / 2;
        Vector3 originalPosition = ropeHolder.transform.position;
        ropeHolder.transform.position = new Vector3(originalPosition.x, midpoint.y, -1);
        ropeHolder.transform.localScale = new Vector3(1, ropeY * 0.5f * transform.localScale.y, 1);
        
    }
}
