using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameStateMachine : StateMachine<GameStateMachine>
{
    public PlayerControllerInput controllerInput ;

    void Start()
    {
        StateMap.Add("Playing", new PlayingState(this));
        StateMap.Add("Pause", new PauseState(this));

        SetState("Playing");
    }
}

public class PlayingState : State<GameStateMachine>
{
    public PlayingState(GameStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override IEnumerator Start()
    {
        StateMachine.controllerInput.enabled = true;
        StateMachine.controllerInput.input.actions["Pause"].performed += Pause;

        yield break;
    }

    public override IEnumerator End()
    {
        // disable character controller
        StateMachine.controllerInput.enabled = false;
        // unbind pause button
        StateMachine.controllerInput.input.actions["Pause"].performed -= Pause;

        yield break;
    }

    private void Pause(InputAction.CallbackContext context)
    {
        StateMachine.SetState("Pause");
    }
}


public class PauseState : State<GameStateMachine>
{
    public PauseState(GameStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override IEnumerator Start()
    {
        Time.timeScale = 0;
        StateMachine.controllerInput.input.actions["Pause"].performed += UnPause;
        yield break;
    }

    public override IEnumerator End()
    {
        Time.timeScale = 1;
        StateMachine.controllerInput.input.actions["Pause"].performed -= UnPause;
        yield break;
    }

    private void UnPause(InputAction.CallbackContext context)
    {
        StateMachine.SetState("Playing");
    }
}