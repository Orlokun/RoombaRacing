using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField]
    private float viewAngle;
    [Range (0,360)]
    [SerializeField]
    private float viewRadius;
    public float meshResolution;
    public MeshFilter viewMeshFilter;
    private Mesh viewMesh; 

    [SerializeField]
    private LayerMask targetMask;
    [SerializeField]
    private LayerMask obstacleMask;
    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();
    public int edgeResolveIterations;

    public float edgDistanceThreshHold; 

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo (Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }


    private void Start()
    {
        StartCoroutine(FindTargetEveryStep(.3f));
        viewMesh = new Mesh();
        viewMesh.name = "ViewMesh";
        viewMeshFilter.mesh = viewMesh;
    }

    IEnumerator FindTargetEveryStep(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    private void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] collidersInViewRadius = Physics.OverlapSphere(transform.position, viewRadius);
        for (int i = 0; i<collidersInViewRadius.Length; i++)
        {
            Transform target = collidersInViewRadius[i].GetComponent<Transform>();
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle/2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    public Vector3 DirectionFromAngle(float anglesInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            anglesInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(anglesInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(anglesInDegrees * Mathf.Deg2Rad));
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    public void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo lastViewCast = new ViewCastInfo();


        for (int i = 0; i<= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            Debug.DrawLine(transform.position, transform.position + DirectionFromAngle(angle, true) 
                                                                        * viewRadius, Color.red);
            ViewCastInfo newViewCast = ViewCast(angle);
            if (i>0)
            {
                bool edgeDistanceThreshHoldExceeded = Mathf.Abs(lastViewCast.distance - newViewCast.distance) > edgDistanceThreshHold;
                if  (lastViewCast.hit != newViewCast.hit || (lastViewCast.hit&&newViewCast.hit && edgeDistanceThreshHoldExceeded))
                {
                    EdgeInfo edge = FindEdge(lastViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }
            viewPoints.Add(newViewCast.hitPoint);
            lastViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount -2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i<vertexCount-1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if (i < vertexCount -2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge (ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;

        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i<edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);
            bool edgeDistanceThreshHoldExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgDistanceThreshHold;

            if (newViewCast.hit == minViewCast.hit && !edgeDistanceThreshHoldExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.hitPoint; 
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.hitPoint;
            }
        }
        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 direction = DirectionFromAngle(globalAngle, true);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + direction * viewRadius, viewRadius, globalAngle);
        }
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 hitPoint;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _hitPoint, float _distance, float _angle)
        {
            hit = _hit;
            hitPoint = _hitPoint;
            distance = _distance;
            angle = _angle; 
        }
    }

    public float GetViewRadius()
    {
        return viewRadius;
    }

    public float GetViewAngle()
    {
        return viewAngle;
    }
}
