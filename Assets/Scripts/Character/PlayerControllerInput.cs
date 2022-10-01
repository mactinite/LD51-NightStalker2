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

    private void Update()
    {
        controller.inputMove = input.actions["Move"].ReadValue<Vector2>();
        controller.inputJump = input.actions["Jump"].ReadValue<float>() > 0;
    }
}
