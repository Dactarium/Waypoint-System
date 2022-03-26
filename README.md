# Waypoint System
 Unity Waypoint System for NPCs, Vecihles and etc.

# Index
- [Getting Started](#getting-started)<br/>
- [Creating new waypoint](#creating-new-waypoint)<br/>
- [Selected waypoint utilities]()<br/>
- [Selected multiple waypoints utilities]()<br/>
- [Usage On Script](#usage-on-script)
  - [Waypoint Class](#waypoint-class)
    - [Public Values](#public-values)
    - [Set (Waypoint waypoint)](#set-waypoint-waypoint)
    - [Connect/Disconnect (Waypoint waypoint)](#connectdisconnect-waypoint-waypoint)
    - [Has (Waypoint waypoint)](#has-waypoint-waypoint))
    - [FindConnectionsWithTag (string tag) & FindConnectionsWithName (string name)](#findconnectionswithtag-string-tag--findconnectionswithname-string-name)
  - [WaypointNavigator](#waypointnavigator)
    - [Navigate (Waypoint start, Waypoint destination)](#navigate-waypoint-start-waypoint-destination)
  - [WaypointRoot](#waypointroot)
    - [FindWaypointsWithTag (string tag) & FindWaypointsWithName (string name)](#findwaypointswithtag-string-tag--findwaypointswithname-string-name)
    - [GetNearestWaypoint (Vector3 pos)](#getnearestwaypoint-vector3-pos)
    - [GetFarestWaypoint (Vector3 pos)](#getfarestwaypoint-vector3-pos)
- [Credits](#credits)

# Getting Started

Download "WaypointSystem.unitypackage" and import to unity

# Creating new waypoint

**Open Tools/Waypoint Editor**

![image](https://user-images.githubusercontent.com/75855560/158930752-47dcc242-c1b4-45f2-beb1-cf13c391b01b.png)

**Press Create New Waypoint Root**

![image](https://user-images.githubusercontent.com/75855560/160245505-76412431-f79b-49dc-acb0-7eb540427f5a.png)

**Press Create Waypoint Button**

![image](https://user-images.githubusercontent.com/75855560/160245544-cd4633b9-3538-4c8a-acbd-42fb5ea0b240.png)

# Selected Waypoint Utilities

![image](https://user-images.githubusercontent.com/75855560/160245579-51a38b45-399d-4284-8633-d3dfb7c68573.png)

| Button | Description |
| --- | --- |
| Create waypoint from selected | Creates new waypoint and connects selected waypoint |
| Remove selected waypoint | Removes and disconnects selected waypoint |

# Selected Multiple Waypoints Utilities

![image](https://user-images.githubusercontent.com/75855560/160245603-7fd60f2e-713a-48bf-8de9-49e045e2bd62.png)

| Button | Description |
| --- | --- |
| Connect to New Waypoint | Creates new waypoint and connects selected waypoints |
| Connect Selected Waypoints |  Connects selected waypoints eachother |
| Disconnect Selected Waypoints | Donnects selected waypoints eachother |
| Remove Selected Waypoints |  Removes and disconnects selected waypoints |

Ps. Make sure Waypoint Selection is True

# Usage On Script
## Waypoint Class
### Public Values
&nbsp;&nbsp;&nbsp;&nbsp;**Radius (get/set):** Radius of Waypoint. It affects **Random Position**.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;**Connections (get):** Returns connections of waypoint.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;**LeftBound (get):** Returns left bounds world position of waypoint.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;**RightBound (get):** Returns right bounds world position of waypoint.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;**RandomPosition (get):** Returns Random world position in radius of waypoint.<br/>

---

### Set (Waypoint waypoint)
Sets other waypoints radius and connections to waypoint
```Csharp
waypoint.Set(other);
```

---

### Connect/Disconnect (Waypoint waypoint)
Adds or Removes Waypoint from Connections
```Csharp
waypoint.Connect(other);
```
You should use Has method before using and you should write otherWaypoint.Connect(waypoint);
```Csharp
waypoint.Disconnect(other);
```
You should write otherWaypoint.Disconnect(waypoint);
<br/>

---

### Has (Waypoint waypoint)
Checks is waypoint in Connections and returns bool value
```Csharp
waypoint.Has(other);
```

---

### FindConnectionsWithTag (string tag) & FindConnectionsWithName (string name)
Finds Connections with tag or name and returns list of waypoint
```Csharp
waypoint.FindConnectionsWithTag(tag); // If there is no connection with tag returns null
```
```Csharp
waypoint.FindConnectionsWithName(name); // If there is no connection with name returns null
```
## WaypointNavigator
### Navigate (Waypoint start, Waypoint destination)
Finds route for start to destination waypoint with A* Pathfinding Algorithm and returns stack of waypoint.
```Csharp
WaypointNavigator.Navigate(waypoint, other);
```
## WaypointRoot
### FindWaypointsWithTag (string tag) & FindWaypointsWithName (string name)
Finds child waypoints with tag or name and returns list of waypoint.
```Csharp
waypointRoot.FindWaypointsWithTag(tag); // If there is no connection with tag returns null
```
```Csharp
waypointRoot.FindWaypointsWithName(name); // If there is no connection with name returns null
```

---

### GetNearestWaypoint (Vector3 pos)
Returns nearest waypoint given position.
```Csharp
waypointRoot.GetNearestWaypoint(position);
```

---

### GetFarestWaypoint (Vector3 pos)
Returns nearest waypoint given position.
```Csharp
waypointRoot.GetFarestWaypoint(position);
```
# Credits
I learned how system works from [Game Dev Guide](https://www.youtube.com/channel/UCR35rzd4LLomtQout93gi0w)'s [Building a Traffic System](https://www.youtube.com/watch?v=MXCZ-n5VyJc&list=PL5OiuuZeHt3q3KePW2i-WyRUy9ofhXo0B&index=3) video.
