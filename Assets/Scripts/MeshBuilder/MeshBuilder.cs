using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[ExecuteInEditMode, 
 RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider)), 
 RequireComponent(typeof(SplineComponent))]
public class MeshBuilder : MonoBehaviour
{
	[NonSerialized]
	public bool bShouldRender = false;
	
	private MeshFilter MeshFilter;
	private MeshCollider MeshCollider;
	
	private List<PointOnBezierCurveData> FirstSplinePointData;
	private List<PointOnBezierCurveData> SecondSplinePointData;
	private void Start()
	{
		MeshFilter = GetComponent<MeshFilter>();
		MeshCollider = GetComponent<MeshCollider>();
		
		var SplineComponents = GetComponents<SplineComponent>();

		Assert.IsTrue(SplineComponents.Length >= 2, "At least 2 spline required to build a road");

		if (SplineComponents.Length >= 2)
		{
			FirstSplinePointData  = SplineComponents[0].AllPointData;
			SplineComponents[0].OnAnyControlPointMovedAction += OnSplineComponentAnyControlPointMoved;
            
			SecondSplinePointData = SplineComponents[1].AllPointData;
			SplineComponents[1].OnAnyControlPointMovedAction += OnSplineComponentAnyControlPointMoved;
		}
	}

	public void UpdateMesh()
	{
		UpdateMesh(SecondSplinePointData, FirstSplinePointData);
	}
	private void UpdateMesh(List<PointOnBezierCurveData> LeftNodes, List<PointOnBezierCurveData> RightNodes)
	{
		int Loop = Mathf.Min(LeftNodes.Count, RightNodes.Count);
		
		var mesh = new Mesh();

		List<Vector3> MeshVertices = new List<Vector3>();
		List<int> MeshIndices = new List<int>();

		for (int i = 0; i < Loop; ++i)
		{
			MeshVertices.Add(LeftNodes[i].WorldPosition);
			MeshVertices.Add(RightNodes[i].WorldPosition);
		}

		for (int i = 0; i < Loop - 1; ++i)
		{
			int leftCurrent = i * 2;      // Current left node index
			int rightCurrent = i * 2 + 1; // Current right node index
			int leftNext = (i + 1) * 2;   // Next left node index
			int rightNext = (i + 1) * 2 + 1; // Next right node index
    
			// First triangle: leftCurrent -> rightCurrent -> leftNext
			MeshIndices.Add(leftCurrent);
			MeshIndices.Add(rightCurrent);
			MeshIndices.Add(leftNext);
    
			// Second triangle: rightCurrent -> rightNext -> leftNext
			MeshIndices.Add(rightCurrent);
			MeshIndices.Add(rightNext);
			MeshIndices.Add(leftNext);
		}
        
		mesh.vertices = MeshVertices.ToArray();
		mesh.triangles = MeshIndices.ToArray();
		
		MeshFilter.mesh = mesh;
		MeshCollider.sharedMesh = mesh;
	}
	
	private void OnSplineComponentAnyControlPointMoved()
	{
		if (bShouldRender)
		{
			UpdateMesh(SecondSplinePointData, FirstSplinePointData);
		}
	}
}
