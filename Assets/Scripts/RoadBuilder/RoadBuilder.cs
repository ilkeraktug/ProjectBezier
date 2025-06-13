using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class RoadBuilder : MonoBehaviour
{
	public int SplineComponentCalculateAccuracy = 512;

	private SplineComponent LeftSideSpline;
	private SplineComponent RightSideSpline;

	private NodeManager NodeManager;

	private MeshBuilder MeshBuilder;

	private void Start()
	{
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
		ClearSplineControlPoints();
		
		ActivateManagers();
	}

	public void DivideSplinesByCount(int Count)
	{
		LeftSideSpline.DividePointsByCount(Count);
		RightSideSpline.DividePointsByCount(Count);
		ClearSplineControlPoints();

		ActivateManagers();
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
		
		MeshBuilder.bShouldRender = false;
		MeshBuilder.UpdateMesh();
	}
	private void OnDrawGizmos()
	{
		bool bSplineHasAnyPoint = LeftSideSpline && LeftSideSpline.HasAnyPoints();
		
		if (!bSplineHasAnyPoint || Selection.activeObject != gameObject)
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