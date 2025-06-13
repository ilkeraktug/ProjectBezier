using System;
using UnityEditor;
using UnityEngine;

public class RoadBuilderEditorWindow : EditorWindow
{
    private int BezierCurveDegree = 4;

    private int DivideCount = 5;
    private float DivideDistance = 3.0f;
    
    private static float DefaultButtonHeight = 20.0f;
    private static float DefaultButtonVerticalSpace = 2.0f;
    
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
                
                if (RoadBuilder.IsSubdivided())
                {
                    OnAfterSubdivided(RoadBuilder);

                    ShowCancelButton(RoadBuilder);
                }
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
        GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
        centeredStyle.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("BEZIER CURVE", centeredStyle);
        
        GUILayout.Space(DefaultButtonHeight + DefaultButtonVerticalSpace);
        
        GUILayout.BeginHorizontal();
        
            BezierCurveDegree = Mathf.Clamp(EditorGUILayout.DelayedIntField("Bezier Curve Degree", BezierCurveDegree), 4, 10);
            
            GUILayout.Space(DefaultButtonHeight + DefaultButtonVerticalSpace);
            
            BezierCurveDegree = EditorGUILayout.IntSlider("Bezier Curve Degree", BezierCurveDegree, 4, 10);
        
        GUILayout.EndHorizontal();
            
        GUILayout.Space(DefaultButtonHeight);
        
        if (GUILayout.Button("Subdivision"))
        {
            RoadBuilder.Subdivide(BezierCurveDegree);
        }
            
    }

    void OnAfterSubdivided(RoadBuilder RoadBuilder)
    {
        GUILayout.Space(DefaultButtonHeight + DefaultButtonVerticalSpace);
        
        GUILayout.BeginVertical();
        
            GUILayout.BeginHorizontal();
        
                if (GUILayout.Button("Count Mode"))
                {
                    RoadBuilder.DivideSplinesByCount(DivideCount);
                }
                
                if (GUILayout.Button("Distance Mode"))
                {
                    RoadBuilder.DivideSplinesByDistance(DivideDistance);
                }
        
            GUILayout.EndHorizontal();
        
            GUILayout.BeginHorizontal();
            
                DivideCount = Mathf.Clamp(EditorGUILayout.DelayedIntField("Count", DivideCount), 1, 256);

                GUILayout.Space(5);
                
                DivideDistance = Mathf.Clamp(EditorGUILayout.DelayedFloatField("Distance", DivideDistance), 1.0f, float.MaxValue);
            
            GUILayout.EndHorizontal();
        
        GUILayout.EndVertical();
    }

    void ShowCancelButton(RoadBuilder RoadBuilder)
    {
        if (GUILayout.Button("Cancel / Clear"))
        {
            RoadBuilder.CancelSubdivide();
        }
    }
}
