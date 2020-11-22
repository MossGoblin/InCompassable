using UnityEngine;
using ToolBox;
using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class MapMaker : MonoBehaviour
{
    [Header("Main")]
    private int[,] grid;
    [SerializeField] private int width;
    [SerializeField] private int depth;
    [SerializeField] private int centroidCount;
    [SerializeField] private Transform tileHolder;
    [SerializeField] private Transform tile;

    [SerializeField]
    private Color[] colors;

    [Header("Others")]
    [SerializeField] Camera mainCamera;
    private List<Vector2> centroidList;
    private Transform[,] tiles;

    // Start is called before the first frame update
    void Start()
    {
        centroidList = new List<Vector2>();

        InitGrid();
        PositionCamera();
        GenerateRegions();
    }

    private void PositionCamera()
    {
        Vector3 newPosition = new Vector3((int)(width / 2), mainCamera.transform.position.y, (int)(depth / 2));
        mainCamera.transform.position = newPosition;
    }

    private void GenerateRegions()
    {
        SelectCentroids();
        SplitGrid();
        Colorize();
    }

    private void Colorize()
    {
        tiles = new Transform[width, depth];
        colors = new Color[centroidCount];
        for (int count = 0; count < centroidCount; count++)
        {
            colors[count] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }

        foreach ((int countW, int countD) in Iterator.Iteration(width, depth))
        {
            Vector3 position = new Vector3(countW, 0, countD);

            Transform obj = Instantiate(tile, position, Quaternion.identity, tileHolder);
            obj.GetChild(0).GetComponent<MeshRenderer>().material.color = colors[grid[countW, countD]];
            tiles[countW, countD] = obj;
        }
    }

    private void SplitGrid()
    {
        foreach ((int countW, int countD) in Iterator.Iteration(width, depth))
        {
            float minDistance = float.MaxValue;
            int count = 0;
            foreach (Vector2 centroid in centroidList)
            {
                if (Vector3.Distance(new Vector2(countW, countD), centroid) < minDistance)
                {
                    minDistance = Vector3.Distance(new Vector2(countW, countD), centroid);
                    grid[countW, countD] = count;
                }
                count++;
            }
        }
    }

    private void SelectCentroids()
    {
        for (int count = 0; count < centroidCount; count++)
        {
            int posW = Random.Range(0, width - 1);
            int posD = Random.Range(0, depth - 1);
            centroidList.Add(new Vector2(posW, posD));
        }
    }

    private void InitGrid()
    {
        grid = Iterator.GenerateGridWithFill(width, depth, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
