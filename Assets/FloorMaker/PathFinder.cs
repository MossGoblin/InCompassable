using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathFinder : MonoBehaviour
{
    // Refs
    public Transform marker;
    public Floor floor;

    // Init lists
    public List<Node> openNodes { get; set; }
    public List<Node> closedNodes { get; set; }



    // Start is called before the first frame update
    void Start()
    {
        openNodes = new List<Node>();
        closedNodes = new List<Node>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void FindPath()
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
            if (SpotIsGood(startPositionDepth, startPositionWidth))
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
            int endPositionWidthRaw = (startPositionWidth + Random.Range(halfDepth, halfWidth * 4 / 3));
            endPositionWidth = endPositionWidthRaw % (floor.width - 4);
            // Check if the position is viable
            if (SpotIsGood(startPositionDepth, startPositionWidth))
            {
                searching = false;
            }
        }

        // Place markers
        Transform start = Instantiate(marker, new Vector3(startPositionWidth, 3, startPositionDepth), Quaternion.identity);
        start.GetComponent<MeshRenderer>().material.color = Color.yellow;
        Transform end = Instantiate(marker, new Vector3(endPositionWidth, 3, endPositionDepth), Quaternion.identity);
        end.GetComponent<MeshRenderer>().material.color = Color.green;
    }

    private bool SpotIsGood(int depth, int width)
    {
        // Count neighbours
        if (floor.finalGrid[depth, width] == 1) // fail - occupied position
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
    public Node parent { get; set; }
    public float gScore { get; set; }
    public float hScore { get; set; }
    public float fScore { get; set; }

    public Node(Vector3 pos)
    {
        this.position = pos;
    }
}