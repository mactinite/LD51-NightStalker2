using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HeartbeatMechanic : MonoBehaviour
{
    public float shakeFreqeuncy = 0.2f;
    public float shakeAmplitude = 0.1f;
    private float timer = 0f;
    public float interval = 10;
    public static UnityEvent onTick = new UnityEvent();
    public Image heartbeatIndicator;

    private void Awake()
    {
        // clear all listeners before game starts;
        onTick.RemoveAllListeners();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        iTween.ShakePosition(heartbeatIndicator.gameObject, Vector3.one * (timer / interval * shakeAmplitude),
            shakeFreqeuncy);

        if (timer >= interval)
        {
            onTick.Invoke();
            iTween.PunchScale(heartbeatIndicator.gameObject, (Vector3.one * 1.25f), 1f);
            timer = 0;
        }
    }
}