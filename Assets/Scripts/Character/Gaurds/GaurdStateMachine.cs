using System.Collections;
using System.Collections.Generic;
using ECM.Controllers;
using UnityEngine;
using UnityEngine.AI;

public class GaurdStateMachine : StateMachine<GaurdStateMachine>
{
    public BaseAgentController controller;
    public AquireTarget aquireTarget;
    public NavMeshAgent agent;
    public Vector3 lastAlertedPosition;
    public float attackRange = 5f;
    public Vector3 originalPosition;
    public Vector3 originalForward;
    public float confidence = 0;
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        originalForward = transform.forward;
        
        StateMap.Add("StandingIdle", new StandingIdle(this));
        StateMap.Add("Chasing", new Chasing(this));
        StateMap.Add("Searching", new Searching(this));
        
        SetState("StandingIdle");
    }
}

public class StandingIdle : State<GaurdStateMachine>
{
    public StandingIdle(GaurdStateMachine stateMachine) : base(stateMachine)
    {
    }

    public StandingIdle()
    {
    }

    public override IEnumerator Start()
    {
        while (StateMachine.aquireTarget.target == null)
        {
            if(StateMachine.agent.remainingDistance <=0)
                StateMachine.controller.RotateTowards(StateMachine.originalForward);
            yield return null;
        }
        StateMachine.SetState("Chasing");
        yield break;
    }

    public override IEnumerator End()
    {
        yield break;
    }
}

public class Searching : State<GaurdStateMachine>
{
    public Searching(GaurdStateMachine stateMachine) : base(stateMachine)
    {
    }

    public Searching()
    {
    }

    public override IEnumerator Start()
    {
        Vector3 dir = StateMachine.transform.position - StateMachine.aquireTarget.lastSeenPosition;
        StateMachine.agent.SetDestination(StateMachine.aquireTarget.lastSeenPosition);

        // Walk towards last seen position till we see 
        while (StateMachine.agent.remainingDistance > 0)
        {

            if (StateMachine.aquireTarget.target != null)
            {
                StateMachine.SetState("Chasing");
                yield break;
            }
            yield return null;
        }

        float timer = 0f;
        int rotationCount = 0;
        while (StateMachine.aquireTarget.target == null)
        {
            timer += Time.deltaTime;
            var right = Vector3.right;
            Vector3 rotationDirection =
                ( rotationCount % 2 == 0 ? right : -right);

            if (rotationCount >= 5)
            {
                StateMachine.agent.SetDestination(StateMachine.originalPosition);
                StateMachine.SetState("StandingIdle");
                yield break;
            }
            
            if (timer <= 1f)
            {
                if(StateMachine.agent.destination != rotationDirection)
                    StateMachine.controller.RotateTowards(rotationDirection * StateMachine.agent.angularSpeed * Time.deltaTime);
            }
            else
            {
                rotationCount++;
                timer = 0;
            }
            yield return null;
        }
        
        StateMachine.SetState("Chasing");
        
        yield break;
    }

    public override IEnumerator End()
    {
        yield break;
    }
}

public class Chasing : State<GaurdStateMachine>
{
    public Chasing(GaurdStateMachine stateMachine) : base(stateMachine)
    {
    }

    public Chasing()
    {
    }

    public override IEnumerator Start()
    {
        while (StateMachine.aquireTarget.target != null)
        {
            StateMachine.agent.SetDestination(StateMachine.aquireTarget.lastSeenPosition);
            yield return null;
        }
        
        StateMachine.SetState("Searching");
        yield break;
    }

    public override IEnumerator End()
    {
        yield break;
    }
}

public class Attacking : State<GaurdStateMachine>
{
    public Attacking(GaurdStateMachine stateMachine) : base(stateMachine)
    {
    }

    public Attacking()
    {
    }

    public override IEnumerator Start()
    {
        
        yield break;
    }

    public override IEnumerator End()
    {
        yield break;
    }
}


