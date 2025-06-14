using System.Linq;
using UnityEditor;
using UnityEngine;

public enum ControlPointsHandling
{
	DeleteControlPoints,
	KeepControlPoints
}

[ExecuteInEditMode]
public class RoadBuilder : MonoBehaviour
{
	public int SplineComponentCalculateAccuracy = 512;

	public ControlPointsHandling ControlPointsHandle = ControlPointsHandling.KeepControlPoints;
	
	// Automatically Move Spline Point
	public bool bAutoMovePoints = false;
	
	private bool SplinesIntersect = false;

	private SplineComponent LeftSideSpline;
	private SplineComponent RightSideSpline;

	private NodeManager NodeManager;

	private MeshBuilder MeshBuilder;
	
	private void Start()
	{
		// Render mesh only works on origin.
		transform.position = Vector3.zero;
		
		SplineComponent[] SplineComponents = gameObject.GetComponents<SplineComponent>();

		if (SplineComponents.Length != 2)
		{
			foreach (var SplineComponent in SplineComponents)
			{
				Destroy(SplineComponent);
			}
			
			LeftSideSpline = gameObject.AddComponent<SplineComponent>();
			RightSideSpline = gameObject.AddComponent<SplineComponent>();
		}
		else
		{
			LeftSideSpline = SplineComponents[0];
			RightSideSpline = SplineComponents[1];
		}

		LeftSideSpline.OnAnyControlPointMovedAction += OnSplineComponentAnyControlPointMoved;
		RightSideSpline.OnAnyControlPointMovedAction += OnSplineComponentAnyControlPointMoved;
		
		NodeManager = gameObject.GetComponent<NodeManager>();

		if (!NodeManager)
		{
			NodeManager = gameObject.AddComponent<NodeManager>();
		}
		
		MeshBuilder = gameObject.GetComponent<MeshBuilder>();

		if (!MeshBuilder)
		{
			MeshBuilder = gameObject.AddComponent<MeshBuilder>();
		}
	}

	public void Subdivide(int BezierCurveDegree)
	{
		LeftSideSpline.Init(BezierCurveDegree, SplineComponentCalculateAccuracy, Color.green);
		RightSideSpline.Init(BezierCurveDegree, SplineComponentCalculateAccuracy, Color.blue);

		CreateSplineControlPoints();
		
		DisableManagers();

		// Divided curves to detect intersection between 2 splines
		LeftSideSpline.DividePointsByDistance(2.0f);
		RightSideSpline.DividePointsByDistance(2.0f);
	}
	
	public void CancelSubdivide()
	{
		LeftSideSpline.ResetEverything();
		RightSideSpline.ResetEverything();

		DisableManagers();
	}

	public bool TwoSplineIntersectAtAnyPoint()
	{
		return SplinesIntersect;
	}
	public void DivideSplinesByDistance(float Distance)
	{
		if (bAutoMovePoints)
		{
			HandleAutomaticallyMoveSplineControlPoints();
		}

		LeftSideSpline.DividePointsByDistance(Distance);
		RightSideSpline.DividePointsByDistance(Distance);
		
		OnSplinePointsDivided();
	}

	public void DivideSplinesByCount(int Count)
	{
		LeftSideSpline.DividePointsByCount(Count);
		RightSideSpline.DividePointsByCount(Count);

		OnSplinePointsDivided();
	}

	public bool IsSubdivided()
	{
		return LeftSideSpline && LeftSideSpline.HasAnyPoints();
	}

	public bool BuiltRoad()
	{
		return MeshBuilder && MeshBuilder.bShouldRender;
	}
	
	private void CreateSplineControlPoints()
	{
		LeftSideSpline.CreateControlPoints(transform.position);
		RightSideSpline.CreateControlPoints(transform.position + transform.right * 10.0f);
	}
	
	private void ClearSplineControlPoints()
	{
		LeftSideSpline.ClearControlPoints();
		RightSideSpline.ClearControlPoints();
	}

	private void ActivateManagers()
	{
		NodeManager.bShouldCreateNodes = true;
		NodeManager.CreateNodes();
		
		MeshBuilder.bShouldRender = true;
		MeshBuilder.UpdateMesh();
	}
	
	private void DisableManagers()
	{
		NodeManager.bShouldCreateNodes = false;
		NodeManager.DestroyAllNodes();
		
		MeshBuilder.UpdateMesh();
		MeshBuilder.bShouldRender = false;
	}

	private void OnSplinePointsDivided()
	{
		if (ControlPointsHandle == ControlPointsHandling.DeleteControlPoints)
		{
			ClearSplineControlPoints();
		}

		ActivateManagers();
	}

	private void OnSplineComponentAnyControlPointMoved()
	{
		SplinesIntersect = BezierCurveHelper.DoesBezierCurvesIntersectsAnyPoint(LeftSideSpline.AllPointData, RightSideSpline.AllPointData);
	}

