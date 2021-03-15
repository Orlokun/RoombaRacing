using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour
{
    List<WheelCollider> wColliders;
    List<GameObject> wObjects;
    Dictionary<WheelCollider, GameObject> wColMeshDictionary;


    float torqueForce = 200f;

    [SerializeField]GameObject wheelCollidersParent;
    [SerializeField] GameObject wMeshesParentObject;

    //Steering//
    float maxSteerAngle = 30f;

    #region AwakeFunctions

    private void Awake()
    {
        Initializers();
        GetWheelColliders();
        GetWheelMeshes();
        GenerateWheelDictionary();
    }

    void Initializers()
    {
        wColliders = new List<WheelCollider>();
        wObjects = new List<GameObject>();
        wColMeshDictionary = new Dictionary<WheelCollider, GameObject>();
    }

    void GetWheelColliders()
    {
        Transform t = wheelCollidersParent.transform;
        for (int i = 0;i<t.childCount;i++)
        {
            wColliders.Add(t.GetChild(i).GetComponent<WheelCollider>());
        }
    }

    void GetWheelMeshes()
    {
        Transform t = wMeshesParentObject.transform;
        for (int i = 0; i < t.childCount; i++)
        {
            wObjects.Add(t.GetChild(i).gameObject);
        }
    }

    void GenerateWheelDictionary()
    {
        for (int i = 0; i<wColliders.Count;i++)
        {
            wColMeshDictionary.Add(wColliders[i], wObjects[i]);
        }
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        float acceleration = Input.GetAxis("Vertical");
        float steering = Input.GetAxis("Horizontal");
        Go(acceleration, steering);
    }

    void Go (float accel, float steering)
    {
        foreach (WheelCollider wCol in wColliders)
        {
            AddForceToWheel(wCol, accel);
            Steer(wCol, steering);
            RotateWheelMeshTorque(wCol);
        }
    }
    
    void Steer(WheelCollider wCol, float steering)
    {
        steering = Mathf.Clamp(steering, -1, 1) * maxSteerAngle;
        wCol.steerAngle = steering;
    }

    void AddForceToWheel(WheelCollider wCol, float accel)
    {
        accel = Mathf.Clamp(accel, -1, 1);
        float thrustTorque = accel * torqueForce;
        wCol.motorTorque = thrustTorque;
    }
    void RotateWheelMeshTorque(WheelCollider wCol)
    {
        Quaternion quat;
        Vector3 position;
        wCol.GetWorldPose(out position, out quat);

        GameObject mObj;
        wColMeshDictionary.TryGetValue(wCol, out mObj);
        mObj.transform.position = position;
        mObj.transform.rotation = quat;
    }

}
