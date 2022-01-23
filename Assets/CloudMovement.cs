using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    public List<GameObject> clouds;
    public float minimum = -1.0F;
    public float maximum = 1.0F;

    static float t = 0.0f;
    public float time = 1;

    void Start()
    {
    }

    void Update()
    {
        // if (Time.time - time < 0.2) return;
        Cloud();
    }

    public void DTime()
    {
        // time = Time.time;
    }

    public void Cloud()
    {
        foreach (var cloud in clouds)
        {
            double a = (Mathf.Sin(Time.time * time) + 1) / 2.0;

            var c = cloud.transform.position;
            cloud.transform.DOKill();
            cloud.transform.DOMoveY((float) a, 1f);
        }
    }
}