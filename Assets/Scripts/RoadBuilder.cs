using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

[ExecuteInEditMode]
public class RoadBuilder : MonoBehaviour
{
    public int BezierCurveDegree = 4;

    [SerializeField] 
    public List<Vector3> LeftSideBezierCurvePoints;
    
    [SerializeField] 
    public List<Vector3> RightSideBezierCurvePoints;

    public int LineDrawSubstep = 256;

    private float LeftLineLength;
    private float RightLineLength;

    public void CreatePoints()
    {
        LeftSideBezierCurvePoints = new List<Vector3>();
        RightSideBezierCurvePoints = new List<Vector3>();
        
        for (int i = 0; i < BezierCurveDegree; ++i)
        {
            Vector3 NewPointLocation = new Vector3(0.0f, 0.0f, 10.0f * (i % BezierCurveDegree));

            LeftSideBezierCurvePoints.Add(NewPointLocation);

            NewPointLocation.x += 10.0f;
            
            RightSideBezierCurvePoints.Add(NewPointLocation);
        }

        Assert.IsTrue(LeftSideBezierCurvePoints.Count == RightSideBezierCurvePoints.Count);
    }

    public void ClearPoints()
    {
        LeftSideBezierCurvePoints.Clear();
        RightSideBezierCurvePoints.Clear();
    }

    public void OnAnyLeftPointMoved()
    {
        CalculateLeftBezierCurveLengths();
    }
    
    public void OnAnyRightPointMoved()
    {
        CalculateRightBezierCurveLengths();
    }
    
    private void CalculateLeftBezierCurveLengths()
    {
        LeftLineLength = BezierCurveHelper.CalculateBezierCurveLength(LeftSideBezierCurvePoints);
    }

    private void CalculateRightBezierCurveLengths()
    {
        RightLineLength = BezierCurveHelper.CalculateBezierCurveLength(RightSideBezierCurvePoints);
    }
    
    private void OnDrawGizmos()
    {
        if (LeftSideBezierCurvePoints.Count == 0 || Selection.activeObject != gameObject)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (LeftSideBezierCurvePoints.Count != 0)
        {
            Tools.current = Tool.None;
        }

        Gizmos.color = Color.green;
        DrawWireSphereAtBezierCurveControlPoints(LeftSideBezierCurvePoints);
        BezierCurveHelper.DrawBezierCurveOnGizmo(LeftSideBezierCurvePoints, LineDrawSubstep);
        
        Gizmos.color = Color.blue;
        DrawWireSphereAtBezierCurveControlPoints(RightSideBezierCurvePoints);
        BezierCurveHelper.DrawBezierCurveOnGizmo(RightSideBezierCurvePoints, LineDrawSubstep);
    }

    private void DrawWireSphereAtBezierCurveControlPoints(List<Vector3> ControlPoints, float Radius = 0.4f)
    {
        foreach (Vector3 CurrentControlPoint in ControlPoints)
        {
            Gizmos.DrawWireSphere(CurrentControlPoint, Radius);
        }
    }
}