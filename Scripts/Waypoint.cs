using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{   
    [Range(0f, 5f)]
    public float Radius = .5f;
    public List<Waypoint> Connections = new List<Waypoint>();
    public Vector3 GetPosition{
        get{
            Vector2 insideCircle = Random.insideUnitCircle * Radius;
            return transform.right * insideCircle.x + transform.forward * insideCircle.y;
        }
    }
    public Vector3 MinBound => transform.position - transform.right * Radius;
    public Vector3 MaxBound => transform.position + transform.right * Radius;
    public void Connect(Waypoint waypoint) => Connections.Add(waypoint);
    public bool Disconnect(Waypoint waypoint) => Connections.Remove(waypoint);
    public bool Has(Waypoint waypoint){
        foreach(Waypoint connection in Connections){
            if(connection == waypoint) return true;
        }

        return false;
    }

    public void Set(Waypoint waypoint){
        Radius = waypoint.Radius;
        Connections = new List<Waypoint>(waypoint.Connections);
    }
}
