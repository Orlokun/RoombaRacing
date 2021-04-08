using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipCar : MonoBehaviour
{
    private Rigidbody _rBody;
    private Transform _myT;
    private float _lastTimeChecked;
    // Start is called before the first frame update
    void Start()
    {
        _rBody = GetComponent<Rigidbody>();
        _myT = GetComponent<Transform>();
    }

    void RightCar()
    {
        _myT.position += Vector3.up;
        _myT.transform.rotation = Quaternion.LookRotation(_myT.forward);
    }
    void FixedUpdate()
    {
        if (transform.up.y > 0f || _rBody.velocity.magnitude > 1)
        {
            _lastTimeChecked = Time.time;
        }

        if (Time.time > _lastTimeChecked + 3)
        {
            RightCar();
        }
    }
}
