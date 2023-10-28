using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    //A Request manager for pathfindiing
    //PathRequestManager requestManager;

    //The grid on the map
    AGrid grid;

    void Awake()
    {
        //Get the request manager and grid
        //requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<AGrid>();
    }

    IEnumerator FindPath(Vector3 startPosition, Vector3 targetPosition, PathRequestManager requestManager)
    {
        //All the waypoints in the path
        Vector3[] waypoints = new Vector3[0];
        //If find the path successfully
        bool pathSuccess = false;

        //Get the node 
        ANode startNode = grid.NodeFromWorldPoint(startPosition);
        ANode targetNode = grid.NodeFromWorldPoint(targetPosition);

        //Check if the start node and target node walkable
        if (startNode.walkable && targetNode.walkable)
        {

            Heap<ANode> openSet = new Heap<ANode>(grid.MaxSize);
            HashSet<ANode> closedSet = new HashSet<ANode>();

            openSet.Add(startNode);

            //Keep checking nodes in Open List
            while (openSet.Count > 0)
            {
                ANode currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                //Found the shortest path successfully
                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                //Check each neighbour
                foreach (ANode neighbour in grid.GetNeighbours(currentNode))
                {
                    //Discard unwalkable and closedlist neighbour
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    //Check if current node should be this neighbour's parent node
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        //If add the neighbour to the open list
                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }

        }
        yield return null;

        //If found path, retrace the path to waypoints
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);

        Resources.UnloadUnusedAssets();
    }

    //Calculate distance between two nodes
    int GetDistance(ANode nodeA, ANode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    //Retrace all nodes' parents and form a list of waypoints
    Vector3[] RetracePath(ANode startNode, ANode endNode)
    {
        List<ANode> path = new List<ANode>();
        ANode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] waypoints = SimplifyPath(path, startNode);

        Array.Reverse(waypoints);
        return waypoints;

    }

    //Check if need to change direction and delete some waypoints
    Vector3[] SimplifyPath(List<ANode> path, ANode startNode)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;
        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i - 1].worldPosition); //Changed from path[i] to path[i-1]
            }
            directionOld = directionNew;
            if (i == path.Count - 1 && directionOld != new Vector2(path[i].gridX, path[i].gridY) - new Vector2(startNode.gridX, startNode.gridY))
                waypoints.Add(path[path.Count - 1].worldPosition);
        }
        return waypoints.ToArray();

    }

    //Called by PathRequestManager and start the coroutine
    public void StartFindPath(Vector3 startPos, Vector3 targetPos, PathRequestManager requestManager)
    {
        StartCoroutine(FindPath(startPos, targetPos, requestManager));
    }


}
