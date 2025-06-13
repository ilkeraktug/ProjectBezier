using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class RoadBuilder : MonoBehaviour
{
	public int BezierCurveDegree = 4;
	public int SplineComponentCalculateAccuracy = 512;

	private SplineComponent LeftSideSpline;
	private SplineComponent RightSideSpline;

	private NodeManager NodeManager;

	private MeshBuilder MeshBuilder;

	public int Count_DELETE = 5;
	public float Distance_DELETE = 3.0f;

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

		LeftSideSpline.Init(BezierCurveDegree, SplineComponentCalculateAccuracy, Color.green);
		RightSideSpline.Init(BezierCurveDegree, SplineComponentCalculateAccuracy, Color.blue);

		CreatePoints();
	}

	public void CreatePoints()
	{
		LeftSideSpline.CreateControlPoints(transform.position);
		RightSideSpline.CreateControlPoints(transform.position + transform.right * 10.0f);
	}

	public void ClearSplineComponents()
	{
		LeftSideSpline.ClearControlPoints();
		RightSideSpline.ClearControlPoints();
	}

	public void DivideSplinesByDistance(float Distance)
	{
		LeftSideSpline.DividePointsByDistance(Distance);
		RightSideSpline.DividePointsByDistance(Distance);

		NodeManager.CreateNodes();
	}

	public void DivideSplinesByCount(int Count)
	{
		LeftSideSpline.DividePointsByCount(Count);
		RightSideSpline.DividePointsByCount(Count);

		NodeManager.CreateNodes();
	}

	public void ResetSplineComponents()
	{
		ClearSplineComponents();

		LeftSideSpline.Init(BezierCurveDegree, SplineComponentCalculateAccuracy, Color.green);
		RightSideSpline.Init(BezierCurveDegree, SplineComponentCalculateAccuracy, Color.blue);

		CreatePoints();
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