using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineComponent))]
public class SplineComponentEditor : Editor
{
    void OnSceneGUI()
    {
        SplineComponent asSplineComponent = target as SplineComponent;

        if (asSplineComponent)
        {
            for (int i = 0; i < asSplineComponent.BezierCurveControlPoints.Count; ++i)
            {
                Vector3 NewPointPosition = Handles.PositionHandle(asSplineComponent.BezierCurveControlPoints[i], asSplineComponent.transform.rotation);

                if (asSplineComponent.BezierCurveControlPoints[i] != NewPointPosition)
                {
                    asSplineComponent.BezierCurveControlPoints[i] = NewPointPosition;

                    asSplineComponent.OnAnyControlPointMoved();
                }
            }
        }
    }
}
