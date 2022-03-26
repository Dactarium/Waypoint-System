using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WaypointManagerWindow : EditorWindow
{
    [MenuItem("Tools/Waypoint Editor")]
    public static void Open(){
        GetWindow<WaypointManagerWindow>();
    }

    public WaypointRoot WaypointRoot;

    [Tooltip("Affects Selection contains Waypoint")]
    public bool WaypointSelection = false;

    private List<Waypoint> selectedWaypoints = new List<Waypoint>();
    private Vector2 scrollPos = Vector2.zero;

    private void OnGUI(){
        SerializedObject obj = new SerializedObject(this);
        
        EditorGUILayout.PropertyField(obj.FindProperty("WaypointSelection"), GUILayout.Width(165));

        Br();
        if(GUILayout.Button("Create New Waypoint Root")) CreateNewWaypointRoot();
        
        EditorGUILayout.PropertyField(obj.FindProperty("WaypointRoot"));
        if(WaypointRoot == null){
            EditorGUILayout.HelpBox("Root transform must be selected. Please assign a root transform.", MessageType.Warning);
        }else{

            if(WaypointSelection && selectedWaypoints.Count > 0){
                GameObject[] selectedGameObjects = new GameObject[selectedWaypoints.Count];
                for(int i = 0; i < selectedWaypoints.Count; i++){
                   selectedGameObjects[i] = selectedWaypoints[i].gameObject;
                }
                Selection.objects = selectedGameObjects;
            }

            Br();
            EditorGUILayout.BeginVertical("box");
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                    DrawButtons();
                EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        obj.ApplyModifiedProperties();
    }

    private void OnSelectionChange(){
        Waypoint[] selectionWaypoints = Selection.GetFiltered<Waypoint>(SelectionMode.Unfiltered);
        WaypointRoot selectedRoot = null;
        selectedWaypoints.Clear();

        if(selectedRoot == null && Selection.activeGameObject && Selection.activeGameObject.GetComponent<Waypoint>() && Selection.activeGameObject.transform.parent) selectedRoot = Selection.activeGameObject.transform.parent.GetComponent<WaypointRoot>();

        foreach(Waypoint selectionWaypoint in selectionWaypoints){
            if(!selectionWaypoint.transform.parent || !selectionWaypoint.transform.parent.GetComponent<WaypointRoot>()){
                Debug.LogWarning($"Waypoint named \"{selectionWaypoint.name}\" must have parent type of WaypointRoot");
                continue;
            }
            if(selectedRoot == null) selectedRoot = selectionWaypoint.transform.parent.GetComponent<WaypointRoot>();
            if(selectedRoot == selectionWaypoint.transform.parent.GetComponent<WaypointRoot>()) selectedWaypoints.Add(selectionWaypoint.GetComponent<Waypoint>());
        }

        if(selectedRoot != null) WaypointRoot = selectedRoot;
        Repaint();
    }

    private void checkSelectedWaypoints(){
        if(selectedWaypoints.Count == 0) return;

        foreach(Waypoint waypoint in selectedWaypoints){
            if(waypoint != null) continue;

            OnSelectionChange();
            return;
        }
    }

    private void DrawButtons(){
        if(GUILayout.Button("Create Waypoint")) CreateWaypoint();
        if(selectedWaypoints != null){
            checkSelectedWaypoints();
            Br();
            if(selectedWaypoints.Count > 0){
                WaypointRoot selectedRoot = selectedWaypoints[0].transform.parent.GetComponent<WaypointRoot>();
                if(selectedRoot == null){
                    GUI.enabled = false;
                }else{
                    GUI.enabled = selectedRoot == WaypointRoot;
                }
            }
            if(selectedWaypoints.Count == 1){
                DrawLabel("Selected: " + selectedWaypoints[0].name, Color.yellow);

                if(GUILayout.Button("Create Waypoint From Selected")) CreateWaypointFrom(selectedWaypoints[0]);
                if(GUILayout.Button("Remove Waypoint")) RemoveWaypoint(selectedWaypoints[0]);
                
            }else if(selectedWaypoints.Count > 1){
                DrawLabel(selectedWaypoints.Count + " Waypoints selected", Color.yellow);
                if(GUILayout.Button("Connect to New Waypoint")) ConnectToNewWaypoint(selectedWaypoints);
                if(GUILayout.Button("Connect Selected Waypoints")) ConnectWaypoints(selectedWaypoints);
                if(GUILayout.Button("Disconnect Selected Waypoints")) DisconnectWaypoints(selectedWaypoints);
                if(GUILayout.Button("Remove Selected Waypoints")) RemoveWaypoints(selectedWaypoints);
            }
        }
    }

    private void Br() => DrawLabel("", Color.white);
    private void DrawLabel(string label) => DrawLabel(label, Color.white);
    private void DrawLabel(string label, Color color) => DrawLabel(label, color, TextAnchor.MiddleCenter);
    private void DrawLabel(string label, Color color, TextAnchor anchor){
        var c = GUI.color;
        GUI.color = color;
    
        var style = new GUIStyle(GUI.skin.label) {alignment = anchor};
        EditorGUILayout.LabelField(label, style, GUILayout.ExpandWidth(true));
        GUI.color = c;
    }

    private void CreateNewWaypointRoot(){
        GameObject waypointRootObj = new GameObject("Waypoint Root", typeof(WaypointRoot));

        WaypointRoot = waypointRootObj.GetComponent<WaypointRoot>();
    }

    private void CreateWaypoint(){
        GameObject waypointObject = new GameObject("Waypoint", typeof(Waypoint));
        waypointObject.transform.SetParent(WaypointRoot.transform, false);
        
        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();

        if(WaypointRoot.transform.childCount > 1){
            Waypoint connectedWaypoint = WaypointRoot.transform.GetChild(WaypointRoot.transform.childCount - 2).GetComponent<Waypoint>();
            newWaypoint.Connect(connectedWaypoint);
            connectedWaypoint.Connect(newWaypoint);

            newWaypoint.transform.position = connectedWaypoint.transform.position;
            newWaypoint.transform.forward = connectedWaypoint.transform.forward;
        }

        Selection.activeGameObject = newWaypoint.gameObject;

        Undo.RegisterCreatedObjectUndo(newWaypoint.gameObject, "Created new waypoint");
    }

    private void CreateWaypointFrom(Waypoint selectedWaypoint){
        GameObject waypointObject = new GameObject("Waypoint", typeof(Waypoint));
        waypointObject.transform.SetParent(WaypointRoot.transform, false);
        
        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();

        waypointObject.transform.position = selectedWaypoint.transform.position;
        waypointObject.transform.forward = selectedWaypoint.transform.forward;

        ConnectWaypoint(newWaypoint, selectedWaypoint);

        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex() + 1);

        Selection.activeGameObject = newWaypoint.gameObject;

        Undo.RegisterCreatedObjectUndo(newWaypoint.gameObject, "Created new waypoint from selected waypoint");
    }

    private void RemoveWaypoint(Waypoint selectedWaypoint){
        Undo.DestroyObjectImmediate(selectedWaypoint.gameObject);
    }

    private void ConnectToNewWaypoint(List<Waypoint> waypoints){
        GameObject waypointObject = new GameObject("Waypoint", typeof(Waypoint));
        waypointObject.transform.SetParent(WaypointRoot.transform, false);
        
        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();
        
        Vector3 position = Vector3.zero;
        Vector3 forward = Vector3.zero;

        for(int i = 0; i < waypoints.Count; i++){
            ConnectWaypoint(newWaypoint, waypoints[i]);

            position = (position * i + waypoints[i].transform.position) / (i + 1);
            forward = (forward * i + waypoints[i].transform.forward) / (i + 1);
        }

        waypointObject.transform.position = position;
        waypointObject.transform.forward = forward;

        Selection.activeGameObject = newWaypoint.gameObject;

        Undo.RegisterCreatedObjectUndo(newWaypoint.gameObject, "Created new waypoint and connected all selected waypoints");
    }

    private void ConnectWaypoints(List<Waypoint> waypoints){
        for(int i = 0; i < waypoints.Count - 1; i++){
            for(int j = i + 1; j < waypoints.Count; j++){
                ConnectWaypoint(waypoints[i], waypoints[j]);
            }
        }

        SceneView.RepaintAll();
    }

    private void ConnectWaypoint(Waypoint a, Waypoint b){
        Undo.RecordObjects(new Object[]{a, b}, "Connected Waypoints");
        if(!a.Has(b)) a.Connect(b);
        if(!b.Has(a)) b.Connect(a);
    }

    private void DisconnectWaypoints(List<Waypoint> waypoints){
        for(int i = 0; i < waypoints.Count - 1; i++){
            for(int j = i + 1; j < waypoints.Count; j++){
                DisconnectWaypoint(waypoints[i], waypoints[j]);
            }
        }

        SceneView.RepaintAll();
    }

    private void DisconnectWaypoint(Waypoint a, Waypoint b){
        Undo.RecordObjects(new Object[]{a, b}, "Disconnected Waypoints");
        a.Disconnect(b);
        b.Disconnect(a);
    }

    private void RemoveWaypoints(List<Waypoint> waypoints){
        foreach(Waypoint waypoint in waypoints){
            RemoveWaypoint(waypoint);
        }
    }

}
