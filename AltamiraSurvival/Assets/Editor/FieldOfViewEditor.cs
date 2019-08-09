using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fow = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.GetViewRadius());
        Vector3 viewAngleA = fow.DirectionFromAngle(-fow.GetViewAngle() / 2, false);
        Vector3 viewAngleB = fow.DirectionFromAngle(fow.GetViewAngle() / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.GetViewRadius());
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.GetViewRadius());

        foreach (Transform visibleTransform in fow.visibleTargets)
        {
            Handles.DrawLine(fow.transform.position, visibleTransform.position);
            Handles.color = Color.red;
        }
    }
}
