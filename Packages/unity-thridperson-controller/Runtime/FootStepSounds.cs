using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FootStepSounds : MonoBehaviour
{
    public RigidbodyController controller;
    public AudioSource left, right;

    public UnityEvent onLeftFoot;
    public UnityEvent onRightFoot;

    public void OnLeftFoot()
    {
        if (!controller.grounded) return;
        
        left.Play();
        onLeftFoot.Invoke();
    }

    public void OnRightFoot()
    {
        if (!controller.grounded) return;

        right.Play();
        onRightFoot.Invoke();
    }
}

