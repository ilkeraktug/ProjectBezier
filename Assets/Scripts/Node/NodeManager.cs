using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[ExecuteInEditMode, RequireComponent(typeof(SplineComponent))]
public class NodeManager : MonoBehaviour
{
    private List<PointOnBezierCurveData> FirstSplinePointData;
    private List<PointOnBezierCurveData> SecondSplinePointData;

    public List<GameObject> FirstSideNodes;
    public List<GameObject> SecondSideNodes;

    private void Start()
    {
        var SplineComponents = GetComponents<SplineComponent>();

        Assert.IsTrue(SplineComponents.Length >= 2, "At least 2 spline required to build a road");

        if (SplineComponents.Length >= 2)
        {
            FirstSplinePointData  = SplineComponents[0].AllPointData;
            SplineComponents[0].OnAnyControlPointMovedAction += OnSplineComponentAnyControlPointMoved;
            
            SecondSplinePointData = SplineComponents[1].AllPointData;
            SplineComponents[1].OnAnyControlPointMovedAction += OnSplineComponentAnyControlPointMoved;
        }

        FirstSideNodes = new List<GameObject>();
        SecondSideNodes = new List<GameObject>();
    }

    public void CreateNodes()
    {
        CreateNodesByData(FirstSplinePointData, FirstSideNodes,"NodeF_");
        CreateNodesByData(SecondSplinePointData, SecondSideNodes,"NodeS_");
    }

    private void CreateNodesByData(List<PointOnBezierCurveData> Data, List<GameObject> Container, string NamePrefix = "Node_")
    {
        foreach (var CurrentGameObject in Container)
        {
            DestroyImmediate(CurrentGameObject);
        }
        Container.Clear();
        
        int i = 0;
        foreach (PointOnBezierCurveData CurrentSplinePointData in Data)
        {
            GameObject NewNode = new GameObject(NamePrefix + i++);
            NewNode.AddComponent<Node>();
            
            NewNode.transform.position = CurrentSplinePointData.WorldPosition;
            NewNode.transform.SetParent(transform);

            Container.Add(NewNode);
        }
    }

    private void OnSplineComponentAnyControlPointMoved()
    {
        CreateNodes();
    }
}
