using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CloudMovement : DayNightComponent
{
    public float time = 1;

    public float Y;
    // public bool light;

    protected override void OnDayNightSwitch(bool isLight)
    {
        // gameObject.SetActive(isLight);
        if (isLight || !isLight)
        {
            Y += 180;
            gameObject.transform.DORotate(new Vector3(0, Y, 0), 2f);
        }

        if (Y > 540) Y = 0;
    }

    void Update()
    {
        Move();
    }

    public void Move()
    {
        gameObject.transform.DOKill();
        var c = gameObject.transform.position;
        double a = (Mathf.Sin(Time.time * time) + c.y);
        gameObject.transform.DOMoveY((float) a, 1f);
        gameObject.transform.DORotate(new Vector3(0, Y, 0), 2f);
    }
}