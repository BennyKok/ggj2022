using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ScaleTween : MonoBehaviour
{
    public float time;

    public void ScaleTo(float scale)
    {
        transform.DOScale(scale, time);
    }
}