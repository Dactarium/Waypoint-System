using UnityEngine;
using UnityEditor;

[InitializeOnLoad()]
public class WaypointEditor 
{   
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(Waypoint waypoint, GizmoType gizmoType){
        if((gizmoType & GizmoType.Selected) != 0){
            Gizmos.color = Color.green;
        }else{
            Gizmos.color = Color.white;
        }

        Gizmos.DrawSphere(waypoint.transform.position, .1f);

        
        Handles.color = Color.yellow;
        if(waypoint.GetType() == typeof(Location)) Handles.color = Color.red;

        Handles.DrawSolidDisc(waypoint.transform.position, waypoint.transform.up, waypoint.Radius);

        if(waypoint.Connections.Count > 0){
            Vector3 sourceForward = waypoint.transform.forward;

            foreach(Waypoint connection in waypoint.Connections){
                Gizmos.color = Color.yellow;

                Vector3 targetForward = connection.transform.forward;
                
                waypoint.transform.LookAt(connection.transform);
                connection.transform.LookAt(waypoint.transform);

                Gizmos.DrawLine(waypoint.MinBound, connection.MaxBound);
                Gizmos.DrawLine(waypoint.MaxBound, connection.MinBound);

                connection.transform.forward = targetForward;
            }

            waypoint.transform.forward = sourceForward;
        }

    }

}
