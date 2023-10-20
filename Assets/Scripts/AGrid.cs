using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGrid : MonoBehaviour
{
    //Test: Player location
    public Transform player;
    //Call unwalkable mask
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    //how much space each node covers
    public float nodeRadius;
    //Gap between cube
    public float gizmosDensity = .05f;
    //2 Dimension grid
    ANode[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    public bool displayGridGizmos;

    void Awake()
    {
        //Calculate numbers of grids
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector2(gridWorldSize.x, gridWorldSize.y));
        if (grid != null && displayGridGizmos)
        {
            ANode playerNode = NodeFromWorldPoint(player.position);

            foreach (ANode n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.yellow : Color.red;
                //Player node
                if (playerNode == n)
                {
                    Gizmos.color = Color.blue;
                }
                Gizmos.DrawCube(n.worldPosition, Vector2.one * (nodeDiameter - gizmosDensity));
            }
        }
    }


    void CreateGrid()
    {
        //Create grid based on the grid size
        grid = new ANode[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics2D.OverlapBox(worldPoint, new Vector2(nodeDiameter - gizmosDensity, nodeDiameter - gizmosDensity), 0f, unwalkableMask));
                grid[x, y] = new ANode(walkable, worldPoint, x, y);
            }
        }
    }

    public ANode NodeFromWorldPoint(Vector3 worldPosition)
    {
        //Change a world position to a percentage of a length of X of Y in the grid
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;

        //Clamp01:Clamps value between 0 and 1 and returns value.
        //If the value is negative then zero is returned. If value is greater than one then one is returned.
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        //Get the node's x and y for the world position
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    public List<ANode> GetNeighbours(ANode node)
    {
        List<ANode> neighbours = new List<ANode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                //This is the current checking node instead of the neighbour
                if (x == 0 && y == 0)
                    continue;

                //If is neighbour, check if the neighbour is in the grid
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);

                }
            }
        }


        return neighbours;
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }
}
