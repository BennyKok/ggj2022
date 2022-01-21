using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RigidbodyController))]
public class FootStepEvents : MonoBehaviour
{
    public float minSoundInterval = 0.2f;
    public float soundInterval;
    public float walkVelFraction = 7f;
    public UnityEvent onLeftFoot;
    public UnityEvent onRightFoot;

    private int index;
    private float lastSoundTime;

    private RigidbodyController controller;

    private void Start()
    {
        controller = GetComponent<RigidbodyController>();
    }


    void Update()
    {
        var vel = controller.targetBody.velocity;
        vel.y = 0;
        if (controller.grounded && vel.magnitude > 0)
        {
            if (Time.time - lastSoundTime >= Mathf.Max(minSoundInterval, soundInterval / vel.magnitude * walkVelFraction))
            {
                if (index == 0)
                {
                    lastSoundTime = Time.time;
                    onLeftFoot.Invoke();
                    index++;
                }
                else
                {
                    lastSoundTime = Time.time;
                    onRightFoot.Invoke();
                    index = 0;
                }
            }
        }
    }
}

