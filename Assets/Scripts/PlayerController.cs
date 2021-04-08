using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : HoverController
{
        
    void FixedUpdate()
    {
        base.FixedUpdate();
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        InputMovement(vertical, horizontal);
    }
}
