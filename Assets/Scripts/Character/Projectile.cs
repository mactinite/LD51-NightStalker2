using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public SoundSource soundSource;
    public GameObject hitEffect;
    private void OnCollisionEnter(Collision other)
    {
        if (soundSource)
        {
            soundSource.EmitSound();
        }
        hitEffect.transform.SetParent(null, true);
        hitEffect.SetActive(true);
        gameObject.SetActive(false);
    }
}