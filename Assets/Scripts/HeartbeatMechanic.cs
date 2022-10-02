using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HeartbeatMechanic : MonoBehaviour
{
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

        if (timer > interval)
        {
            onTick.Invoke();
        }
    }
}
