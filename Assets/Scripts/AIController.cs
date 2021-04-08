using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class AIController : MonoBehaviour
{
    public CircuitWaypoints circuitPoints;
    private Driver _driver;
    public float steeringSen = 0.01f;
    private Vector3 _target;
    private int _currentWayPoint = 0;

// Start is called before the first frame update
    private void Start()
    {
        _driver = GetComponent<Driver>();
        _target = circuitPoints.GetWayPoints()[_currentWayPoint].transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetRelativePosition();
    }

    void GetRelativePosition()    //Needs to be refactored
    {
        var localTarget = _driver.GetRBody().gameObject.transform.InverseTransformPoint(_target);
        var distToTarget = Vector3.Distance(_target, _driver.GetRBody().gameObject.transform.position);

        var targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        var steer = Mathf.Clamp(targetAngle * steeringSen, -1, 1) * Mathf.Sign(_driver.CurrentSpeed);
        var accel = 1f;
        var _breakForce = 0f;

        if (distToTarget < 5) { _breakForce = 1; accel = 0; }

        _driver.Go(accel, steer, _breakForce);
        if (distToTarget < 2)
        {
            _currentWayPoint++;
            if (_currentWayPoint >= circuitPoints.GetWayPoints().Count)
            {
                _currentWayPoint = 0;
            }
            _target = circuitPoints.GetWayPoints()[_currentWayPoint].transform.position;
        }
        _driver.CalculateEngineSound();
    }
}
