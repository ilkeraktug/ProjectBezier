using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoadBuilder))]
public class RoadBuilderEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
        
		RoadBuilder asRoadBuilder = target as RoadBuilder;
        
		if (GUILayout.Button("ResetSplineComponents"))
		{
			asRoadBuilder?.ResetSplineComponents();
		}
		else if (GUILayout.Button("DELETE_COUNTMODE"))
		{
			asRoadBuilder?.DivideSplinesByCount(asRoadBuilder.Count_DELETE);
		}
		else if (GUILayout.Button("DELETE_DISTANCEMODE"))
		{
			asRoadBuilder?.DivideSplinesByDistance(asRoadBuilder.Distance_DELETE);
		}
	}
}
