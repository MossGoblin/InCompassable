using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathFinder : MonoBehaviour
{
    // Refs
    public Transform marker;

    public Floor floor;
    public Transform spawnPoints;

    public int minFreeNbrs = 2;
    public int padding = 2;

    // Init lists
    public List<Node> openList { get; set; }

    public List<Node> closedList { get; set; }

    public Node startNode;
    public Node endNode;


    public void CreateSpawns(int[,] grid, float paddingWidthLeft, float paddingWidthRight, float paddingDepthBelow, float paddingDepthAbove)
    {
        // Select an available point for starting the flood
        Vector3 floodPoint = ScoutAround(grid, new Vector3(grid.GetLength(0) / 2, 0, grid.GetLength(1) / 2));
        //Vector3 floodPoint = SelectAvailablePoint(grid, paddingWidthLeft, paddingWidthRight, paddingDepthLeft, paddingDepthRight);

        // Floodfill the plane
        List<Vector3> floodPlane = FloodThePlane(grid, floodPoint);

        // TODO - check if we flooded at least half the plane
        int planeSize = grid.GetLength(0) * grid.GetLength(1);
        if (floodPlane.Count < planeSize / 2)
        {
            Debug.Log($"Flooded only {floodPlane.Count} out of {planeSize} cells");
            return;
        }

        // Select points in the flood
        // Calculate padding
        int borderLeft = (int)(grid.GetLength(0) / 2 * paddingWidthLeft);
        int borderRight = (int)(grid.GetLength(0) / 2 * 3 * paddingWidthRight);
        int borderBelow = (int)(grid.GetLength(1) / 2 * paddingDepthBelow);
        int borderAbove = (int)(grid.GetLength(1) / 2 * 3 * paddingDepthAbove);

        int width = (int)((borderRight - borderLeft) / 2);
        int depth = (int)((borderAbove - borderBelow) / 2);
        Vector3 centerOfLeftArea = new Vector3(width, 0, depth);

        // Create a dictionary out of the floodList - set value to distance to firstPoint
        Dictionary<Vector3, float> floodDistance = new Dictionary<Vector3, float>();
        foreach(Vector3 point in floodPlane)
        {
            Vector3 distance = (point - centerOfLeftArea).normalized;
            float distance_magnitude = distance.x + distance.z;
            floodDistance.Add(point, distance_magnitude);
        }

        // Order the dictionary
        Dictionary<Vector3, float> orderedDict = floodDistance.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        Vector3 firstPoint = orderedDict.FirstOrDefault().Key;
        DebugColor(firstPoint, Color.yellow);
        Debug.Log(firstPoint);

    }


    private List<Vector3> FloodThePlane(int[,] grid, Vector3 floodPoint)
    {
        bool flooding = true;

        Vector3 currentPoint = floodPoint;
        List<Vector3> dry = new List<Vector3>();
        List<Vector3> flooded = new List<Vector3>();

        dry.Add(currentPoint);

        while (flooding)
        {
            // Get a point from teh dry list and mark it as flooded
            currentPoint = dry.First();
            dry.Remove(currentPoint);
            flooded.Add(currentPoint);

            DebugColor(currentPoint, Color.cyan);



            // get all nbrs of the current point and add the to the dry list if not already flooded
            List<Vector3> nbrs = floor.GetNbrs(currentPoint);
            if (nbrs.Count > 0)
            {
                foreach (Vector3 nbr in nbrs)
                {
                    if (!flooded.Contains(nbr) && !dry.Contains(nbr))
                    {
                        dry.Add(nbr);
                    }
                }
            }

            if (dry.Count == 0)
            {
                // EXIT with a grace
                flooding = false;
                Debug.Log($"Plane flooded : {flooded.Count}");
            }
        }

        return flooded;
    }

    public void CreateSpawnPoints()
    {
        Debug.Log("Spawning points...");

        // TODO ALGO
        // Pick a point inside one half of the floor
        // Check if available
        // If not - start searching in an expanding circle around the first point

        // Set up variables
        int startWidthHalf = 0;
        int startDepthHalf = 0;
        int halfOfWidth = 0;
        int halfOfDepth = 0;
        int startPositionWidth = 0;
        int startPositionDepth = 0;
        int endPositionWidth = 0;
        int endPositionDepth = 0;

        halfOfWidth = floor.width / 2;
        halfOfDepth = floor.depth / 2;

        // Pick a starting point
        startWidthHalf = Random.Range(0, 1);
        startDepthHalf = Random.Range(0, 1);
        // Somewhere within one of the quadrants, determined by startWidthHalf and startDepthHalf
        startPositionWidth = startWidthHalf * halfOfWidth + Random.Range(padding, halfOfWidth - padding);
        startPositionDepth = startDepthHalf * halfOfDepth + Random.Range(padding, halfOfDepth - padding);

        // find a spot for the start point
        //Vector3 startPositionVector = ScoutAround(startPositionWidth, startPositionDepth);
        //Debug.Log($"start: {startPositionVector.x}/{startPositionVector.z}");

        // Repeat for endPoint
        int endWidthHalf = 1 - startWidthHalf;
        int endDepthHalf = 1 - startDepthHalf;
        endPositionWidth = Random.Range(padding, halfOfWidth - padding) + endWidthHalf * halfOfWidth;
        endPositionDepth = Random.Range(padding, halfOfDepth - padding) + endDepthHalf * halfOfDepth;

        // find a spot for the start point
        //Vector3 endPositionVector = ScoutAround(endPositionWidth, endPositionDepth);
        //Debug.Log($" end: {endPositionVector.x}/{endPositionVector.z}");


        // Remove old markers
        foreach (Transform obj in spawnPoints)
        {
            Destroy(obj.gameObject);
        }

        // Place markers
        //Transform start = Instantiate(marker, startPositionVector, Quaternion.identity);
        //start.parent = spawnPoints;
        //start.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.yellow;
        //Transform end = Instantiate(marker, endPositionVector, Quaternion.identity);
        //end.parent = spawnPoints;
        //end.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.green;

        // Create start and end nodes
        //startNode = new Node(new Vector3(start.transform.position.x, 0, start.transform.position.z), null, end.position);
        //endNode = new Node(new Vector3(end.transform.position.x, 0, end.transform.position.z), null, end.position);

        //// LogDump spawn positions
        //Debug.Log($"start: {start.transform.position}");
        //Debug.Log($"end: {end.transform.position}");
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
                    DebugColor(new Vector3(searchPositionW, 0, searchPositionD), Color.cyan);

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
        // Sanily check
        if (width <= 0 || width >= floor.finalGrid.GetLength(0) || depth <= 0 || depth >= floor.finalGrid.GetLength(1))
        {
            return false;
        }

        // Regular check
        if (floor.finalGrid[width, depth] == 1) // fail - occupied position
        {
            return false;
        }

        // POI check
        if (floor.gridPOI[width, depth] == 1) // fail - place is POI
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

public class Node
{
    public Vector3 position { get; set; }
    public Node parent { get; private set; }
    public float gScore { get; set; }
    public float hScore { get; private set; }
    public float fScore
    {
        get
        {
            return gScore + hScore;
        }
    }

    public Node(Vector3 pos, Node parent, Vector3 endNodePosition)
    {
        this.position = pos;
        hScore = GetDistance(this.position, endNodePosition);
        if (parent == null)
        {
            gScore = 0;
        }
        else
        {
            this.parent = parent;
            gScore = parent.gScore + 1;
        }
    }

    public void SetParent(Node parent)
    {
        if (parent == null)
        {
            gScore = 0;
        }
        else
        {
            this.parent = parent;
            gScore = parent.gScore + 1;
        }
    }
    public float GetDistance(Vector3 origin, Vector3 target)
    {
        // Simlified version
        Vector3 dist = origin - target;
        float distX = Mathf.Abs(dist.x);
        float distZ = Mathf.Abs(dist.z);
        int smaller = (int)Mathf.Min(distX, distZ);
        int larger = (int)Mathf.Max(distX, distZ);

        float score = (float)(smaller * 1.4 + (larger - smaller));
        // Euclidian version
        //Vector3 dist = origin - target;
        //float score = Mathf.Sqrt((dist.x * dist.x) + (dist.z * dist.z));

        return score;
    }
}