using System.Collections.Generic;
using UnityEngine;

public static class BezierCurveHelper
{
	public static Vector3 GetPointOnCurve(List<Vector3> BezierCurveControlPoints, float Alpha)
	{
		Vector3 Point = Vector3.zero;

		int BezierCurveDegree = BezierCurveControlPoints.Count - 1;
		
		for (int i = 0; i < BezierCurveControlPoints.Count; ++i)
		{
			Point += MathHelper.GetCombination(BezierCurveDegree, i) * Mathf.Pow(1 - Alpha, BezierCurveDegree - i) * Mathf.Pow(Alpha, i) * BezierCurveControlPoints[i];
		}

		return Point;
	}

	public static float CalculateBezierCurveLength(List<Vector3> BezierCurveControlPoints, int Substep = 256)
	{
		float Step = 1.0f / Substep;
		
		float CurrentAlpha = 0.0f;
		
		float TotalLength = 0.0f;

		for (int i = 0; i < Substep; ++i)
		{
			float NextAlpha = Mathf.Clamp01(CurrentAlpha + Step);
			
			Vector3 CurrentPoint = BezierCurveHelper.GetPointOnCurve(BezierCurveControlPoints, CurrentAlpha);
			Vector3 NextPoint    = BezierCurveHelper.GetPointOnCurve(BezierCurveControlPoints, NextAlpha);
			
			TotalLength += Vector3.Distance(CurrentPoint, NextPoint);
            
			CurrentAlpha = NextAlpha;
		}

		return TotalLength;
	}

	public static void DrawBezierCurveOnGizmo(List<Vector3> BezierCurveControlPoints, int Substep = 256)
	{
		float CurrentAlpha = 0.0f;

		float LineDrawStep = 1.0f / Substep;
		
		for (int i = 0; i < Substep; ++i)
		{
			float NextAlpha = CurrentAlpha + LineDrawStep;
            
			Vector3 CurrentPoint = BezierCurveHelper.GetPointOnCurve(BezierCurveControlPoints, CurrentAlpha);
			Vector3 NextPoint    = BezierCurveHelper.GetPointOnCurve(BezierCurveControlPoints, NextAlpha);
			
			Gizmos.DrawLine(CurrentPoint, NextPoint);

			CurrentAlpha = NextAlpha;
		}
	}
}