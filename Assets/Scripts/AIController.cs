using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class AIController : HoverController
{
    public CircuitWaypoints circuitPoints;
    public float steeringSen = 0.01f;
    
    //Variables For Straight Line target Prioritization
    private List<GameObject> objectsInStraightLine;
    private float horizontalCoverage;
    private float zAxisCoverage;
    
    private Vector3 _target;

    private void Start()
    {
        GetGameObjectsInStraightLine();
        FindClosestObjectWithinObjectsInLine();
    }

    private void GetGameObjectsInStraightLine()
    {
        GamObject[ g]
    }
    
    void FindClosestObjectWithinObjectsInLine()
    {
        float previousDist = 0;
        for (int i = 0; i < circuitPoints.GetWayPoints().Count;i++)
        {
            var distance = Vector3.Distance(hoverBody.transform.position, circuitPoints.GetWayPoints()[i].transform.position);
            if (i == 0)
            {
                previousDist = distance;
            }
            else if (distance < previousDist)
            {
                previousDist = distance;
            }
        }
        _target = circuitPoints.GetWayPoints()[_currentWayPoint].transform.position;
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        SetSteerAndAccel();
    }

    void SetSteerAndAccel() //Needs to be refactored
    {
        var localTarget = hoverBody.gameObject.transform.InverseTransformPoint(_target);
        var targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        var steer = Mathf.Clamp(targetAngle * steeringSen, -1, 1) * Mathf.Sign(CurrentSpeed);

        var distToTarget = Vector3.Distance(_target, hoverBody.transform.position);
        var accel = 1f;
        var _breakForce = 0f;

        InputMovement(accel, steer);
    }
}
