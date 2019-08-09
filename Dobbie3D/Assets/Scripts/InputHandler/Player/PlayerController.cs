using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]


public class PlayerController : ActiveCharacter
{
    public Rigidbody rBody;
    private Animator myAnim;
    private InputHandler inputHandler;

    private float rotateSpeed = 13;
    private float pMoveSpeed = 3;
    private float pMaxSpeed = 3;
    private float runSpeed = 6;
    private float walkSpeed = 3;


    private float currentAcceleration = 0f;
    private float topAcceleration = 300f;

    private bool isAccelerating = false;

    private Quaternion m_Quaternion = Quaternion.identity;

    public enum MovementType
    {
        TopDown, FP
    };
    public MovementType actualMovementType;


    void Awake()
    {
        rBody = GetComponent<Rigidbody>();
        myAnim = GetComponent<Animator>();
    }

    #region Movement
    public void MovePlayer(MovementType mType, Vector3 movement)
    {
        if (isAccelerating == true)
        {
            ProcessAcceleration();
        }

        switch (mType)
        {
            case MovementType.TopDown:
                MoveTopDownPlayer(movement);
                break;
            case MovementType.FP:
                MoveFPlayer(movement);
                break;
            default:
                break;
        }

        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(movement), Time.deltaTime * rotateSpeed);
    }

    private void ProcessAcceleration ()
    {
        pMoveSpeed *= currentAcceleration;
        currentAcceleration++;
        if (currentAcceleration >= topAcceleration)
        {
            isAccelerating = false;
            pMoveSpeed = pMaxSpeed;
            currentAcceleration = 0;
        }
    }

    private void MoveTopDownPlayer(Vector3 movement)
    {
        Vector3 myMovement = movement * pMoveSpeed * Time.fixedDeltaTime;
        rBody.MovePosition(rBody.position + myMovement);
    }

    private void MoveFPlayer(Vector3 movement)
    {
        Vector3 myMovement = movement * pMoveSpeed * Time.deltaTime;
        Vector3 newPosition = rBody.position + rBody.transform.TransformDirection(myMovement);
        rBody.MovePosition(newPosition);
    }

    #endregion

    public float GetPlayerSpeed()
    {
        return pMoveSpeed;
    }

    public float GetRotateSpeed()
    {
        return rotateSpeed;
    }

    public void AugmentSpeed()
    {
        pMoveSpeed = runSpeed;
        pMaxSpeed = runSpeed;
    }

    public void DiminishSpeed()
    {
        pMoveSpeed = walkSpeed;
        pMaxSpeed = walkSpeed;
    }

}
