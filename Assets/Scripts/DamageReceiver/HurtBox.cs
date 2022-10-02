using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mactinite.ToolboxCommons;
public class HurtBox : MonoBehaviour
{
    public LayerMask hitLayer;
    public float damage = 25;

    private List<Collider> damaged = new List<Collider>();
    
    private void Update()
    {
        // enabled?
    }

    private void OnEnable()
    {
        damaged.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(damaged.Contains(other)) { return;}
        if (other.gameObject.IsInLayerMask(hitLayer) && enabled)
        {
            damaged.Add(other);
            Damage dmg = new Damage(damage, transform.position);
            other.gameObject.SendMessage("Damage", dmg, SendMessageOptions.DontRequireReceiver);
        }
    }
}
