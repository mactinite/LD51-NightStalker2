using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundListener : MonoBehaviour
{
    public bool newSound = false;
    public Vector3 soundLocation;
    
    public void HearSound(Vector3 from)
    {
        newSound = true;
        soundLocation = from;
    }

}
