﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public float radius = 108f;

    // collections
    int[,] grid;
    Transform[,] terrain;
    List<Vector3> pingMap;
    Dictionary<Vector3, RectTransform> iconMap;
    RectTransform testIcon;

    void Start()
    {
        grid = new int[,] {
            {1,2,1,1,1,1,1,1,1,1},
            {1,0,0,0,0,0,0,0,1,1},
            {1,1,1,0,0,0,0,1,1,1},
            {1,0,1,0,0,0,0,0,0,1},
            {1,1,1,1,0,0,0,0,0,2},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,2,0,0,0,0,0,0,1},
            {1,0,1,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,1,1,1},
            {1,2,1,1,1,1,1,1,1,1}
        };
        iconMap = new Dictionary<Vector3, RectTransform>();
        terrain = new Transform[grid.GetLength(0), grid.GetLength(1)];
        pingMap = new List<Vector3>();
        CreateMap();
        CenterPlayer(1.5f, 8.5f);
        CenterMainCamera();
        CreateRockIcons();
    }

    void Update()
    {
        float angle = GetAngle();
        // spawn an icon
        Vector3 newPosition = GetPosition(testIcon, angle, radius);
        testIcon.localPosition = newPosition;

    }

    private float GetAngle()
    {
        // Find the marked object on te map
        Vector3 mapObject = pingMap.FirstOrDefault();
        Vector3 mapObjectCompensated = mapObject + new Vector3(0.5f, 0, 0.5f);
        // get the position of the player
        Vector3 playerPosition = player.position;
        // get the direction to the object
        Vector3 directionToObject = (mapObjectCompensated - playerPosition);
        float playerRotationY = player.rotation.eulerAngles.y;
        Vector3 playerRotation = new Vector3(0, playerRotationY, 0);

        // get the angle between the player rotation and the direction to the object

        float angleToObject = -Vector3.SignedAngle(directionToObject, player.forward, Vector3.up);

        // Debug.Log($"mapObject: {mapObjectCompensated}");
        // Debug.Log($"playerPosition: {playerPosition}");
        // Debug.Log($".playerRotation: {playerRotation}");
        // Debug.Log($".dir: {directionToObject}");
        // Debug.Log($"angle: {angleToObject}");

        return angleToObject;
    }

    private void CreateRockIcons()
    {

        
        float angle = GetAngle();
        // create the object
        var newIcon = Instantiate(rockIconPF, new Vector3(0, 0, 0), Quaternion.identity, background);


        Vector3 position = GetPosition(newIcon, angle, radius);
        newIcon.localPosition = position;
        iconMap.Add(position, newIcon);
        testIcon = newIcon;

    }

    private Vector3 GetPosition(RectTransform icon, float angle, float radius)
    {
        float angleInRadians = angle * Mathf.Deg2Rad;
        float posX = Mathf.Sin(angleInRadians);
        float posY = Mathf.Cos(angleInRadians);
        Vector3 relativeDirection = new Vector3(posX, posY, 0);
        Vector3 relativePosition = new Vector3(posX, posY, 0) * radius;
        Vector3 parentPosition = icon.parent.position;
        Vector3 iconPosition = relativePosition + parentPosition;

        // Debug.Log($"angle: {angle}");
        // Debug.Log($"angle rad: {angleInRadians}");
        // Debug.Log($"posX: {posX}");
        // Debug.Log($"posY: {posY}");
        // Debug.Log($"rel direction: {relativeDirection}");
        // Debug.Log($"rel dir X: {relativeDirection.x}");
        // Debug.Log($"rel dir Y: {relativeDirection.y}");
        // Debug.Log($"rel dir Z: {relativeDirection.z}");

        // Debug.Log($"rel position: {relativePosition}");
        // Debug.Log($"parent: {parentPosition}");
        // Debug.Log($"position: {iconPosition}");

        // return position;
        return relativePosition;
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
    private void CenterPlayer(float posX, float posZ)
    {
        Vector3 mapCenter = new Vector3(posX, 0, posZ);
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
                    pingMap.Add(new Vector3(countC, 0, countR));
                }

                newObj.parent = transform;
                terrain[countR, countC] = newObj;
            }
        }
    }
}
