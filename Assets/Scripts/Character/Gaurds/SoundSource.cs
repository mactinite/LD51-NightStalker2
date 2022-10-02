using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundSource : MonoBehaviour
{
    public LayerMask listenerLayerMask;
    public float range = 10f;

    public void EmitSound()
    {
        var colliders = Physics.OverlapSphere(transform.position, range, listenerLayerMask);
        if (colliders.Length > 0)
        {
            foreach (var collider in colliders) 
            {
                collider.gameObject.BroadcastMessage("HearSound", transform.position, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
