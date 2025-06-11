using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class RoadBuilder : MonoBehaviour
{
    public int PointNumberPerSide = 4;

    [SerializeField] 
    private List<Vector3> BezierCurvePoints;

    public void CreatePoints()
    {
        BezierCurvePoints = new List<Vector3>();
        
        for (int i = 0; i < PointNumberPerSide * 2; ++i)
        {
            Vector3 NewPointLocation = new Vector3(10.0f * (i / PointNumberPerSide), 0.0f, 10.0f * (i % PointNumberPerSide));

            BezierCurvePoints.Add(NewPointLocation);
        }
    }

    public void ClearPoints()
    {
        BezierCurvePoints.Clear();
    }

    public List<Vector3> GetBezierCurvePoints()
    {
        return BezierCurvePoints;
    }

    private void OnDrawGizmos()
    {
        if (Selection.activeObject != gameObject)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Tools.current = Tool.None;
        
        for (int i = 0; i < BezierCurvePoints.Count; ++i)
        {
            Vector3 CurrentPointPosition = BezierCurvePoints[i];
            bool bLeftLane = i < 4;

            Gizmos.color = bLeftLane ? Color.green : Color.blue;
            Gizmos.DrawWireSphere(CurrentPointPosition, 0.4f);
        }
    }
}

[CustomEditor(typeof(RoadBuilder))]
public class RoadBuilderEditor : Editor
{
    void OnSceneGUI()
    {
        RoadBuilder asRoadBuilder = target as RoadBuilder;

        if (asRoadBuilder)
        {
            List<Vector3> BezierCurvePoints = asRoadBuilder.GetBezierCurvePoints();
            
            for (int i = 0; i < BezierCurvePoints.Count; ++i)
            {
                Vector3 CurrentPointPosition = BezierCurvePoints[i];
                BezierCurvePoints[i] = Handles.PositionHandle(CurrentPointPosition, asRoadBuilder.transform.rotation);
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