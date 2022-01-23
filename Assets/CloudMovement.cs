using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    public List<GameObject> clouds;
    public float time = 1;

    void Start()
    {
    }

    void Update()
    {
        Cloud();
    }

    public void Cloud()
    {
        foreach (var cloud in clouds)
        {
            var c = cloud.transform.position;
            double a = (Mathf.Sin(Time.time * time) + c.y);
            cloud.transform.DOKill();
            cloud.transform.DOMoveY((float) a, 1f);
        }
    }
}