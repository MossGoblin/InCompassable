using System.Collections.Generic;
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

    public void FindPath()
    {
        Debug.Log("On The Path...");
        Debug.Log($"{floor.finalGrid.Length}");

        // Init lists
        openList = new List<Node>();
        closedList = new List<Node>();
        // Get the start node in the open list
        openList.Add(startNode);
        bool searching = true;
        while (searching)
        {
            // Get next node from the openList
            Node currentNode = openList.OrderBy(n => n.fScore).FirstOrDefault(); // the node with the smallest fScore
            // Move node from open to closed list
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            //Debug.Log("** close");
            // Check if the current node is the end node
            if (currentNode.position == endNode.position)
            {
                // FOUND THE PATH
                Debug.Log("FOUND PATH");
                searching = false;
            }
            // Iterate nbrs
            // For each nbs - if there is an open node at that position - work with it; otherwise - create a new one
            // Get nbrs
            List<Vector3> nbrPositions = floor.GetNbrs(currentNode);
            if (nbrPositions.Count > 0)
            {
                foreach (Vector3 nbrPos in nbrPositions)
                {
                    // Check if nbr is closed - if it is - skip
                    Node closedNode = closedList.FirstOrDefault(n => n.position == nbrPos);
                    if (closedNode != null)
                    {
                        continue;
                    }
                    // Find if there is an open node at this position
                    Node openNode = openList.FirstOrDefault(n => n.position == nbrPos);
                    if (openNode != null) // There is an open node at this position - update it
                    {
                        // TODO some check needs to happen here !!
                        if (currentNode.gScore + 1 < openNode.gScore)
                        {
                            openNode.parent = currentNode;
                            //Debug.Log("--- updated");
                        }
                    }
                    else // No node at this position so far
                    {
                        Node newNode = new Node(nbrPos, currentNode, endNode.position);
                        openList.Add(newNode);
                        //Debug.Log("-- open");
                    }
                }
            }
            if (openList.Count == 0)
            {
                searching = false;
            }
            Debug.Log($"o: {openList.Count} / c: {closedList.Count} / a: {floor.finalGrid.Length}");
        }
        // search ended; return something
        return;
    }

    public void CreateSpawnPoints()
    {
        int depthHalf = 0;
        int widthHalf = 0;
        int halfDepth = 0;
        int halfWidth = 0;
        int startPositionDepth = 0;
        int startPositionWidth = 0;
        int endPositionDepth = 0;
        int endPositionWidth = 0;

        // Pick start point
        // Find a point with at least 3 empty nbrs
        bool searching = true;
        while (searching)
        {
            // pick random half of the map (width and depth) for a starting point
            depthHalf = Random.Range(0, 1);
            widthHalf = Random.Range(0, 1);
            halfDepth = floor.depth / 2 - 4; // Shave 2 on each side to account for the double tile borders
            halfWidth = floor.width / 2 - 4;
            startPositionDepth = Random.Range(0, halfDepth) + depthHalf * halfDepth;
            startPositionWidth = Random.Range(0, halfWidth) + widthHalf * halfWidth;

            // Check if the position is viable
            if (AccessibleSpot(startPositionDepth, startPositionWidth))
            {
                searching = false;
            }
        }

        // Pick end point
        // Endpoint - depth range - no limit (half width to full depth); width range - half depth up to 2/3 width
        searching = true;
        while (searching)
        {
            int endPositionDepthRaw = (startPositionDepth + Random.Range(halfDepth, 2 * halfDepth));
            endPositionDepth = endPositionDepthRaw % (floor.depth - 4);
            int endPositionWidthRaw = (startPositionWidth + Random.Range(halfWidth, halfWidth * 4 / 3));
            endPositionWidth = endPositionWidthRaw % (floor.width - 4);
            // Check if the position is viable
            if (AccessibleSpot(endPositionDepth, endPositionWidth))
            {
                searching = false;
            }
        }

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
        startNode = new Node(new Vector3(start.transform.position.x, 0, start.transform.position.z), null, start.position);
        endNode = new Node(new Vector3(end.transform.position.x, 0, end.transform.position.z), null, end.position);
    }

    private bool AccessibleSpot(int depth, int width)
    {
        // Count neighbours
        if (floor.finalGrid[depth, width] == 1) // fail - occupied position
        {
            return false;
        }
        if (floor.gridPOI[depth, width] == 1) // fail - place is POI
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
            if ((depth + posD <= 2) || (depth + posD >= (floor.depth - 4)))
            {
                break;
            }
            if ((width + posW <= 2) || (width + posW >= (floor.width - 4)))
            {
                break;
            }
            // if nbr is occupied
            if (floor.finalGrid[depth + posD, width + posW] == 1)
            {
                break;
            }
            // if nbr is POI
            if (floor.gridPOI[depth + posD, width + posW] == 1)
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
    public Node parent
    {
        get
        {
            return parent;
        }
        set
        {
            this.parent = parent;
            gScore = parent.gScore + 1; ;
        }
    }
    public float gScore { get; set; }
    public float hScore { get; set; }
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
        hScore = GetScore(this.position, endNodePosition);
        if (parent == null)
        {
            gScore = 0;
        }
        else
        {
            gScore = parent.gScore + 1;
        }
    }

    public float GetScore(Vector3 pos1, Vector3 pos2)
    {
        Vector3 dist = pos1 - pos2;
        float score = Mathf.Sqrt(dist.x * dist.x + dist.z + dist.z);
        return score;
    }
}