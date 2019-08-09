using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputHandler : MonoBehaviour
{
    #region GlobalData
    private PlayerController pController;
    Transform headTransform;
    private Camera mainCamera;

    private List<Rigidbody> bodiesInMotionWithPlayer = new List<Rigidbody>();
    private Vector3 movementAmount = new Vector3(0f, 0f, 0f);

    [SerializeField]
    private float mouseVelocity;
    private float mouseSensitivity = 1500;
    private float xAxisClamp;
    Vector2 smoothedVelocity;
    Vector2 currentLookingPosition;

    private Vector2 camMouseLook;
    private Vector2 smoothV;
    public float sensitivity = 2f;
    public float smoothing = 2f;

    public enum PerspectiveHandler
    {
        FirstPersonMouse, FirstPersonJoyStick, TopDownMouse, TopDownJS
    };

    public PerspectiveHandler actualCameraType;

    #endregion

    #region Awake
    private void Awake()
    {
        pController = FindObjectOfType<PlayerController>();
        mainCamera = Camera.main;
        SetCameraTypeData();
        headTransform = FindObjectOfType<HeadComponent>().transform;
    }

    private void SetCameraTypeData()
    {
        switch (actualCameraType)
        {
            case PerspectiveHandler.TopDownMouse:
                PositionCam4MouseTopDown();
                break;
            case PerspectiveHandler.TopDownJS:
                PositionMouse4JSTopDown();
                break;
            case PerspectiveHandler.FirstPersonMouse:
                PositionMouse4MouseFP();
                break;
            case PerspectiveHandler.FirstPersonJoyStick:
                PositionMouse4JSFP();
                break;
            default:
                break;
        }
    }

    void PositionCam4MouseTopDown()
    {
        mainCamera.gameObject.AddComponent<TopDownCameraController>();
        mainCamera.transform.parent = null;
        mainCamera.transform.LookAt(pController.transform);
    }

    void PositionMouse4MouseFP()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    void PositionMouse4JSTopDown()
    {

    }

    void PositionMouse4JSFP()
    {

    }


    #endregion


    #region Update
    void FixedUpdate()
    {
        ProcessRotationAndView();
        ProcessPlayerMovement();
        ProcessPlayerActions();

        if (Input.GetKeyDown("escape"))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void ProcessRotationAndView()
    {
        switch (actualCameraType)
        {
            case PerspectiveHandler.TopDownMouse:
                ProcessRotationFromTopViewMouse();
                break;
            case PerspectiveHandler.TopDownJS:
                ProcessRotationFromTopViewJoystick();
                break;
            case PerspectiveHandler.FirstPersonMouse:
                ProcessFirstPersonMouseControll();
                break;
            case PerspectiveHandler.FirstPersonJoyStick:
                ProcessFirstPersonJoyStickCamControll();
                break;
            default:
                break;
        }
    }

    void ProcessRotationFromTopViewMouse()
    {
        mainCamera.transform.position = new Vector3(pController.transform.position.x, pController.transform.position.y + 10f, pController.transform.position.z);
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                                                                Input.mousePosition.y, mainCamera.transform.position.y));
        pController.transform.LookAt(mousePosition);
    }

    void ProcessRotationFromTopViewJoystick()
    {
        Vector3 joyStickInput = new Vector3(Input.GetAxisRaw("RJHorizontal"), 0, Input.GetAxisRaw("RJVertical"));
        joyStickInput.Normalize();
        Debug.Log(joyStickInput);
        if (joyStickInput.x >= .4f || joyStickInput.x <= -.4f || joyStickInput.z >= .4f || joyStickInput.z <= -.4f)
        {
            pController.transform.rotation = Quaternion.Lerp(pController.transform.rotation, Quaternion.LookRotation(joyStickInput), Time.deltaTime * pController.GetRotateSpeed());
        }
    }

    void ProcessFirstPersonMouseControll()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Vector2 inputValues = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        inputValues = Vector2.Scale(inputValues, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        smoothedVelocity.x = Mathf.Lerp(smoothedVelocity.x, inputValues.x, 1f / smoothing);
        smoothedVelocity.y = Mathf.Lerp(smoothedVelocity.y, inputValues.y, 1f / smoothing);

        currentLookingPosition += smoothedVelocity;

        if (currentLookingPosition.y > 90f)
        {
            currentLookingPosition.y = 90f;
            ClampXRotationValue(270f);
        }

        if (currentLookingPosition.y < -90f)
        {
            currentLookingPosition.y = -90f;
            ClampXRotationValue(90f);
        }

        headTransform.localRotation = Quaternion.AngleAxis(-currentLookingPosition.y, Vector3.right);
        pController.transform.localRotation = Quaternion.AngleAxis(currentLookingPosition.x, pController.transform.up);

    }

    void ProcessFirstPersonJoyStickCamControll()
    {

    }

    void ClampXRotationValue(float value)
    {
        Vector3 eulerRotation = headTransform.eulerAngles;
        eulerRotation.x = value;
        headTransform.eulerAngles = eulerRotation;
    }

    #endregion

    #region Movement
    void ProcessPlayerMovement()
    {
        switch (actualCameraType)
        {
            case PerspectiveHandler.FirstPersonJoyStick:
                JSFPMovement();
                break;
            case PerspectiveHandler.FirstPersonMouse:
                MouseFPMovement();
                break;
            case PerspectiveHandler.TopDownJS:
                JSTopDownMovement();
                break;
            case PerspectiveHandler.TopDownMouse:
                MouseTopDopDownMovement();
                break;
            default:
                break;
        }

    }
    void JSFPMovement()
    {

    }

    void MouseFPMovement()
    {
        movementAmount = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 direction = movementAmount.normalized;
        pController.MovePlayer(PlayerController.MovementType.FP, direction);
        ProcessObjectsInMovementWithPlayer(direction);
    }

    void JSTopDownMovement()
    {

    }

    void MouseTopDopDownMovement()
    {
        movementAmount = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 direction = movementAmount.normalized;
        pController.MovePlayer(PlayerController.MovementType.TopDown, direction);
    }

    void ProcessObjectsInMovementWithPlayer(Vector3 direction)
    {
        if (bodiesInMotionWithPlayer.Count > 0)
        {
            foreach (Rigidbody rBody in bodiesInMotionWithPlayer)
            {
                float pMoveSpeed = pController.GetPlayerSpeed();
                rBody.MovePosition(transform.position + (direction * pMoveSpeed * Time.deltaTime));
            }
        }

    }
    #endregion

    #region Actions

    void ProcessPlayerActions()
    {
        CheckIfRunning();
    }

    void CheckIfRunning()
    {
        if (Input.GetAxis("Running") != 0)
        {
            pController.AugmentSpeed();
        }
        else
        {
            pController.DiminishSpeed();
        }
    }
    #endregion

}