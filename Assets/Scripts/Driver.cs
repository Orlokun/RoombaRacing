using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region AccelSteerStyles

public enum AccelerationStyle
{
    FrontWheel,
    RearWheel,
    FourWheel, //Do not Use. Is ont Working Properly
}

public enum SteeringStyle
{
    FrontWheel,
    RearWheel,
    FourWheel, //Do not Use. Is ont Working Properly
}

#endregion

public class Driver : MonoBehaviour
{
    private List<WheelCollider> _wColliders;
    private List<GameObject> _wObjects;
    private Dictionary<WheelCollider, GameObject> _wColMeshDictionary;

    public SteeringStyle actualSteerStyle;
    public AccelerationStyle actualAccelStyle;

    [SerializeField] GameObject wheelCollidersParent;
    [SerializeField] GameObject wMeshesParentObject;

    [SerializeField] bool hasVisibleWheels;


    //Steering//
    float maxSteerAngle = 30f;

    //Accelerating
    float torqueForce = 200f;

    //Breaking
    float breakTorque = 500f;

    #region AwakeFunctions

    private void Awake()
    {
        Initializers();
        GetWheelColliders();
        if (hasVisibleWheels)
        {
            GetWheelMeshes();
            GenerateWheelDictionary();
        }
    }

    void Initializers()
    {
        _wColliders = new List<WheelCollider>();
        _wObjects = new List<GameObject>();
        _wColMeshDictionary = hasVisibleWheels ? new Dictionary<WheelCollider, GameObject>() : null;
    }

    void GetWheelColliders()
    {
        Transform t = wheelCollidersParent.transform;
        for (int i = 0; i < t.childCount; i++)
        {
            _wColliders.Add(t.GetChild(i).GetComponent<WheelCollider>());
        }
    }

    void GetWheelMeshes()
    {
        Transform t = wMeshesParentObject.transform;
        for (int i = 0; i < t.childCount; i++)
        {
            _wObjects.Add(t.GetChild(i).gameObject);
        }
    }

    void GenerateWheelDictionary()
    {
        if (!hasVisibleWheels) return;
        for (int i = 0; i < _wColliders.Count; i++)
        {
            _wColMeshDictionary.Add(_wColliders[i], _wObjects[i]);
        }
    }

    #endregion

    #region UpdateFunctions

    void FixedUpdate()
    {
        float acceleration = Input.GetAxis("Vertical");
        float steering = Input.GetAxis("Horizontal");
        float breaking = Input.GetAxis("Jump");
        Go(acceleration, steering, breaking);
    }

    void Go(float accel, float steering, float breakForce)
    {
        for (int i = 0; i < _wColliders.Count; i++)
        {
            AddForceToWheel(actualAccelStyle, _wColliders[i], accel, i);
            Steer(actualSteerStyle, _wColliders[i], steering, i);
            RotateWheelMeshTorque(_wColliders[i]);
            WheelBreak(_wColliders[i], breakForce, i);
        }
    }

    void Steer(SteeringStyle steerStyle, WheelCollider wCol, float steering, int wheelId)
    {
        switch (steerStyle)
        {
            case SteeringStyle.FourWheel:
                if (wheelId < 2)
                {
                    steering = Mathf.Clamp(steering, -1, 1) * maxSteerAngle;
                    wCol.steerAngle = steering;
                }
                else
                {
                    steering = Mathf.Clamp(steering * -1, -1, 1) * maxSteerAngle;
                    wCol.steerAngle = steering;
                }

                break;
            case SteeringStyle.FrontWheel:
                if (wheelId < 2)
                {
                    steering = Mathf.Clamp(steering, -1, 1) * maxSteerAngle;
                    wCol.steerAngle = steering;
                }

                break;
            case SteeringStyle.RearWheel:
                if (wheelId > 1)
                {
                    steering = Mathf.Clamp(steering * -1, -1, 1) * maxSteerAngle;
                    wCol.steerAngle = steering;
                }

                break;
            default:
                return;
        }
    }

    void AddForceToWheel(AccelerationStyle accelStyle, WheelCollider wCol, float accel, int wheelId)
    {
        var thrustTorque = accel * torqueForce;
        switch (accelStyle)
        {
            case AccelerationStyle.FourWheel:
                accel = Mathf.Clamp(accel, -1, 1);
                wCol.motorTorque = thrustTorque;
                break;
            case AccelerationStyle.FrontWheel:
                if (wheelId < 2)
                {
                    accel = Mathf.Clamp(accel, -1, 1);
                    wCol.motorTorque = thrustTorque;
                }

                break;
            case AccelerationStyle.RearWheel:
                if (wheelId > 1)
                {
                    accel = Mathf.Clamp(accel, -1, 1);
                    wCol.motorTorque = thrustTorque;
                }

                break;
            default:
                return;
        }
    }

    void RotateWheelMeshTorque(WheelCollider wCol)
    {
        if (hasVisibleWheels)
        {
            wCol.GetWorldPose(out var position, out var quat);
            _wColMeshDictionary.TryGetValue(wCol, out var mObj);
            if (mObj != null)
            {
                mObj.transform.position = position;
                mObj.transform.rotation = quat;
            }
        }
    }

    void WheelBreak(WheelCollider wCol, float breakForce, int wheelId)
    {
        if (wheelId > 1)
        {
            breakForce = Mathf.Clamp(breakForce, -1, 1) * breakTorque;
            wCol.brakeTorque = breakForce;
        }
    }

    void CheckSkid(WheelCollider wCol, int wheelId)
    {
        int skidNumber = 0;
        WheelHit wHit;
        wCol.GetGroundHit(out wHit);
        if (Mathf.Abs(wHit.forwardSlip) > 0.4f || Mathf.Abs(wHit.sidewaysSlip) > 0.4f)
        {
            //TODO: Implement Audio System
        }
    }

    #endregion

    public void SetState(SteeringStyle _style)
    {
        actualSteerStyle = _style;
    }
}