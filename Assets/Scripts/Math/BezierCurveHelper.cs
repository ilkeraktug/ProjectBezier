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

	public static bool DoesBezierCurvesIntersectsAnyPoint(List<PointOnBezierCurveData> FirstBezierPointData, List<PointOnBezierCurveData> SecondBezierPointData, int Substep = 256)
	{
		for (int i = 0; i < FirstBezierPointData.Count - 1; ++i)
		{
			Vector2 First_FirstPoint = new Vector2(FirstBezierPointData[i].WorldPosition.x, FirstBezierPointData[i].WorldPosition.z);
			Vector2 First_SecondPoint = new Vector2(FirstBezierPointData[i + 1].WorldPosition.x, FirstBezierPointData[i + 1].WorldPosition.z);
			
			for (int j = 0; j < SecondBezierPointData.Count - 1; ++j)
			{
				Vector2 Second_FirstPoint = new Vector2(SecondBezierPointData[j].WorldPosition.x, SecondBezierPointData[j].WorldPosition.z);
				Vector2 Second_SecondPoint = new Vector2(SecondBezierPointData[j + 1].WorldPosition.x, SecondBezierPointData[j + 1].WorldPosition.z);

				if (MathHelper.DoTwoLinesIntersect(First_FirstPoint, First_SecondPoint, Second_FirstPoint,
					    Second_SecondPoint))
				{
					return true;
				}
			}
		}

		return false;
	}
}