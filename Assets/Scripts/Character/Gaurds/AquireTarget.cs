using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AquireTarget : MonoBehaviour
{
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

    // Update is called once per frame
    void LateUpdate()
    {
        // lets just populate a list of closeby targets, and then we'll let the behavior tree decide
        GetTarget();
    }

    // Check if any objects within the view distance match the target mask
    private void GetTarget()
    {
        Physics.OverlapSphereNonAlloc(transform.position, viewDistance, targetBuffer, targetMask);
        if(target != null) lastSeenPosition = target.transform.position;
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
    }

    private bool IsTargetHidden(GameObject potentialTarget)
    {
        bool hidden = false;

        Vector3 targetDir = potentialTarget.transform.position - transform.position;
        Vector3 forwardDir = transform.forward;
        Vector3 headPos = transform.position + (transform.up * height);
        Vector3 targetHeadPos = potentialTarget.transform.position + (transform.up * height);


        if (Vector3.Dot(forwardDir, targetDir.normalized) < 1 - FoV)
        {
            hidden = true;
        }

        if (Physics.Linecast(headPos, targetHeadPos, obstacleMask))
        {
            hidden = true;
        }

        if (Vector3.Distance(headPos, targetHeadPos) > viewDistance)
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