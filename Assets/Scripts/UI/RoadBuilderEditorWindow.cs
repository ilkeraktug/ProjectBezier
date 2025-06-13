using System;
using UnityEditor;
using UnityEngine;

public class RoadBuilderEditorWindow : EditorWindow
{
    private int BezierCurveDegree = 4;

    private int DivideCount = 5;
    private float DivideDistance = 3.0f;
    
    
    [MenuItem("ADASTEC/RoadBuilder")]
    public static void ShowExample()
    {
        RoadBuilderEditorWindow Window = GetWindow<RoadBuilderEditorWindow>();
        Window.titleContent = new GUIContent("Road Builder");
    }
    public void OnGUI()
    {
        GameObject SelectedObject = Selection.activeGameObject;
        
        if (SelectedObject)
        {
            RoadBuilder RoadBuilder = SelectedObject.GetComponent<RoadBuilder>();

            if (RoadBuilder)
            {
                OnBeforeSubdivided(RoadBuilder);
                OnAfterSubdivided(RoadBuilder);
            }
            else
            {
                OnSelectedObjectHasNoRoadBuilderScript(SelectedObject);
            }
        }
        else
        {
            OnNoObjectIsSelected();
        }
    }

    public void Update()
    {
        // RoadBuilderEditorWindow::OnGuI is not called when the window is out of focus. (I think)
        // I need user to select a gameObjects that contains RoadBuilder script to modify the curves.
        // When user select a gameObject, obiviosly the Window lost focus, thus do not update its content. In order to prevent confusion on user end, I want to update window contents as soon as possible.
        Repaint();
    }

    void OnNoObjectIsSelected()
    {
        GUILayout.Label("Please select a object!");
    }

    void OnSelectedObjectHasNoRoadBuilderScript(GameObject SelectedObject)
    {
        EditorGUILayout.BeginVertical(new GUIStyle());
        GUILayout.Label("Could not find <b>RoadBuilder</b> script!");

        if (GUILayout.Button("Add RoadBuilder script"))
        {
            SelectedObject.AddComponent<RoadBuilder>();
        }

        GUILayout.EndVertical();
    }

    void OnBeforeSubdivided(RoadBuilder RoadBuilder)
    {
        BezierCurveDegree = Mathf.Clamp(EditorGUILayout.DelayedIntField("Bezier Curve Degree", BezierCurveDegree), 4, 10);
        
        if (GUILayout.Button("Subdivision"))
        {
            RoadBuilder.Subdivide(BezierCurveDegree);
        }
        if (GUILayout.Button("Cancel"))
        {
            RoadBuilder.CancelSubdivide();
        }
    }

    void OnAfterSubdivided(RoadBuilder RoadBuilder)
    {
        DivideCount = Mathf.Clamp(EditorGUILayout.DelayedIntField("DivideCount", DivideCount), 1, 256);
        
        if (GUILayout.Button("DivideCount"))
        {
            RoadBuilder.DivideSplinesByCount(DivideCount);
        }
        
        DivideDistance = Mathf.Clamp(EditorGUILayout.DelayedFloatField("DivideDistance", DivideDistance), 1.0f, float.MaxValue);
        
        if (GUILayout.Button("DivideDistance"))
        {
            RoadBuilder.DivideSplinesByDistance(DivideDistance);
        }
    }
}
