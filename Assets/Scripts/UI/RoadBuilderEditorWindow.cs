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
                OnSelectedObjectHasNoRoadBuilderScript();
            }
        }
        else
        {
            OnNoObjectIsSelected();
        }
    }

    void OnNoObjectIsSelected()
    {
        GUILayout.Label("Please select a object!");
    }

    void OnSelectedObjectHasNoRoadBuilderScript()
    {
        GUILayout.Label("Could not find <b>RoadBuilder</b> script!");
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
