using System.Collections.Generic;
using UnityEngine;

public class WaypointRoot : MonoBehaviour
{   
    [Tooltip("Color of connections between waypoints.")]
    public Color ConnectionColor = Color.white;

    [Tooltip("It limits loop count.")]
    public int PathfindingSearchLimit = 10000;
    
    public List<Waypoint> FindWaypointsWithTag(string tag){
        List<Waypoint> waypoints = new List<Waypoint>();

        foreach(Transform child in transform){
            if(child.CompareTag(tag) && child.GetComponent<Waypoint>()) waypoints.Add(child.GetComponent<Waypoint>());
        }

        if(waypoints.Count > 0)return waypoints;
        return null;
    }

    public List<Waypoint> FindWaypointsWithName(string name){
        List<Waypoint> waypoints = new List<Waypoint>();

        foreach(Transform child in transform){
            if(child.name.Equals(name) && child.GetComponent<Waypoint>()) waypoints.Add(child.GetComponent<Waypoint>());
        }

        if(waypoints.Count > 0)return waypoints;
        return null;
    }

    public Waypoint GetNearestWaypoint(Vector3 pos){
        Waypoint nearestWaypoint = null;
        float nearestDistance = Mathf.Infinity;

        foreach(Transform child in transform){
            if(child.GetComponent<Waypoint>()){
                float distance = (pos - child.position).magnitude;
                if(distance < nearestDistance){
                    nearestWaypoint = child.GetComponent<Waypoint>();
                    nearestDistance = distance;
                }
            }
        }

        return nearestWaypoint;
    }

    public Waypoint GetFarestWaypoint(Vector3 pos){
        Waypoint farestWaypoint = null;
        float farestDistance = -1;
        
        foreach(Transform child in transform){
            if(child.GetComponent<Waypoint>()){
                float distance = (pos - child.position).magnitude;
                if(distance > farestDistance){
                    farestWaypoint = child.GetComponent<Waypoint>();
                    farestDistance = distance;
                }
            }
        }

        return farestWaypoint;
    }

}

