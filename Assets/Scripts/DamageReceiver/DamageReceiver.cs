using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mactinite.EDS;
public class DamageReceiver : DamageReceiver<Damage>
{
}

[Serializable]
public class Damage : DamageBase
{
    public Vector3 From;

    public Damage()
    {
    }

    public Damage(float amount , Vector3 from) : base(amount)
    {
        this.From = from;
    }
    
    
}
