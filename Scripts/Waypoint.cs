using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class Waypoint : MonoBehaviour
{   
    [Range(0f, 5f)]
    public float Radius = .5f;
    public Color Color = Color.yellow;
    [SerializeField] private List<Waypoint> _connections = new List<Waypoint>();

    ///<summary>
    ///You can't modify the list but you can access the elements.<br/>
    ///Use Connect and Disconnect functions to modify list.
    ///</summary>
    public List<Waypoint> Connections => new List<Waypoint>(_connections);

    public Vector3 LeftBound => transform.position - transform.right * Radius;
    public Vector3 RightBound => transform.position + transform.right * Radius;

    ///<summary>
    ///Get Random position in Radius.
    ///</summary>
    public Vector3 RandomPosition{
        get{
            Vector2 insideCircle = Random.insideUnitCircle * Radius;
            return transform.position + Vector3.right * insideCircle.x + Vector3.forward * insideCircle.y;
        }
    }

    ///<summary>
    ///Sets Waypoint's Radius and Connections
    ///</summary>
    public void Set(Waypoint waypoint){
        Radius = waypoint.Radius;
        _connections = new List<Waypoint>(waypoint.Connections);
    }

    public void Connect(Waypoint waypoint) => _connections.Add(waypoint);
    public bool Disconnect(Waypoint waypoint) => _connections.Remove(waypoint);

    ///<summary>
    ///Checks is Waypoint in The Connections
    ///</summary>
    public bool Has(Waypoint waypoint){
        foreach(Waypoint connection in _connections){
            if(connection == waypoint) return true;
        }
        return false;
    }

    public List<Waypoint> FindConnectionsWithTag(string tag){
         List<Waypoint> waypoints = new List<Waypoint>();

        foreach(Waypoint connection in _connections){
            if(connection.CompareTag(tag)) waypoints.Add(connection);
        }

        if(waypoints.Count > 0)return waypoints;
        return null;
    }

    public List<Waypoint> FindConnectionsWithName(string name){
         List<Waypoint> waypoints = new List<Waypoint>();

        foreach(Waypoint connection in _connections){
            if(connection.name.Equals(name)) waypoints.Add(connection);
        }

        if(waypoints.Count > 0)return waypoints;
        return null;
    }
    
    void OnDestroy(){
        foreach(Waypoint connection in _connections){
            #if UNITY_EDITOR
                if(connection != null) Undo.RegisterCompleteObjectUndo(connection, "Disconnected from destoryed object");
            #endif

            connection.Disconnect(this);
        }
    }
}
