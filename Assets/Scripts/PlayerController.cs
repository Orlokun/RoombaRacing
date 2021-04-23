using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : HoverController
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        MovementInput(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        CheckJumpInput(Input.GetButton("Jump"));
        CheckTurboInput(Input.GetKeyDown(KeyCode.LeftShift));
    }



    protected void CheckJumpInput(bool jump)
    {
        if (jump)
        {
            ActivateJump();
        }
    }

}
