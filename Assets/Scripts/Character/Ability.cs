using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public string Animation = "Throw";
    public int animatorLayer = 2;
    public Rigidbody projectilePrefab;
    public Transform spawnPosition;
    public HurtBox hurtbox;
    public float projectileForce = 100f;
    public LayerMask projectileHitLayerMask;
    public CharacterAnimation characterAnim;
    public float activationDelay = 0.2f;
    public float abilityDuration = 1f;
    private float layerWeight = 0;
    private float _targetLayerWeight = 0;
    private float _smoothingVel;

    private bool ready = true;

    private Vector3 screenCenter = new Vector3(0.5f, 0.5f);
    private void Update()
    {
        if (!ready)
        {
            layerWeight = Mathf.SmoothDamp(layerWeight, _targetLayerWeight, ref _smoothingVel, 0.1f);
            characterAnim.anim.SetLayerWeight(animatorLayer, layerWeight);
        }
    }

    public void TriggerAbility()
    {
        if(ready)
            StartCoroutine(AbilityRoutine());
    }

    IEnumerator AbilityRoutine()
    {
        ready = false;
        characterAnim.anim.SetTrigger(Animation);
        _targetLayerWeight = 1f;
        yield return new WaitForSeconds(activationDelay);

        if (projectilePrefab)
        {
            FireProjectile();
        }

        if (hurtbox != null)
        {
            hurtbox.enabled = true;
        }
        yield return new WaitForSeconds(abilityDuration - activationDelay - 0.1f);

        _targetLayerWeight = 0;
        while (layerWeight > 0.1f)
        {
            yield return null;
        }
        
        if (hurtbox != null)
        {
            hurtbox.enabled = false;
        }
        characterAnim.anim.SetLayerWeight(animatorLayer, 0);
        ready = true;
    }


    public void FireProjectile()
    {
        var projectileRigidBody = Instantiate(projectilePrefab, spawnPosition.position,
            Quaternion.LookRotation(spawnPosition.forward));
        var cam = Camera.main;
        Ray screenRay = cam.ViewportPointToRay(screenCenter);
        Vector3 targetPos = cam.transform.position + (cam.transform.forward * 20f);
        if (Physics.Raycast(screenRay.origin, screenRay.direction * 20f, out var hit, projectileHitLayerMask))
        {
            targetPos = hit.point;
        }
        Debug.DrawLine(screenRay.origin, targetPos, Color.red, 0.1f);
        Debug.DrawLine(spawnPosition.position, targetPos, Color.blue, 0.1f);
        Vector3 direction = (targetPos - spawnPosition.position);
        projectileRigidBody.AddForce(direction * projectileForce);
    }
}
