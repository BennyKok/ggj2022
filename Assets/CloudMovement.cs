using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CloudMovement : DayNightComponent
{
    public List<GameObject> clouds;
    public float time = 1;

    protected override void OnDayNightSwitch(bool isLight)
    {
        gameObject.SetActive(isLight);
    }

    void Update()
    {
        Cloud();
    }
    
    public void Cloud()
    {
        foreach (var cloud in clouds)
        {
            cloud.transform.DOKill();
            var c = cloud.transform.position;
            double a = (Mathf.Sin(Time.time * time) + c.y);
            cloud.transform.DOMoveY((float) a, 1f);
        }
    }
}