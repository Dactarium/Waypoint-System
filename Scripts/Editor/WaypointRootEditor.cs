using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaypointRoot))]
public class WaypointRootEditor : Editor
{
    [SerializeField] private Color TestRouteColor = Color.red;
    [SerializeField] private Waypoint start;
    [SerializeField] private Waypoint destination; 
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SerializedObject obj = new SerializedObject(this);
        
        Br();
        DrawLabel("Navigation Test", new Color(.5f, .7f, .8f));
        Br();
        WaypointRoot waypointRoot = target as WaypointRoot;

        TestRouteColor = EditorGUILayout.ColorField("Route Color",TestRouteColor);

        start = EditorGUILayout.ObjectField("Start Waypoint", start, typeof(Waypoint), true) as Waypoint;
        if(start != null && start.transform.parent != waypointRoot.transform) EditorGUILayout.HelpBox("Waypoint must be child of this waypoint root.", MessageType.Error);
        
        destination = EditorGUILayout.ObjectField("Target Waypoint", destination, typeof(Waypoint), true) as Waypoint;
        if(destination != null && destination.transform.parent != waypointRoot.transform) EditorGUILayout.HelpBox("Waypoint must be child of this waypoint root.", MessageType.Error);
    
        EditorGUILayout.HelpBox("You dont have to set start and target waypoints.\nA random child will be assigned to null waypoints.", MessageType.Info);

        if(GUILayout.Button("Navigate")){
            TestPathFinding(waypointRoot, start, destination);

            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }

        obj.ApplyModifiedProperties();
    }

    private void Br() => DrawLabel("", Color.white);
    
    private void DrawLabel(string label) => DrawLabel(label, Color.white);

    private void DrawLabel(string label, Color color){
        var c = GUI.color;
        GUI.color = color;
    
        var style = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};
        EditorGUILayout.LabelField(label, style, GUILayout.ExpandWidth(true));
        GUI.color = c;
    }

    private void TestPathFinding(WaypointRoot waypointRoot, Waypoint start, Waypoint destination){
        if(start == null) start = waypointRoot.transform.GetChild(Random.Range(0, waypointRoot.transform.childCount)).GetComponent<Waypoint>();
        if(destination == null) destination = waypointRoot.transform.GetChild(Random.Range(0, waypointRoot.transform.childCount)).GetComponent<Waypoint>();

        Stack<Waypoint> route = WaypointNavigator.Navigate(start, destination);

        if(route.Count < 2) return;

        Vector3 startPos = route.Pop().transform.position;
        do{
            Vector3 endPos = route.Pop().transform.position;
            Debug.DrawLine(startPos, endPos, TestRouteColor, 0.1f);
            startPos = endPos;
        }while(route.Count > 0);
    }
}
