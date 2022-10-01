using System.Collections;
using System.Collections.Generic;
using ECM.Controllers;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    public Animator anim;

    public BaseCharacterController controller;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.moveDirection.magnitude > 0)
        {
            anim.SetBool("Moving", true);
        }
        else
        {
            anim.SetBool("Moving", false);
        }
    }
}
