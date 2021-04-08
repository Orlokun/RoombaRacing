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
    private List<GameObject> _wMeshObjects;
    private Dictionary<WheelCollider, GameObject> _wColMeshDictionary;
    private Transform[] _skidTransforms;
    private List<Light> _brakeLights;

    public SteeringStyle actualSteerStyle;
    public AccelerationStyle actualAccelStyle;

    [SerializeField] private GameObject wheelCollidersParent;
    [SerializeField] private GameObject wMeshesParentObject;
    [SerializeField] private Transform wheelPrefabMark;
    [SerializeField] private GameObject carBody;
    [SerializeField] private AudioSource highAccel; 
    //[SerializeField] ParticleSystem wheelSmokePrefab;
    //private ParticleSystem[] smokeParticles;
    [SerializeField] bool hasVisibleWheels;
    
    //AccelElements
    [SerializeField] Rigidbody rBody;
    [SerializeField] float gearLength = 3;
    public float CurrentSpeed => rBody.velocity.magnitude * gearLength;
    public float lowPitch = 1f;
    public float highPitch = 6f;
    public int numberOfGears = 5;
    private float _rpm;
    private int _currentGear = 1;
    private float _currentGearPerc;
    public float maxSpeed = 200;
    

    //Steering//
    private const float MaxSteerAngle = 45f;
    //Accelerating
    private const float TorqueForce = 1000f;
    //Breaking
    private const float BreakTorque = 10000f;

    #region AwakeFunctions
    private void Awake()
    {
        SafetyCheck();
        Initializers();
        GetWheelColliders();
        if (hasVisibleWheels)
        {
            GetWheelMeshes();
            GenerateWheelDictionary();
        }
        GetBreakLights();
    }
    void SafetyCheck()
    {
        if (wheelCollidersParent == null || wMeshesParentObject == null || wheelPrefabMark == null || carBody == null)
        {
            Debug.LogError("ONE OF YOUR SET IN EDITOR OBJECTS IS NOT PLACED");
        }
    }
    void Initializers()
    {
        _wColliders = new List<WheelCollider>();
        _wMeshObjects = new List<GameObject>();
        _wColMeshDictionary = hasVisibleWheels ? new Dictionary<WheelCollider, GameObject>() : null;
        _brakeLights = new List<Light>();
    }
    void GetWheelColliders()
    {
        Transform t = wheelCollidersParent.transform;
        for (int i = 0; i < t.childCount; i++)
        {
            _wColliders.Add(t.GetChild(i).GetComponent<WheelCollider>());
        }
        _skidTransforms = new Transform[_wColliders.Count];
    }
    void GetWheelMeshes()
    {
        Transform t = wMeshesParentObject.transform;
        for (int i = 0; i < t.childCount; i++)
        {
            _wMeshObjects.Add(t.GetChild(i).gameObject);
        }
    }
    void GenerateWheelDictionary()
    {
        if (!hasVisibleWheels) return;
        for (var i = 0; i < _wColliders.Count; i++)
        {
            _wColMeshDictionary.Add(_wColliders[i], _wMeshObjects[i]);
        }
    }
    void GetBreakLights()
    {
        for (var i = 0; i < carBody.transform.childCount; i++)
        {
            if (!carBody.transform.GetChild(i).GetComponent<Light>()) continue;
            var myLight = carBody.transform.GetChild(i).GetComponent<Light>();
            _brakeLights.Add((myLight));
        }
    }

    #endregion

    #region UpdateFunctions
    public void Go(float accel, float steering, float breakForce)
    {
        for (int i = 0; i < _wColliders.Count; i++)
        {
            AddForceToWheel(actualAccelStyle, _wColliders[i], accel, i);
            Steer(actualSteerStyle, _wColliders[i], steering, i);
            RotateWheelMeshTorque(_wColliders[i]);
            WheelBreak(_wColliders[i], breakForce, i);
            CheckSkid(_wColliders[i]);
        }
    }

    void Steer(SteeringStyle steerStyle, WheelCollider wCol, float steering, int wheelId)
    {
        switch (steerStyle)
        {
            case SteeringStyle.FourWheel:
                if (wheelId < 2)
                {
                    steering = Mathf.Clamp(steering, -1, 1) * MaxSteerAngle;
                    wCol.steerAngle = steering;
                }
                else
                {
                    steering = Mathf.Clamp(steering * -1, -1, 1) * MaxSteerAngle;
                    wCol.steerAngle = steering;
                }
                break;
            case SteeringStyle.FrontWheel:
                if (wheelId < 2)
                {
                    steering = Mathf.Clamp(steering, -1, 1) * MaxSteerAngle;
                    wCol.steerAngle = steering;
                }
                break;
            case SteeringStyle.RearWheel:
                if (wheelId > 1)
                {
                    steering = Mathf.Clamp(steering * -1, -1, 1) * MaxSteerAngle;
                    wCol.steerAngle = steering;
                }
                break;
            default:
                return;
        }
    }

    void AddForceToWheel(AccelerationStyle accelStyle, WheelCollider wCol, float accel, int wheelId)
    {
        float thrustTorque = 0;
        if (CurrentSpeed < maxSpeed)
        {
            thrustTorque = accel * TorqueForce;
        }
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
                mObj.transform.localRotation = quat;
            }
        }
    }

    void WheelBreak(WheelCollider wCol, float breakForce, int wheelId)
    {
        if (wheelId > 1)
        {
            breakForce = Mathf.Clamp(breakForce, -1, 1) * BreakTorque;
            wCol.brakeTorque = breakForce;
            if (breakForce != 0)
                SetBrakeLightsIntensity(1.8f);
            else
            {
                SetBrakeLightsIntensity(1f);
            }
        }
    }

    void SetBrakeLightsIntensity(float intensity)
    {
        if (_brakeLights[0].intensity == intensity) return;
        foreach (var myLight in _brakeLights)
        {
            myLight.intensity = intensity;
        }
    }
    void CheckSkid(WheelCollider wCol)
    {
        wCol.GetGroundHit(out var wHit);
        if (Mathf.Abs(wHit.forwardSlip) > 0.4f || Mathf.Abs(wHit.sidewaysSlip) > 0.4f)
        {
            //TODO: Implement Audio System
        }
        else
        {
            //TODO: Deactivate Soud
        }
    }
    
    public void CalculateEngineSound()
    {
        float gearPerc = (1 / (float)numberOfGears);
        float targetGearFactor = Mathf.InverseLerp(gearPerc * _currentGear, gearPerc * _currentGear + 1,
            Mathf.Abs(CurrentSpeed / maxSpeed));
        _currentGearPerc = Mathf.Lerp(_currentGearPerc, targetGearFactor, Time.deltaTime * 5f);

        var gearNumFactor = (_currentGear / (float)numberOfGears);
        _rpm = Mathf.Lerp(gearNumFactor, 1, _currentGearPerc);

        float speedPerc = Mathf.Abs(CurrentSpeed / maxSpeed);
        float upperGearMax = (1 / (float) numberOfGears) * (_currentGear + 1);
        float downGearMax = (1 / (float) numberOfGears) * (_currentGear);

        if (_currentGear > 0 && speedPerc < downGearMax)
        {
            _currentGear--;
        }

        if (speedPerc > upperGearMax && _currentGear < (numberOfGears - 1))
        {
            _currentGear++;
        }

        float pitch = Mathf.Lerp(lowPitch, highPitch, _rpm);
        highAccel.pitch = Mathf.Min(highPitch, pitch) * 0.25f;
    }

    #endregion


    #region Getters&Setters

    public void SetState(SteeringStyle _style)
    {
        actualSteerStyle = _style;
    }

    public Rigidbody GetRBody()
    {
        return rBody;
    }
    #endregion

}
