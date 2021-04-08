using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour
{
    public Transform Target;
    public float SmoothSpeed = 4f;

    public float MouseSensivity = 1f;

	
	void FixedUpdate ()
    {
        transform.position = Vector3.Lerp(transform.position, Target.position, Time.fixedDeltaTime * SmoothSpeed);

        float mouseX = Input.GetAxis("Mouse X");

        transform.RotateAround(Target.position, Vector3.up, mouseX * MouseSensivity);
	}
}
