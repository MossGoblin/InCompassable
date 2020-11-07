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

    public RectTransform background;
    public RectTransform rockIconPF;

    // parameters
    [Range(0f, 180f)]
    public float angle = 165f;
    [Range(0f, 200f)]
    public float radius = 120f;

    // collections
    int[,] grid;
    Transform[,] terrain;
    int[,] pingMap;
    Dictionary<Vector3, RectTransform> iconMap;
    RectTransform testIcon;

    void Start()
    {
        grid = new int[,] {
            {1,1,1,1,1,1,1,1,1,1},
            {1,0,0,0,0,0,0,0,1,1},
            {1,1,1,0,0,0,0,1,1,1},
            {1,0,1,0,0,0,0,0,0,1},
            {1,1,1,1,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,1,0,0,0,0,0,0,1},
            {1,0,1,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,1,1,1},
            {1,2,1,1,1,1,1,1,1,1}
        };
        iconMap = new Dictionary<Vector3, RectTransform>();
        terrain = new Transform[grid.GetLength(0), grid.GetLength(1)];
        pingMap = new int[grid.GetLength(0), grid.GetLength(1)];
        CreateMap();
        CenterPlayer();
        CenterMainCamera();
        CreateRockIcon();
    }

    void Update()
    {
        Vector3 newPosition = GetPosition(testIcon, angle, radius);
        testIcon.position = newPosition;
    }
    private void CreateRockIcon()
    {
        // create the object
        var newIcon = Instantiate(rockIconPF, new Vector3(0, 0, 0), Quaternion.identity, background);
        Vector3 localPosition = GetPosition(newIcon, angle, radius);
        newIcon.position = localPosition;
        iconMap.Add(localPosition, newIcon);
        testIcon = newIcon;
    }

    private Vector3 GetPosition(RectTransform icon, float angle, float radius)
    {
        Vector3 position;
        float posX = Mathf.Cos(angle);
        float posY = Mathf.Sin(angle);
        Vector3 relativePosition = new Vector3(posX, posY, 0) * radius;
        Vector3 parentPosition = icon.parent.position;
        position = relativePosition + parentPosition;
        // Debug.Log($"angle: {angle}");
        // Debug.Log($"posX: {posX}");
        // Debug.Log($"posY: {posY}");
        Debug.Log($"parent: {parentPosition}");
        Debug.Log($"rel position: {relativePosition}");
        Debug.Log($"position: {position}");
        return position;
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
}
