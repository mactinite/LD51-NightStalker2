using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public Image fillImage;
    public bool shakeOnChange = false;
    public float shakeAmount = 0.2f;
    public float dampingSpeed;
    private float _currentValue = 1;
    private float _velocity = 0;

    public DamageReceiver damageReceiver;

    private void Start()
    {
        if (damageReceiver)
        {
            damageReceiver.OnDamage += OnDamage;
            damageReceiver.OnDestroyed += OnDestroyed;
        }
    }

    private void OnDestroyed(Vector2 obj)
    {
        damageReceiver.OnDamage -= OnDamage;
        damageReceiver.OnDestroyed -= OnDestroyed;
        gameObject.SetActive(false);
    }

    private void OnDamage(Vector2 at, Damage dmg)
    {
        SetValue(Mathf.InverseLerp(0, damageReceiver.maxHealth, dmg.NewHealth));
    }

    private void Update()
    {
        fillImage.fillAmount = Mathf.SmoothDamp(fillImage.fillAmount, _currentValue, ref _velocity, dampingSpeed);
    }

    public void SetValue(float val)
    {
        if (shakeOnChange)
        {
            Vector3 shakeAxis = Vector3.one * shakeAmount;
            iTween.PunchScale(gameObject,shakeAxis, 0.3f);
        }

        _currentValue = val;
    }
}