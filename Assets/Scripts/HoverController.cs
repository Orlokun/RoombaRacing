using UnityEngine;
using System.Collections;

public enum HoverAccelState
{
    NormalAccel,
    Turbo,
    Nitro,
}


public class HoverController : MonoBehaviour
{
    #region GlobalInGameVariables

    [SerializeField] private float jumpForce;
    [SerializeField] private float nitroForceMultiplier;
    [SerializeField] private int turboCharges;
    private float turboTime;
    
    #endregion
    
    #region GlobalHoverVariables
    public float GoUpForce;
    public float ForwardForce;
    public float RotationTorque = 10000f;
    public Transform[] RaycastHelpers;
    public Transform CenterRaycastHelper;
    public float HeightFromGround = 2f;
    public float GroundedThreshold = 3f;
    public bool BlockAirControl;

    public LayerMask GroundLayer;

    protected Rigidbody hoverBody;

    bool grounded;
    #endregion

    #region EngineSoundVariables

    private AudioSource playerAudioSource;
    [SerializeField] private float enginePitch;

    [SerializeField] int maxVelMagnitude;
    private float maxPitch = 1.6f;
    public float CurrentSpeed => hoverBody.velocity.magnitude;
    
    #endregion
    
	void Start ()
    {
        hoverBody = GetComponent<Rigidbody>();
        playerAudioSource = GetComponent<AudioSource>();
        playerAudioSource.pitch = enginePitch;
    }
	
	protected virtual void FixedUpdate ()
    {
        CheckGrounded();
        HeightUp();
        CheckSound();
    }

    private void CheckSound()
    {
        playerAudioSource.pitch = GetNecessaryPitch();
    }
    private float GetNecessaryPitch()
    {    
        var accelInput = Mathf.Abs(Input.GetAxis("Vertical")) > 0;
        var pitch = hoverBody.velocity.magnitude / maxVelMagnitude;
        if (pitch < enginePitch)
        {
            pitch = enginePitch;
        }
        if (!grounded && accelInput)
        {
            pitch = Mathf.Lerp(pitch, 1.5f, .1f);
        }
        if (accelInput)
            pitch += .1f;
        
        return pitch += .1f;
    }
    private void CheckGrounded()
    {
        Ray ray = new Ray(CenterRaycastHelper.position, -CenterRaycastHelper.up);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, GroundedThreshold, GroundLayer))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }
    private void HeightUp()
    {
        foreach (Transform raycastHelper in RaycastHelpers)
        {
            Ray ray = new Ray(raycastHelper.position, -raycastHelper.up);
            RaycastHit hitInfo;

            if(Physics.Raycast(ray, out hitInfo, HeightFromGround, GroundLayer))
            {
                float distance = Vector3.Distance(raycastHelper.position, hitInfo.point);

                if(distance < HeightFromGround)
                {
                    hoverBody.AddForceAtPosition(raycastHelper.up * GoUpForce * (1f - distance / HeightFromGround), raycastHelper.position, ForceMode.Force);
                }
            }
        }
    }

    protected void ActivateJump()
    {
        foreach (Transform raycastHelper in RaycastHelpers)
        {
            Ray ray = new Ray(raycastHelper.position, -raycastHelper.up);
            RaycastHit hitInfo;

            if(Physics.Raycast(ray, out hitInfo, HeightFromGround, GroundLayer))
            {
                float distance = Vector3.Distance(raycastHelper.position, hitInfo.point);

                if(distance < HeightFromGround)
                {
                    hoverBody.AddForceAtPosition(raycastHelper.up * GoUpForce * jumpForce, raycastHelper.position, ForceMode.Impulse);
                }
            }
        }
    }
    
    protected void MovementInput(float forward, float side)
    {
        if (!grounded) return;
        hoverBody.AddRelativeForce(Vector3.forward * forward * ForwardForce, ForceMode.Force);
        hoverBody.AddRelativeTorque(Vector3.up * RotationTorque * side * (forward == 0 ? 1f : Mathf.Sign(forward)), ForceMode.Force);
    }

    protected void CheckTurboInput(bool turboInput)
    {
        if (turboInput && turboCharges > 0 && GetRigidBody().transform.rotation.x > -10f) 
        {
            turboCharges--;
            TurboInput();
        }
    }

    protected void TurboInput()
    {
        hoverBody.AddRelativeForce(Vector3.forward * ForwardForce * nitroForceMultiplier, ForceMode.Impulse);
    }
    
    public Rigidbody GetRigidBody()
    {
        return hoverBody;
    }
}
