using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeartbeat : MonoBehaviour
{
    public SoundSource heartBeatSoundSource;
    private void Start()
    {
        HeartbeatMechanic.onTick.AddListener(OnBeat);
    }

    private void OnBeat()
    {
        heartBeatSoundSource.EmitSound();
    }
}
