using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class Waypoint : MonoBehaviour
{   
    [Range(0f, 5f)]
    public float Radius = .5f;
    [SerializeField] private List<Waypoint> _connections = new List<Waypoint>();

    ///<summary>
    ///You can't modify the list but you can access the elements.<br/>
    ///Use Connect and Disconnect functions to modify list.
    ///</summary>
    public List<Waypoint> Connections => new List<Waypoint>(_connections);

    ///<summary>
    ///Get Random position in Radius.
    ///</summary>
    public Vector3 GetPosition{
        get{
            Vector2 insideCircle = Random.insideUnitCircle * Radius;
            return transform.right * insideCircle.x + transform.forward * insideCircle.y;
        }
    }

    public Vector3 LeftBound => transform.position - transform.right * Radius;
    public Vector3 RightBound => transform.position + transform.right * Radius;
    public void Connect(Waypoint waypoint) => _connections.Add(waypoint);
    public bool Disconnect(Waypoint waypoint) => _connections.Remove(waypoint);
    public bool Has(Waypoint waypoint){
        foreach(Waypoint connection in _connections){
            if(connection == waypoint) return true;
        }
        return false;
    }

    public void Set(Waypoint waypoint){
        Radius = waypoint.Radius;
        _connections = new List<Waypoint>(waypoint.Connections);
    }

    void OnDestroy(){
        foreach(Waypoint connection in _connections){
            #if UNITY_EDITOR
                if(connection != null) Undo.RegisterCompleteObjectUndo(connection, "disconnected");
            #endif

            connection.Disconnect(this);
        }
    }
}
