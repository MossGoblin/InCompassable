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

    // DBG ONLY
    private bool haveSpawns = false;

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
        if (haveSpawns)
        {
            FindPath();
        }
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

        List<Vector3> undesirables = new List<Vector3>();

        int halfWidth = floor.width / 2 - 2;
        int halfDepth = floor.depth / 2 - 2; // Shave 2 on each side to account for the double tile borders

        // Pick start point
        // Find a point with at least 3 empty nbrs

        // Create a dictionary of all available points
        Debug.Log("List viable empties");

        Dictionary<Vector3, bool> availables = new Dictionary<Vector3, bool>();
        for (int countW = 0; countW < floor.finalGrid.GetLength(0); countW++)
        {
            for (int countD = 0; countD < floor.finalGrid.GetLength(1); countD++)
            {
                if (AccessibleSpot(countW, countD))
                {
                    availables.Add(new Vector3(countW, 3, countD), true);
                }
            }
        }

        // pick starting point within the first half of the map
        bool searching = true;
        Vector3 startVector = Vector3.zero;
        Vector3 endVector = Vector3.zero;

        // DBG Safety
        int counter = 0;

        while (searching)
        {
            float randomW = Random.Range(0, halfWidth / 2);
            startVector = availables.FirstOrDefault(kvp => (kvp.Key.x >= (randomW)) && (kvp.Key.x <= (halfWidth - randomW))).Key;
            availables[startVector] = false;
            // Find a Vector at approximately that distance for the end point

            endVector = availables.FirstOrDefault(kvp => (Mathf.Abs(Math.Abs(kvp.Key.x - startVector.x) - floor.spawnsWidthDist) <= floor.spawnDistanceThreshold) &&
                                                                    Mathf.Abs(Math.Abs(kvp.Key.z - startVector.z) - floor.spawnsDepthDist) <= floor.spawnDistanceThreshold).Key;
            if (endVector.x != 0 && endVector.z != 0)
            {
                availables[endVector] = false;
                searching = false;
                haveSpawns = true;
            }

            if (counter >= 30)
            {
                Debug.Log("Cycled OUT");
                searching = false;
            }
            counter++;
        }

        // Remove old markers
        foreach (Transform obj in spawnPoints)
        {
            Destroy(obj.gameObject);
        }

        // Place markers
        Transform start = Instantiate(marker, startVector, Quaternion.identity);
        start.parent = spawnPoints;
        start.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.yellow;
        Transform end = Instantiate(marker, endVector, Quaternion.identity);
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

        return score;
    }
}