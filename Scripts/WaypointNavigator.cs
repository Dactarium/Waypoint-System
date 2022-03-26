using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class WaypointNavigator{

        ///<summary>
        ///Returns route to destination waypoint.<br/>
        ///If cant reach destination returns route to calculated lowest f cost waypoint.
        ///</summary>
        public static Stack<Waypoint> Navigate(Waypoint start, Waypoint destination){

            Stack<Waypoint> route = new Stack<Waypoint>();
            if(start == destination){
                Debug.LogWarning("Target destination is self!");
                route.Push(start);
                return route;
            }

            int pahtfindingSearchLimit = start.transform.parent.GetComponent<WaypointRoot>().PathfindingSearchLimit;

            Vector3 destinationPos = destination.transform.position;

            List<NavigatorNode> calculatedNodes = new List<NavigatorNode>();
            List<NavigatorNode> closedNodes = new List<NavigatorNode>();
            calculatedNodes.Add(new NavigatorNode(start, destinationPos));

            NavigatorNode destinationNode = null;
            int loopCount = 0;
            do{
                NavigatorNode lowestF = calculatedNodes[0];
                calculatedNodes.Remove(lowestF);
                closedNodes.Add(lowestF);

                foreach(Waypoint connection in lowestF.waypoint.Connections){
                    if(connection == destination){
                        destinationNode = new NavigatorNode(destination, lowestF, destinationPos);
                        break;
                    }
                    if(GetNode(connection, closedNodes) != null) continue;

                    NavigatorNode connectionNode = GetNode(connection, calculatedNodes);

                    if(connectionNode == null){

                        connectionNode = new NavigatorNode(connection, lowestF, destinationPos);
                        calculatedNodes.Add(connectionNode);

                    }else{
                        
                        connectionNode.checkG(lowestF);

                    }

                    calculatedNodes = calculatedNodes.OrderBy(o => o.F).ToList();
                }

            }while(destinationNode == null && loopCount++ < pahtfindingSearchLimit);

            NavigatorNode currentNode = destinationNode;
            if(currentNode == null) currentNode = calculatedNodes[0];

            loopCount = 0;
            do{
                route.Push(currentNode.waypoint);
                currentNode = currentNode.parent;
            }while(currentNode != null && loopCount++ < pahtfindingSearchLimit);
            
            return route;
        }

        private static NavigatorNode GetNode(Waypoint waypoint, List<NavigatorNode> nodes){
            foreach(NavigatorNode node in nodes){
                if(node.waypoint == waypoint) return node;
            }
            return null;
        }

        public class NavigatorNode{
            public Waypoint waypoint {get; private set;}
            public NavigatorNode parent {get; private set;}
            private float g;
            private float h;
            public float F {get; private set;}

            public NavigatorNode(Waypoint waypoint, Vector3 destination){
                this.waypoint = waypoint;
                this.parent = null;
                this.g = 0;
                h = (waypoint.transform.position - destination).magnitude;
                calculateF();
            }
            public NavigatorNode(Waypoint waypoint, NavigatorNode parent, Vector3 destination){
                this.waypoint = waypoint;
                this.parent = parent;
                this.g = parent.g + (waypoint.transform.position - parent.waypoint.transform.position).magnitude;
                h = (waypoint.transform.position - destination).magnitude;
                calculateF();
            }

            public void checkG(NavigatorNode previous){
                float newG = previous.g + (waypoint.transform.position - previous.waypoint.transform.position).magnitude;
                if(newG < g){
                    g = newG;
                    parent = previous;
                    calculateF();
                }
            }

            private void calculateF() =>  F = g + h;
        }
    }
