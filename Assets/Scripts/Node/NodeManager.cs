using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[ExecuteInEditMode, RequireComponent(typeof(SplineComponent))]
public class NodeManager : MonoBehaviour
{
    private SplineComponent FirstSplineRef;
    private SplineComponent SecondSplineRef;

    public List<GameObject> FirstSideNodes;
    public List<GameObject> SecondSideNodes;

    [NonSerialized]
    public bool bShouldCreateNodes = false;

    private void Start()
    {
        var SplineComponents = GetComponents<SplineComponent>();

        Assert.IsTrue(SplineComponents.Length >= 2, "At least 2 spline required to build a road");

        if (SplineComponents.Length >= 2)
        {
            FirstSplineRef  = SplineComponents[0];
            FirstSplineRef.OnAnyControlPointMovedAction += OnSplineComponentAnyControlPointMoved;
            
            SecondSplineRef = SplineComponents[1];
            SecondSplineRef.OnAnyControlPointMovedAction += OnSplineComponentAnyControlPointMoved;
        }

        FirstSideNodes = new List<GameObject>();
        SecondSideNodes = new List<GameObject>();
    }

    public void CreateNodes()
    {
        CreateNodesByData(FirstSplineRef.AllPointData, FirstSideNodes,"NodeF_");
        CreateNodesByData(SecondSplineRef.AllPointData, SecondSideNodes,"NodeS_");
    }
    
    public void DestroyAllNodes()
    {
        DestroyNodes(FirstSideNodes);
        DestroyNodes(SecondSideNodes);
    }

    private void CreateNodesByData(List<PointOnBezierCurveData> Data, List<GameObject> Container, string NamePrefix = "Node_")
    {
        DestroyNodes(Container);

        if (bShouldCreateNodes)
        {
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
    }

    private void DestroyNodes(List<GameObject> Container)
    {
        foreach (var CurrentGameObject in Container)
        {
            DestroyImmediate(CurrentGameObject);
        }
        Container.Clear();
    }
    
    private void OnSplineComponentAnyControlPointMoved()
    {
        CreateNodes();
    }
}
