using System;
using System.Collections;
using System.Collections.Generic;
using ECM.Controllers;
using mactinite.EDS;
using UnityEngine;

public class AquireTarget : MonoBehaviour
{
    public float alertness = 0;
    public float fullAlertnessTime = 2;

    public LayerMask obstacleMask;
    public LayerMask targetMask;
    public float FoV = 0.6f;
    public Vector3 lastSeenPosition;
    public GameObject target;
    private GameObject player;
    [SerializeField] private List<GameObject> targets = new List<GameObject>();
    public float viewDistance = 3f;
    public float height = 1f;
    private Collider[] targetBuffer = new Collider[5];
    public HealthBarController alertnessBar;

    // Update is called once per frame
    void LateUpdate()
    {
        // lets just populate a list of closeby targets, and then we'll let the behavior tree decide
        GetTarget();

        if (target != null && alertness > 0.2f)
        {
            alertnessBar.gameObject.SetActive(true);
        } else if (target == null)
        {
            alertnessBar.gameObject.SetActive(false);
        }

        alertnessBar.SetValue(alertness);
    }

    private void OnDisable()
    {
        alertnessBar.gameObject.SetActive(false);
    }

    // Check if any objects within the view distance match the target mask
    private void GetTarget()
    {
        Physics.OverlapSphereNonAlloc(transform.position, viewDistance, targetBuffer, targetMask);
        if (target != null) lastSeenPosition = target.transform.position;
        player = null;
        target = null;
        foreach (Collider potentialTarget in targetBuffer)
        {
            if (potentialTarget == null) break;

            if (!IsTargetHidden(potentialTarget.gameObject))
            {
                target = potentialTarget.gameObject;
                if (target.CompareTag("Player"))
                {
                    player = target;
                }
            }
        }

        //prioritize the player as a target
        if (player != null)
        {
            target = player;
        }

        if (target)
        {
            alertness += Time.deltaTime / fullAlertnessTime;
        }
        else
        {
            alertness -= Time.deltaTime / fullAlertnessTime;
        }

        alertness = Mathf.Clamp01(alertness);
    }

    private bool IsTargetHidden(GameObject potentialTarget)
    {
        bool hidden = false;

        Vector3 targetDir = potentialTarget.transform.position - transform.position;
        Vector3 forwardDir = transform.forward;
        Vector3 headPos = transform.position + (transform.up * height);
        Vector3 targetHeadPos = potentialTarget.transform.position + (transform.up * height);

        // hide targets behind
        if (Vector3.Dot(forwardDir, targetDir.normalized) < 1 - FoV)
        {
            // only hidden if crouching
            if (potentialTarget.TryGetComponent<BaseCharacterController>(out var characterController) &&
                characterController.isCrouching)
            {
                hidden = true;
            }
        }

        // hide targets obscured by obstacles
        if (Physics.Linecast(headPos, targetHeadPos, obstacleMask))
        {
            hidden = true;
        }

        // hide targets beyond vision range
        if (Vector3.Distance(headPos, targetHeadPos) > viewDistance)
        {
            hidden = true;
        }

        // hide dead targets;
        if (potentialTarget.TryGetComponent<DamageReceiver>(out var damageReceiver) && damageReceiver.destroyed)
        {
            hidden = true;
        }


        return hidden;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        if (target != null)
        {
            Vector3 headPos = transform.position + (transform.up * height);
            Vector3 targetHeadPos = target.transform.position + (transform.up * height);

            Gizmos.color = IsTargetHidden(target) ? Color.grey : Color.red;
            Gizmos.DrawLine(headPos, targetHeadPos);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(lastSeenPosition, 0.5f);
    }
}