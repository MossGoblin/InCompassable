using System;
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

    // parameters
    // [Range(0f, 180f)]
    // public float angle = 165f;
    public float radius = 115f;

    // collections
    int[,] grid;
    Transform[,] terrain;
    List<Vector3> pingMap;
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
        pingMap = new List<Vector3>();
        CreateMap();
        CenterPlayer();
        CenterMainCamera();
        CreateRockIcon();
    }

    void Update()
    {
        float angle = GetAngle();
        // spawn an icon
        Vector3 newPosition = GetPosition(testIcon, angle, radius);
        testIcon.position = newPosition;

    }

    private float GetAngle()
    {
        // Find the marked object on te map
        Vector3 mapObject = pingMap.FirstOrDefault();
        Vector3 mapObjectCompensated = mapObject + new Vector3(0.5f, 0, 0.5f);
        // get the position of the player
        Vector3 playerPosition = player.position;
        // get the direction to the object
        Vector3 directionToObject = (mapObjectCompensated - playerPosition).normalized;
        
        // clean positions of Y to get clean angle
        Vector3 direction = new Vector3(directionToObject.x, 0, directionToObject.z);
        Vector3 playerPoint = new Vector3(player.forward.x, 0, player.forward.z);
        
        // get the angle between the player rotation and the direction to the object
        float angleToObject = Vector3.Angle(playerPoint, direction);
        // float angleToObjectCallibrated = (angleToObject + 270) % 180;

        Debug.Log($"mapObject: {mapObject}");
        Debug.Log($"playerPosition: {playerPosition}");
        Debug.Log($"playerRotation: {player.forward}");
        Debug.Log($"dir: {directionToObject}");
        Debug.Log($"angle: {angleToObject}");
        // Debug.Log($"angle cal: {angleToObjectCallibrated}");

        return angleToObject;
    }

    private void CreateRockIcon()
    {
        float angle = GetAngle();
        // create the object
        var newIcon = Instantiate(rockIconPF, new Vector3(0, 0, 0), Quaternion.identity, background);
        Vector3 position = GetPosition(newIcon, angle, radius);
        newIcon.position = position;
        iconMap.Add(position, newIcon);
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
        // Debug.Log($"parent: {parentPosition}");
        // Debug.Log($"rel position: {relativePosition}");
        // Debug.Log($"position: {position}");
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
                    pingMap.Add(new Vector3(countC, 0, countR));
                }

                newObj.parent = transform;
                terrain[countR, countC] = newObj;
            }
        }
    }
}
