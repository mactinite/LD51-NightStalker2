using System;
using System.Collections;
using System.Collections.Generic;
using ECM.Common;
using ECM.Controllers;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    public Animator anim;

    public BaseCharacterController controller;
    public DamageReceiver damageReceiver;
    public PlayerControllerInput playerControllerInput;

    private float crouchLayerWeight = 0;
    private float currentCrouchLayerWeight = 0;
    private float smoothVelocity;

    // Start is called before the first frame update
    void Start()
    {
        if (damageReceiver != null)
        {
            damageReceiver.OnDestroyed += Destroyed;
        }
    }

    private void OnDisable()
    {
        if (damageReceiver != null)
        {
            damageReceiver.OnDestroyed -= Destroyed;
        }
    }

    private void Destroyed(Vector2 at)
    {
        anim.SetBool("Dead", true);
        controller.movement.velocity = Vector3.zero;
        controller.movement.capsuleCollider.enabled = false;
        controller.movement.cachedRigidbody.isKinematic = true;
        controller.enabled = false;
        if(playerControllerInput != null)
            playerControllerInput.enabled = false;

        this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        currentCrouchLayerWeight = anim.GetLayerWeight(1);
        
        if (controller.moveDirection.magnitude > 0)
        {
            var relativeTo = transform.InverseTransformDirection(controller.movement.velocity / controller.movement.maxLateralSpeed);
            anim.SetBool("Moving", true);
            anim.SetFloat("HorizontalSpeed", relativeTo.x);
            anim.SetFloat("ForwardSpeed", relativeTo.z);

        }
        else
        {
            anim.SetBool("Moving", false);
            anim.SetFloat("HorizontalSpeed", 0);
            anim.SetFloat("ForwardSpeed", 0);
        }

        if (controller.isCrouching)
        {
            crouchLayerWeight = Mathf.SmoothDamp(currentCrouchLayerWeight, 1, ref smoothVelocity, 0.1f);
        }
        else
        {
            crouchLayerWeight = Mathf.SmoothDamp(currentCrouchLayerWeight, 0, ref smoothVelocity, 0.1f);
        }
        
        anim.SetLayerWeight(1, crouchLayerWeight);


        anim.SetBool("Jumping", controller.isJumping);
        anim.SetBool("Grounded", controller.isGrounded);
    }
}