using System;
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
	}
	
	public void CancelSubdivide()
	{
		LeftSideSpline.ResetEverything();
		RightSideSpline.ResetEverything();

		DisableManagers();
	}
	public void DivideSplinesByDistance(float Distance)
	{
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
		return MeshBuilder.bShouldRender;
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
		NodeManager.CreateNodes();
		
		MeshBuilder.bShouldRender = true;
		MeshBuilder.UpdateMesh();
	}
	
	private void DisableManagers()
	{
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