using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerInput : MonoBehaviour
{
    [SerializeField]
    public ActionCharacterController controller;
    [SerializeField]
    public PlayerInput input;

    public Ability throwAbility;
    public Ability swordAbility;


    private void Update()
    {
        controller.inputMove = input.actions["Move"].ReadValue<Vector2>();
        controller.inputJump = input.actions["Jump"].ReadValue<float>() > 0;
        if (input.actions["Crouch"].triggered)
        {
            controller.crouch = !controller.crouch;
        }
        
        if (input.actions["Aim"].IsPressed() && input.actions["Action"].triggered)
        {
            throwAbility.TriggerAbility();
        }else if (input.actions["Action"].triggered)
        {
            //do an attack here
            swordAbility.TriggerAbility();
        }

        if (controller.inputJump)
        {
            controller.crouch = false;
        }
        
    }
}
