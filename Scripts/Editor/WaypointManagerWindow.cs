using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WaypointManagerWindow : EditorWindow
{
    [MenuItem("Tools/Waypoint Editor")]
    public static void Open(){
        GetWindow<WaypointManagerWindow>();
    }

    public Transform waypointRoot;
    private List<Waypoint> selectedWaypoints = new List<Waypoint>();
    private Vector2 scrollPos = Vector2.zero;
    private void OnGUI(){
        SerializedObject obj = new SerializedObject(this);
        
        EditorGUILayout.PropertyField(obj.FindProperty("waypointRoot"));

        if(waypointRoot == null){
            EditorGUILayout.HelpBox("Root transform must be selected. Please assign a root transform.", MessageType.Warning);
        }else{
            EditorGUILayout.BeginVertical("box");
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            DrawButtons();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        obj.ApplyModifiedProperties();
    }

    private void OnSelectionChange(){
        GameObject[] selectedObjs = Selection.gameObjects;
        selectedWaypoints.Clear();
        foreach(GameObject selectedObj in selectedObjs){
            if(selectedObj.GetComponent<Waypoint>()){
                selectedWaypoints.Add(selectedObj.GetComponent<Waypoint>());
            }
        }
        Repaint();
    }

    private void DrawButtons(){
        if(GUILayout.Button("Create Waypoint")) CreateWaypoint();
        if(selectedWaypoints != null){
            Br();
            if(selectedWaypoints.Count == 1){
                DrawLabel("Selected: "+selectedWaypoints[0].name, Color.yellow);

                if(GUILayout.Button("Create Waypoint From Selected")) CreateWaypointFrom(selectedWaypoints[0]);
                if(GUILayout.Button("Remove Waypoint")) RemoveWaypoint(selectedWaypoints[0]);
                
                Br();

                if(selectedWaypoints[0].GetType() != typeof(Location) && GUILayout.Button("Make Location")) MakeLocation(selectedWaypoints[0]);
                if(selectedWaypoints[0].GetType() == typeof(Location) && GUILayout.Button("Make Waypoint")) MakeWaypoint(selectedWaypoints[0]);

            }else if(selectedWaypoints.Count > 1){
                DrawLabel(selectedWaypoints.Count + " Waypoints selected", Color.yellow);
                if(GUILayout.Button("Connect to New Waypoint")) ConnectToNewWaypoint(selectedWaypoints);
                if(GUILayout.Button("Connect Selected Waypoints")) ConnectWaypoints(selectedWaypoints);
                if(GUILayout.Button("Disconnect Selected Waypoints")) DisconnectWaypoints(selectedWaypoints);
                if(GUILayout.Button("Remove Selected Waypoints")) RemoveWaypoints(selectedWaypoints);

                Br();

                if(GUILayout.Button("Waypoint to Location")) WaypointToLocation(new List<Waypoint>(selectedWaypoints));
                if(GUILayout.Button("Location to Waypoint")) LocationToWaypoint(new List<Waypoint>(selectedWaypoints));

            }
        }
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


    private void CreateWaypoint(){
        GameObject waypointObject = new GameObject("Waypoint", typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);
        
        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();

        if(waypointRoot.childCount > 1){
            Waypoint connectedWaypoint = waypointRoot.GetChild(waypointRoot.childCount - 2).GetComponent<Waypoint>();
            newWaypoint.Connections.Add(connectedWaypoint);
            connectedWaypoint.Connections.Add(newWaypoint);

            newWaypoint.transform.position = connectedWaypoint.transform.position;
            newWaypoint.transform.forward = connectedWaypoint.transform.forward;
        }

        Selection.activeGameObject = newWaypoint.gameObject;
    }

    private void CreateWaypointFrom(Waypoint selectedWaypoint){
        GameObject waypointObject = new GameObject("Waypoint", typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);
        
        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();

        waypointObject.transform.position = selectedWaypoint.transform.position;
        waypointObject.transform.forward = selectedWaypoint.transform.forward;

        ConnectWaypoint(newWaypoint, selectedWaypoint);

        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex() + 1);

        Selection.activeGameObject = newWaypoint.gameObject;
    }

    private void RemoveWaypoint(Waypoint selectedWaypoint){
        
        foreach(Waypoint connection in selectedWaypoint.Connections){
            connection.Disconnect(selectedWaypoint);
        }

        DestroyImmediate(selectedWaypoint.gameObject);
    }

    private void ConnectToNewWaypoint(List<Waypoint> waypoints){
        GameObject waypointObject = new GameObject("Waypoint", typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);
        
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
        a.Disconnect(b);
        b.Disconnect(a);
    }

    private void RemoveWaypoints(List<Waypoint> waypoints){
        foreach(Waypoint waypoint in waypoints){
            RemoveWaypoint(waypoint);
        }
    }

    private void WaypointToLocation(List<Waypoint> waypoints){
        foreach(Waypoint waypoint in waypoints){
            if(waypoint.GetType() != typeof(Location)) MakeLocation(waypoint);
        }
    }

    private void LocationToWaypoint(List<Waypoint> waypoints){
        foreach(Waypoint waypoint in waypoints){
            if(waypoint.GetType() == typeof(Location)) MakeWaypoint(waypoint);
        }
    }

    private void MakeLocation(Waypoint selectedWaypoint){
        GameObject selectedObj = selectedWaypoint.gameObject;
        Location location = selectedObj.AddComponent<Location>();
        
        location.Set(selectedWaypoint);

        foreach(Waypoint connection in selectedWaypoint.Connections){
            connection.Connections.Remove(selectedWaypoint);
            connection.Connections.Add(location);
        }

        selectedWaypoints.Remove(selectedWaypoint);
        selectedWaypoints.Add(location);

        DestroyImmediate(selectedWaypoint);
    }

    private void MakeWaypoint(Waypoint selectedLocation){
        GameObject selectedObj = selectedLocation.gameObject;
        Waypoint waypoint = selectedObj.AddComponent<Waypoint>();
        
        waypoint.Set(selectedLocation);

        foreach(Waypoint connection in selectedLocation.Connections){
            connection.Connections.Remove(selectedLocation);
            connection.Connections.Add(waypoint);
        }

        selectedWaypoints.Remove(selectedLocation);
        selectedWaypoints.Add(waypoint);

        DestroyImmediate(selectedLocation);
    }

}
