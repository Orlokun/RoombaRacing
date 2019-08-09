using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCameraController : MonoBehaviour
{
    private Transform targetTransform;
    private float distanceY = 10f;
    private Quaternion myQuaternion;

    public enum CameraState
    {
        Normal, MovingToTarget, PauseMode
    };

    private CameraState actualCamState;


    void Start()
    {
        targetTransform = FindObjectOfType<PlayerController>().GetComponent<Transform>();
        actualCamState = CameraState.Normal;
    }

    // Update is called once per frame
    void Update()
    {
        switch (actualCamState)
        {
            case CameraState.Normal:
                RegularMovement();
                break;
            case CameraState.MovingToTarget:
                Debug.Log("nothing");
                break;
            case CameraState.PauseMode:
                Debug.Log("nothing");
                break;
        }
    }

    public void ChangeCameraState(CameraState cameraState)
    {

    }

    private void RegularMovement()
    {
        transform.position = new Vector3(targetTransform.position.x, targetTransform.position.y + distanceY, 
                                            targetTransform.position.z);
        Vector3 targetDirection = targetTransform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(targetDirection);
    }
}
