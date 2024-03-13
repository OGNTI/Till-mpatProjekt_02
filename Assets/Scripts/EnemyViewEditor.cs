using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyViewController))]
public class EnemyViewEditor : Editor
{
    void OnSceneGUI()
    {
        EnemyViewController evc = (EnemyViewController)target;

        Vector3 viewAngleA = evc.PosFromAngle(-evc.viewAngle / 2);
        Vector3 viewAngleB = evc.PosFromAngle(evc.viewAngle / 2);

        Handles.color = Color.green;

        //Draw sight lines
        Handles.DrawLine(evc.transform.position, evc.transform.position + viewAngleA * evc.viewRange);
        Handles.DrawLine(evc.transform.position, evc.transform.position + viewAngleB * evc.viewRange);

        //Draw viewRange
        Handles.DrawWireArc(evc.transform.position, Vector3.up, viewAngleA, evc.viewAngle, evc.viewRange);
        //Draw attackRange
        Handles.DrawWireArc(evc.transform.position, Vector3.up, viewAngleA, evc.viewAngle, evc.attackRange);
    }
}
