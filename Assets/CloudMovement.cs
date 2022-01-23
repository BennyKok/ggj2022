using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CloudMovement : DayNightComponent
{
    public float time = 1;
    public Animator filp;

    public float Y = 180;
    // public bool light;

    protected override void OnDayNightSwitch(bool isLight)
    {
        if (isLight)
        {
            filp.ResetTrigger("toStar");
            filp.SetTrigger("toCloud");
        }
        else
        {
            filp.ResetTrigger("toCloud");
            filp.SetTrigger("toStar");
        }

        if (isLight || !isLight) Y += 180;
        if (Y > 540) Y = 0;
    }

    void Update()
    {
        Move();
        filp = gameObject.GetComponent<Animator>();
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