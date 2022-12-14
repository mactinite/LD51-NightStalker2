using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine<T> : MonoBehaviour where T : StateMachine<T>
{
    private State<T> State = null;
    public Dictionary<string, State<T>> StateMap = new Dictionary<string, State<T>>();
    private IEnumerator SetState(State<T> state)
    {
        if(State != null)
            yield return StartCoroutine(State.End());
        State = state;
        yield return StartCoroutine(State.Start());
    }
    public void SetState(string stateName)
    {
        if (StateMap.TryGetValue(stateName, out var nextState))
        {
            StartCoroutine(SetState(nextState));
        }
        else
        {
            throw new Exception(
                $"{stateName} not found in runtime state map. \n" +
                $"Did you forget to register the state or use the wrong identifier?");
        }
    }
    public void Update()
    {
        if(State != null)
            StartCoroutine(State.Tick());
    }

    private void OnDestroy()
    {
        this.StopAllCoroutines();
    }

    private void Awake()
    {
        State = null;
    }
}
