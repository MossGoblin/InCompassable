using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathFinderTest : MonoBehaviour
{
    // Refs
    public Transform playerOne;

    public FloorTest floor;
    public Transform spawnPoints;

    public int minFreeNbrs = 3;
    public int padding = 2;

    // Init lists
    public List<Node> openList { get; set; }

    public List<Node> closedList { get; set; }

    public Node startNode;
    public Node endNode;


    public void CreateSpawns(int[,] grid, float maxPointDeviation)
    {
        // Select an available point for starting the flood
        Vector3 floodPoint = ScoutAround(grid, new Vector3(grid.GetLength(0) / 2, 0, grid.GetLength(1) / 2));
        //Vector3 floodPoint = SelectAvailablePoint(grid, paddingWidthLeft, paddingWidthRight, paddingDepthLeft, paddingDepthRight);

        playerOne.transform.position = floodPoint;
        playerOne.gameObject.SetActive(true);
        // start.parent = spawnPoints;
        // playerOne.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.yellow;
        Debug.Log($"Player position: {playerOne.transform.position}");

    }


    private Vector3 ScoutAround(int[,] grid, Vector3 initPoint)
    {
        int positionWidth = (int)initPoint.x;
        int positionDepth = (int)initPoint.z;
        bool searching = true;
        if (!AccessibleSpot(grid, new Vector3(positionWidth, 0, positionDepth))) // if not accessible
        {
            int searchRadius = 1;
            bool found = false;
            while (searching)
            {
                // create width cooridinate map - only deltas!!
                int coordinateMapLength = searchRadius * 4 + 1; // from start to end and half back
                int[] coordinateMap = new int[coordinateMapLength];
                for (int countA = 0; countA < searchRadius * 2 + 1; countA += 1)
                {
                    coordinateMap[countA] = countA - searchRadius;
                }
                for (int countB = 0; countB < searchRadius * 2; countB += 1)
                {
                    // counting backwards from the right most side of the coordinate map
                    coordinateMap[searchRadius * 2 + 1 + countB] = searchRadius - 1 - countB;
                }

                // iterate area perimeter:
                // the map gives W coordinate directly
                // D coordinate lead with searchRadius steps

                for (int countC = 0; countC < coordinateMap.Length - searchRadius; countC += 1)
                {
                    int searchPositionW = positionWidth + coordinateMap[countC];
                    int searchPositionDIndex = (countC + searchRadius); // no looping, simply add the search radius
                    int searchPositionD = positionDepth + coordinateMap[searchPositionDIndex];
                    if (AccessibleSpot(grid, new Vector3(searchPositionW, 0, searchPositionD)))
                    {
                        searching = false;
                        found = true;
                        positionWidth = searchPositionW;
                        positionDepth = searchPositionD;
                        break;
                    }
                    Debug.Log($"{searchPositionW},{searchPositionD}");
                    // DebugColor(new Vector3(searchPositionW, 0, searchPositionD), Color.cyan);

                }

                if (!found)
                {
                    searchRadius += 1;
                }
            }
        }
        return new Vector3(positionWidth, 0, positionDepth);
    }

    public void DebugColor(Vector3 position, Color color)
    {
        Transform[] flatGrid = floor.gridFillObj.Cast<Transform>().ToArray();
        Transform obj = Array.Find(flatGrid, o => o.transform.position.x == position.x && o.transform.position.z == position.z);
        if (obj != null)
        {
            obj.GetComponent<MeshRenderer>().material.color = color;
        }
    }

    private bool AccessibleSpot(int[,] grid, Vector3 point) // TODO Uses two grids; may be pass a grid collection ??
    {
        int width = (int)point.x;
        int depth = (int)point.z;
        // Sanity check
        if (width <= 0 || width >= grid.GetLength(0) || depth <= 0 || depth >= grid.GetLength(1))
        {
            return false;
        }

        if (grid[width, depth] == 1) // fail - occupied position
        {
            return false;
        }

        // count free nbrs
        int freeNbrs = 0;
        int[] pos = new int[4] { 0, 1, 0, -1 };
        for (int count = 0; count < 4; count += 1)
        {
            int posD = pos[count];
            int posW = pos[(count + 3) % 4];
            // check if nbr is in the border
            if ((depth + posD <= 2) || (depth + posD >= (floor.depth - 2)))
            {
                continue;
            }
            if ((width + posW <= 2) || (width + posW >= (floor.width - 2)))
            {
                continue;
            }
            // if nbr is occupied
            if (floor.finalGrid[width + posW, depth + posD] == 1)
            {
                continue;
            }
            // if nbr is POI
            if (floor.gridPOI[width + posW, depth + posD] == 1)
            {
                continue;
            }

            freeNbrs += 1;
        }

        if (freeNbrs >= minFreeNbrs)
        {
            return true;
        }
        return false;
    }
}