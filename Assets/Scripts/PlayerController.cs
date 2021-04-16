using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : HoverController
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        InputMovement(vertical, horizontal);
    }
}
