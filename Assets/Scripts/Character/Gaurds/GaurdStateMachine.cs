using System.Collections;
using System.Collections.Generic;
using ECM.Controllers;
using mactinite.EDS;
using UnityEngine;
using UnityEngine.AI;

public class GaurdStateMachine : StateMachine<GaurdStateMachine>
{
    public BaseAgentController controller;
    public SoundListener listener;
    public DamageReceiver damageReceiver;
    public AquireTarget aquireTarget;
    public NavMeshAgent agent;
    public Vector3 lastAlertedPosition;
    public float attackRange = 5f;
    public Vector3 originalPosition;
    public Vector3 originalForward;
    public GameObject searchingIndicator;

    public Ability swordAbility;
    // Start is called before the first frame update
    void Start()
    {
        damageReceiver.OnDestroyed += Destroyed;
        originalPosition = transform.position;
        originalForward = transform.forward;
        
        StateMap.Add("StandingIdle", new StandingIdle(this));
        StateMap.Add("Chasing", new Chasing(this));
        StateMap.Add("Searching", new Searching(this));
        StateMap.Add("Attacking", new Attacking(this));
        StateMap.Add("Dead", new Dead(this));
        SetState("StandingIdle");
    }

    private void Destroyed(Vector2 obj)
    {
        damageReceiver.OnDestroyed -= Destroyed;
        StopAllCoroutines();
        SetState("Dead");
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
        while (StateMachine.aquireTarget.alertness < 1f)
        {
            if (StateMachine.listener.newSound)
            {
                StateMachine.aquireTarget.lastSeenPosition = StateMachine.listener.soundLocation;
                StateMachine.listener.newSound = false;
                StateMachine.SetState("Searching");
                yield break;
            }
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
        StateMachine.searchingIndicator.SetActive(true);

        // Walk towards last seen position till we see 
        while (StateMachine.agent.remainingDistance > 0)
        {

            if (StateMachine.aquireTarget.target != null)
            {
                StateMachine.aquireTarget.alertness = 1f;
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
        StateMachine.searchingIndicator.SetActive(false);
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
            var targetPos = StateMachine.aquireTarget.target.transform.position;
            StateMachine.agent.SetDestination(StateMachine.aquireTarget.lastSeenPosition);
            if (Vector3.Distance(targetPos, StateMachine.transform.position) < StateMachine.attackRange)
            {
                StateMachine.SetState("Attacking");
                yield break;
            }
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
        yield return new WaitForSeconds(0.5f);
        StateMachine.swordAbility.TriggerAbility();
        yield return new WaitForSeconds(StateMachine.swordAbility.abilityDuration);
        StateMachine.SetState("Chasing");
        yield break;
    }

    public override IEnumerator End()
    {
        yield break;
    }
}
public class Dead : State<GaurdStateMachine>
{
    public Dead(GaurdStateMachine stateMachine) : base(stateMachine)
    {
    }

    public Dead()
    {
    }

    public override IEnumerator Start()
    {
        // we done
        StateMachine.aquireTarget.enabled = false;
        StateMachine.enabled = false;
        yield break;
    }

    public override IEnumerator End()
    {
        yield break;
    }
}