	private void HandleAutomaticallyMoveSplineControlPoints()
	{
		PointOnBezierCurveData LeftSplineFirstCurveData = LeftSideSpline.AllPointData.First();
		PointOnBezierCurveData LeftSplineLastCurveData  = LeftSideSpline.AllPointData.Last();

		PointOnBezierCurveData RightSplineFirstCurveData = RightSideSpline.AllPointData.First();
		PointOnBezierCurveData RightSplineLastCurveData  = RightSideSpline.AllPointData.Last();
		
		float LeftSplineLengthRatio = LengthRatio(LeftSideSpline);
		float RightSplineLengthRatio = LengthRatio(RightSideSpline);

		float DistanceBetweenFirstPoints = Vector3.Distance(LeftSplineFirstCurveData.WorldPosition, RightSplineFirstCurveData.WorldPosition);
		float DistanceBetweenLastPoints = Vector3.Distance(LeftSplineLastCurveData.WorldPosition, RightSplineLastCurveData.WorldPosition);
		
		bool bMoveFirstControlPoint = DistanceBetweenFirstPoints > DistanceBetweenLastPoints;

		float LengthDifference = CalculateSplineLengthDifference();
		bool bAddLeftSpline = LengthDifference <= 0;
		
		if (bMoveFirstControlPoint)
		{
			if (bAddLeftSpline)
			{
				AddSplinePointFront(RightSideSpline, LeftSideSpline, LengthDifference / RightSplineLengthRatio);
			}
			else
			{
				AddSplinePointFront(LeftSideSpline, RightSideSpline, -LengthDifference / LeftSplineLengthRatio);
			}
		}
		else
		{
			if (bAddLeftSpline)
			{
				AddSplinePointLast(RightSideSpline, LeftSideSpline, -LengthDifference / RightSplineLengthRatio);
			}
			else
			{
				AddSplinePointLast(LeftSideSpline, RightSideSpline, LengthDifference / LeftSplineLengthRatio);
			}
		}
	}

	private float LengthRatio(SplineComponent SplineComponent)
	{
		PointOnBezierCurveData SplineFirstCurveData = SplineComponent.AllPointData.First();
		PointOnBezierCurveData SplineLastCurveData  = SplineComponent.AllPointData.Last();

		float SplineLength = SplineLastCurveData.Distance;
		
		float StartToEndDistance = Vector3.Distance(SplineFirstCurveData.WorldPosition, SplineLastCurveData.WorldPosition);
		return SplineLength / StartToEndDistance;
	}

	private float CalculateSplineLengthDifference()
	{
		float LeftSplineLength = LeftSideSpline.AllPointData.Last().Distance;
		float RightSplineLength = RightSideSpline.AllPointData.Last().Distance;
		
		return LeftSplineLength - RightSplineLength;
	}
	
	private void AddSplinePointFront(SplineComponent From, SplineComponent To, float Scale)
	{
		if (From.BezierCurveControlPoints.Count > 1 && To.BezierCurveControlPoints.Count > 0)
		{
			Vector3 FirstControlPointPosition = To.BezierCurveControlPoints[0];

			Vector3 FirstTangent = From.AllPointData[1].Tangent;

			To.BezierCurveControlPoints[0] = FirstControlPointPosition + FirstTangent * Scale;
		}
	}

	private void AddSplinePointLast(SplineComponent From, SplineComponent To, float Scale)
	{
		if (From.BezierCurveControlPoints.Count > 0 && To.BezierCurveControlPoints.Count > 0)
		{
			Vector3 LastControlPointPosition = To.BezierCurveControlPoints[To.BezierCurveControlPoints.Count - 1];
			
			Vector3 LastTangent = From.AllPointData.Last().Tangent;

			To.BezierCurveControlPoints[To.BezierCurveControlPoints.Count - 1] = LastControlPointPosition + LastTangent * Scale;
		}
	}
	
	private void OnDrawGizmos()
	{
		bool bObjectIsSelected = Selection.activeObject == gameObject;
		bool bAnyPartIsVisibleWhenSelected = IsSubdivided();
		bool bAnyPartIsVisibleWhenNOTSelected = BuiltRoad();

		bool bShouldHideWhenSelected = !(bAnyPartIsVisibleWhenSelected && bObjectIsSelected);
		bool shouldHideWhenNOTSelected  = !bAnyPartIsVisibleWhenNOTSelected;
		
		bool bShouldRenderYellowSphere = bShouldHideWhenSelected && shouldHideWhenNOTSelected;
		
		if (bShouldRenderYellowSphere)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, 1.0f);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (LeftSideSpline.HasAnyPoints())
		{
			Tools.current = Tool.None;
		}
	}
}