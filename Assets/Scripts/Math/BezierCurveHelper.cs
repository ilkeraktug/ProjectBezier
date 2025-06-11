using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BezierCurveHelper
{
	public static Vector3 GetPointOnCurve(List<Vector3> Points, float Alpha)
	{
		Vector3 point = Vector3.zero;

		int BezierCurveDegree = Points.Count - 1;
		
		for (int i = 0; i < Points.Count; ++i)
		{
			point += MathHelper.GetCombination(BezierCurveDegree, i) * Mathf.Pow(1 - Alpha, BezierCurveDegree - i) * Mathf.Pow(Alpha, i) * Points[i];
		}

		return point;
	}
}