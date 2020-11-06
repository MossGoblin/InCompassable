using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{

    // prefabs
    public Transform pfBlock;
    public Transform pfFloor;
    public Transform player;
    public Camera main;

    // collections
    int[,] grid;
    Transform[,] terrain;
    int[,] pingMap;

    void Start()
    {
        grid = new int[,] {
            {1,1,1,1,1,1,1,2,2,1},
            {1,0,0,0,0,0,0,0,1,1},
            {1,1,1,0,0,0,0,1,1,1},
            {1,0,1,0,0,0,0,0,0,1},
            {1,2,1,1,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,1,0,0,0,0,0,0,1},
            {1,0,1,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,1,1,1},
            {1,2,1,1,1,1,1,1,2,1}
        };
        terrain = new Transform[grid.GetLength(0), grid.GetLength(1)];
        pingMap = new int[grid.GetLength(0), grid.GetLength(1)];
        CreateMap();
        CenterPlayer();
        CenterMainCamera();
    }

    private void CenterMainCamera()
    {
        Vector3 mapCenter = new Vector3(grid.GetLength(0) / 2, 13, grid.GetLength(1) / 2);
        main.transform.position = mapCenter;

    }

    private void CenterPlayer()
    {
        Vector3 mapCenter = new Vector3(grid.GetLength(0) / 2, 0, grid.GetLength(1) / 2);
        player.position = mapCenter;
    }

    private void CreateMap()
    {
        for (int countR = 0; countR < grid.GetLength(0); countR++)
        {
            for (int countC = 0; countC < grid.GetLength(1); countC++)
            {
                Transform prefab;
                if (grid[countR, countC] == 1 || grid[countR, countC] == 2)
                {
                    prefab = pfBlock;
                }
                else
                {
                    prefab = pfFloor;
                }

                Transform newObj = Instantiate(prefab, new Vector3(countC, 0, countR), Quaternion.identity);
                if (grid[countR, countC] == 2)
                {
                    newObj.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.blue;
                    newObj.GetChild(0).gameObject.layer = 8;
                    pingMap[countR, countC] = 1;
                }

                newObj.parent = transform;
                terrain[countR, countC] = newObj;
            }
        }
    }

    void Update()
    {
    }
}
