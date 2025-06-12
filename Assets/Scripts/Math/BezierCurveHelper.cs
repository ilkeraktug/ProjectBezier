using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BezierCurveHelper
{
	public static Vector3 GetPointOnCurve(List<Vector3> BezierCurvePoints, float Alpha)
	{
		Vector3 Point = Vector3.zero;

		int BezierCurveDegree = BezierCurvePoints.Count - 1;
		
		for (int i = 0; i < BezierCurvePoints.Count; ++i)
		{
			Point += MathHelper.GetCombination(BezierCurveDegree, i) * Mathf.Pow(1 - Alpha, BezierCurveDegree - i) * Mathf.Pow(Alpha, i) * BezierCurvePoints[i];
		}

		return Point;
	}

	public static float CalculateBezierCurveLength(List<Vector3> BezierCurvePoints, int Substep = 256)
	{
		float Step = 1.0f / Substep;
		
		float CurrentAlpha = 0.0f;
		
		float TotalLength = 0.0f;

		for (int i = 0; i < Substep; ++i)
		{
			float NextAlpha = CurrentAlpha + Step;
			
			Vector3 CurrentPoint = BezierCurveHelper.GetPointOnCurve(BezierCurvePoints, CurrentAlpha);
			Vector3 NextPoint    = BezierCurveHelper.GetPointOnCurve(BezierCurvePoints, NextAlpha);
			
			TotalLength += Vector3.Distance(CurrentPoint, NextPoint);
            
			CurrentAlpha = NextAlpha;
		}

		return TotalLength;
	}
	public static float GetAlphaFromDistance(float CurrentDistance, float MaxDistance)
	{
		return Mathf.Clamp01(CurrentDistance / MaxDistance);
	}

	public static void DrawBezierCurveOnGizmo(List<Vector3> BezierCurvePoints, int Substep = 256)
	{
		float CurrentAlpha = 0.0f;

		float LineDrawStep = 1.0f / Substep;
		
		for (int i = 0; i < Substep; ++i)
		{
			float NextAlpha = CurrentAlpha + LineDrawStep;
            
			Vector3 CurrentPoint = BezierCurveHelper.GetPointOnCurve(BezierCurvePoints, CurrentAlpha);
			Vector3 NextPoint    = BezierCurveHelper.GetPointOnCurve(BezierCurvePoints, NextAlpha);
			
			Gizmos.DrawLine(CurrentPoint, NextPoint);

			CurrentAlpha = NextAlpha;
		}
	}
}