using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Random = UnityEngine.Random;

public class PathFinder : MonoBehaviour
{
    // Refs
    public Transform marker;

    public Floor floor;
    public Transform spawnPoints;

    // Init lists
    public List<Node> openList { get; set; }

    public List<Node> closedList { get; set; }

    public Node startNode;
    public Node endNode;

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
    public void SetUpStartingPoints()
    {
        CreateSpawnPoints();
        FindPath();
    }

    public IEnumerator FindPath()
    {
        // Decolorize all objects
        foreach (Transform obj in floor.gridFillObj)
        {
            obj.GetComponent<MeshRenderer>().material.color = Color.gray;
        }

        Debug.Log("On The Path...");

        int availablePositions = floor.GetAllAvailablePositions();
        Debug.Log($"{availablePositions}");

        // Init lists
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        // Add startNode to openList
        openList.Add(startNode);

        // DBG Debug section
        float maxFScore = 0f;

        // Loop
        bool searching = true;
        bool foundPath = false;
        while (openList.Count > 0)
        {
            // If the open list is empty - exit with no path
            if (openList.Count == 0)
            {
                Debug.Log("NO PATH");
                searching = false;
                break;
            }
            // Get current node from openList - lowest fScore
            Node currentNode = openList.OrderBy(n => n.fScore).FirstOrDefault();
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // DBG Debug section
            maxFScore = Mathf.Max(currentNode.fScore, maxFScore);
            // Debug.Log($"{currentNode.position.x},{currentNode.position.z} / {endNode.position.x},{endNode.position.z} : {currentNode.fScore}/{maxFScore}");
            // Debug only
            if (floor.gridFillObj[(int)currentNode.position.x, (int)currentNode.position.z] != null)
            {
                floor.gridFillObj[(int)currentNode.position.x, (int)currentNode.position.z].GetComponent<MeshRenderer>().material.color = Color.red;
            }

            if ((int)currentNode.position.x == (int)endNode.position.x && (int)currentNode.position.z == (int)endNode.position.z)
            {
                Debug.Log($"REACHED END {currentNode.position}");
                searching = false;
                foundPath = true;
                break;
            }

            // Get nbrs of current node
            List<Vector3> nbrPositions = floor.GetNbrs(currentNode);
            // Iterate nbrs of current node
            foreach (Vector3 nbrPos in nbrPositions)
            {
                // if there is a closed Node at that position - skip it
                Node possiblyClosed = closedList.FirstOrDefault(n => n.position == nbrPos);
                if (possiblyClosed != null)
                {
                    continue;
                }

                // check if there is an open node at that position
                Node possiblyOpen = openList.FirstOrDefault(n => n.position == nbrPos);
                if (possiblyOpen != null) // there is an open node at that position - possible update
                {
                    // check if the path through the current node to nbr is shorter
                    if (possiblyOpen.gScore > currentNode.gScore + 1) // shorter way to the nbr via the current node - update parent and gScore
                    {
                        possiblyOpen.SetParent(currentNode); // this should also recalculate gScore
                    }
                }
                else // No node at this position - create and add to open list
                {
                    Node newNode = new Node(nbrPos, currentNode, endNode.position);
                    openList.Add(newNode);
                }
            }


            // TODO HERE
            //Debug.Log($"o: {openList.Count} / c: {closedList.Count} / a: {availablePositions}");
            yield return null;

        }

        if (foundPath)
        {
            // TODO retrace path
        }
    }

    public void CreateSpawnPoints()
    {
        Debug.Log("Spawning points...");

        // TODO ALGO
        // Pick a point inside one half of the floor
        // Check if available
        // If not - start searching in an expanding circle around the first point

        // Set up variables
        int depthHalf = 0;
        int widthHalf = 0;
        int halfOfDepth = 0;
        int halfOfWidth = 0;
        int startPositionDepth = 0;
        int startPositionWidth = 0;
        int endPositionDepth = 0;
        int endPositionWidth = 0;

        // Pick a starting point
        widthHalf = Random.Range(0, 1);
        depthHalf = Random.Range(0, 1);
        halfOfWidth = floor.width / 2 - 4;
        halfOfDepth = floor.depth / 2 - 4; // Shave 2 on each side to account for the double tile borders
        startPositionWidth = Random.Range(0, halfOfWidth) + widthHalf * halfOfWidth;
        startPositionDepth = Random.Range(0, halfOfDepth) + depthHalf * halfOfDepth;

        // check if the starting point is available
        bool searching = true;
        if (!AccessibleSpot(startPositionWidth, startPositionDepth)) // if not accessible
        {
            int searchRadius = 1;
            while (searching)
            {
                int numberOfPossiblePositions = 0; // TODO number of possible positions - CALCULATE
                int numberOfCheckedPositions = 0;
                // find new random point within searchRadius
                startPositionWidth = Random.Range(0, searchRadius);
                startPositionDepth = (int)Mathf.Sqrt(searchRadius * searchRadius - startPositionWidth * startPositionWidth);
                if (AccessibleSpot(startPositionWidth, startPositionDepth))
                {
                    searching = false;
                }
                numberOfCheckedPositions++;
                if (numberOfCheckedPositions >= numberOfPossiblePositions) // We have exhausted all possible positions - expand the serach
                {
                    searchRadius += 1;
                    break;
                }
            }
        }

        // TODO Repeat for endPoint

        // Remove old markers
        foreach (Transform obj in spawnPoints)
        {
            Destroy(obj.gameObject);
        }

        // Place markers
        Transform start = Instantiate(marker, new Vector3(startPositionWidth, 3, startPositionDepth), Quaternion.identity);
        start.parent = spawnPoints;
        start.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.yellow;
        Transform end = Instantiate(marker, new Vector3(endPositionWidth, 3, endPositionDepth), Quaternion.identity);
        end.parent = spawnPoints;
        end.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.green;

        // Create start and end nodes
        startNode = new Node(new Vector3(start.transform.position.x, 0, start.transform.position.z), null, end.position);
        endNode = new Node(new Vector3(end.transform.position.x, 0, end.transform.position.z), null, end.position);

        // LogDump spawn positions
        Debug.Log($"start: {start.transform.position}");
        Debug.Log($"end: {end.transform.position}");
    }

    private bool AccessibleSpot(int width, int depth)
    {
        // Count neighbours
        if (floor.finalGrid[width, depth] == 1) // fail - occupied position
        {
            return false;
        }
        if (floor.gridPOI[width, depth] == 1) // fail - place is POI
        {
            return false;
        }

        // count free nbrs
        int freeNbrs = 0;
        int[] pos = new int[4] { 0, 1, 0, -1 };
        for (int count = 0; count < 3; count += 1)
        {
            int posD = pos[count];
            int posW = pos[(count + 3) % 4];
            // check if nbr is in the border
            if ((depth + posD <= 2) || (depth + posD >= (floor.width - 4)))
            {
                break;
            }
            if ((width + posW <= 2) || (width + posW >= (floor.depth - 4)))
            {
                break;
            }
            // if nbr is occupied
            if (floor.finalGrid[width + posW, depth + posD] == 1)
            {
                break;
            }
            // if nbr is POI
            if (floor.gridPOI[width + posW, depth + posD] == 1)
            {
                break;
            }

            freeNbrs += 1;
        }

        if (freeNbrs >= 3)
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