using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commander
{
    protected GameObject commandedObject;

    protected virtual void Execute(GameObject commandedObject)
    {
        
    }

}

public class JumpCommand : Commander
{
    protected override void Execute(GameObject commandedObject)
    {
        base.Execute(commandedObject);
    }
}

public class MeleeAttack : Commander
{

}

public class MagicAttack : Commander
{
    public GameObject actor;
    protected override void Execute(GameObject gObject)
    {

    }

    private void DoMagicAttack (GameObject gObjec)
    {
        
    }

}

public class MoveCamera : Commander
{

}

public class MoveAround : Commander
{

}

public class CrouchCommand : Commander
{

}