using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoadBuilder))]
public class RoadBuilderEditor : Editor
{
    void OnSceneGUI()
    {
        RoadBuilder asRoadBuilder = target as RoadBuilder;

        if (asRoadBuilder)
        {
            List<Vector3> LeftBezierCurvePoints = asRoadBuilder.LeftSideBezierCurvePoints;
            
            
            for (int i = 0; i < LeftBezierCurvePoints.Count; ++i)
            {
                Vector3 NewPointPosition = Handles.PositionHandle(LeftBezierCurvePoints[i], asRoadBuilder.transform.rotation);

                if (LeftBezierCurvePoints[i] != NewPointPosition)
                {
                    LeftBezierCurvePoints[i] = NewPointPosition;

                    asRoadBuilder.OnAnyLeftPointMoved();
                }
            }
            
            List<Vector3> RightBezierCurvePoints = asRoadBuilder.RightSideBezierCurvePoints;
            
            for (int i = 0; i < RightBezierCurvePoints.Count; ++i)
            {
                Vector3 NewPointPosition = Handles.PositionHandle(RightBezierCurvePoints[i], asRoadBuilder.transform.rotation);

                if (RightBezierCurvePoints[i] != NewPointPosition)
                {
                    RightBezierCurvePoints[i] = NewPointPosition;

                    asRoadBuilder.OnAnyRightPointMoved();
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        RoadBuilder asRoadBuilder = target as RoadBuilder;
        
        if (GUILayout.Button("CreatePoints"))
        {
            asRoadBuilder?.CreatePoints();
        }
        else if (GUILayout.Button("ClearPoints"))
        {
            asRoadBuilder?.ClearPoints();
        }
    }
}
