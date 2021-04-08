using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitWaypoints : MonoBehaviour
{
    private List<GameObject> _wayPoints;

    private void Awake()
    {
        _wayPoints = new List<GameObject>();
        AddWayPoints();
    }

    private void AddWayPoints()
    {
        for (int i = 0; i<this.transform.childCount;i++)
        {
            _wayPoints.Add(this.transform.GetChild(i).gameObject);
        }
    }
    private void OnDrawGizmos()
    {
        DrawGizmos(false);
    }

    private void OnDrawGizmosSelected()
    {
        DrawGizmos(true);
    }

    private void DrawGizmos(bool selected)
    {
        if (selected == false) { return; }
        if (_wayPoints.Count > 1)
        {
            Vector3 preWp = _wayPoints[0].transform.position;
            for (int i = 0; i < _wayPoints.Count; i++)
            {
                Vector3 nextWp = _wayPoints[i].transform.position;
                Gizmos.DrawLine(preWp,nextWp);
                preWp = nextWp;
            }
            Gizmos.DrawLine(preWp, _wayPoints[0].transform.position);
        }
    }

    public List<GameObject> GetWayPoints()
    {
        return _wayPoints;
    }
}
