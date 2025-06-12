using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

enum SplineDivideType
{
    DivideByCount,
    DivideByDistance
}

[Serializable]
public struct PointOnBezierCurveData
{
    public PointOnBezierCurveData(Vector3 InPosition)
    {
        Distance = 0.0f;
        Alpha = 0.0f;
        
        WorldPosition = InPosition;
    }
    
    public float Distance;

    public float Alpha;

    public Vector3 WorldPosition;
}

public class SplineComponent : MonoBehaviour
{
    public int BezierCurveDegree = 4;

    public int Accuracy = 512;

    public Color GizmoDrawColor = Color.green;
    
    public List<Vector3> BezierCurveControlPoints = new List<Vector3>();
    
    public List<PointOnBezierCurveData> AllPointData = new List<PointOnBezierCurveData>();
    
    public UnityAction OnAnyControlPointMovedAction;

    private SplineDivideType LastDivideType;

    private int LastDivideCount;
    private float LastDivideDistance;

    public void Init(int InBezierCurveDegree, int InAccuracy, Color InGizmoDrawColor)
    {
        BezierCurveDegree = InBezierCurveDegree;
        Accuracy          = InAccuracy;
        
        GizmoDrawColor    = InGizmoDrawColor;
    }
    public void CreateControlPoints(Vector3 InitialPosition)
    {
        ClearControlPoints();
        
        for (int i = 0; i < BezierCurveDegree; ++i)
        {
            Vector3 NewPointLocation = InitialPosition + new Vector3(0.0f, 0.0f, 10.0f * (i % BezierCurveDegree));

            BezierCurveControlPoints.Add(NewPointLocation);
        }
    }

    public void ClearControlPoints()
    {
        BezierCurveControlPoints.Clear();
    }

    public bool HasAnyPoints()
    {
        return BezierCurveControlPoints.Count > 0;
    }

    public void OnAnyControlPointMoved()
    {
        switch (LastDivideType)
        {
            case SplineDivideType.DivideByCount:
                DividePointsByCount(LastDivideCount);
                break;
            case SplineDivideType.DivideByDistance:
                DividePointsByDistance(LastDivideDistance);
                break;
        }
        
        OnAnyControlPointMovedAction?.Invoke();
    }

    public void DividePointsByCount(int NodeCount)
    {
        LastDivideType = SplineDivideType.DivideByCount;
        LastDivideCount = NodeCount;
        
        float TotalLength = BezierCurveHelper.CalculateBezierCurveLength(BezierCurveControlPoints);

        DividePointsByEqualDistance(Mathf.Floor(TotalLength / NodeCount));
    }
    
    public void DividePointsByDistance(float Distance)
    {
        LastDivideType = SplineDivideType.DivideByDistance;
        LastDivideDistance = Distance;

        DividePointsByEqualDistance(Distance);
    }
    private void DividePointsByEqualDistance(float DivideDistance)
    {
        AllPointData.Clear();
        AllPointData.Capacity = Accuracy;

        PointOnBezierCurveData FirstCurveData;
        FirstCurveData.Alpha = 0.0f;
        FirstCurveData.Distance = 0.0f;
        FirstCurveData.WorldPosition = BezierCurveHelper.GetPointOnCurve(BezierCurveControlPoints, 0.0f);
        
        AllPointData.Add(FirstCurveData);
        
        float AlphaDelta = 1.0f / Accuracy;
        float CurrentAlpha = 0.0f;

        float TotalDistance  = 0.0f;
        float LastPointDistance  = 0.0f;
        
        for (int i = 1; i < Accuracy; ++i)
        {
            float NextAlpha = CurrentAlpha + AlphaDelta;
            
            Vector3 CurrentPointOnBezier = BezierCurveHelper.GetPointOnCurve(BezierCurveControlPoints, CurrentAlpha);
            Vector3 NextPointOnBezier    = BezierCurveHelper.GetPointOnCurve(BezierCurveControlPoints, NextAlpha);
            
            TotalDistance += Vector3.Distance(CurrentPointOnBezier, NextPointOnBezier);
            CurrentAlpha = NextAlpha;

            if (TotalDistance - LastPointDistance >= DivideDistance)
            {
                PointOnBezierCurveData CurveData;
                CurveData.Alpha = CurrentAlpha;
                CurveData.Distance = TotalDistance;
                CurveData.WorldPosition = CurrentPointOnBezier;
                
                AllPointData.Add(CurveData);
            
                LastPointDistance = TotalDistance;
            }
        }

        PointOnBezierCurveData LastCurveData;
        LastCurveData.Alpha = 1.0f;
        LastCurveData.Distance = BezierCurveHelper.CalculateBezierCurveLength(BezierCurveControlPoints);
        LastCurveData.WorldPosition = BezierCurveHelper.GetPointOnCurve(BezierCurveControlPoints, 1.0f);
        
        AllPointData.Add(LastCurveData);
    }

    private void DrawGizmoOnAllControlPoints(float Radius = 0.4f)
    {
        foreach (Vector3 CurrentControlPoint in BezierCurveControlPoints)
        {
            Gizmos.DrawWireSphere(CurrentControlPoint, Radius);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = GizmoDrawColor;
        BezierCurveHelper.DrawBezierCurveOnGizmo(BezierCurveControlPoints);
        DrawGizmoOnAllControlPoints();
    }
}
