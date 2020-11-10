using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrjMap : MonoBehaviour
{


    // prefabs
    public Transform pfBlock;
    public Transform pfFloor;
    public Transform player;
    public Camera main;

    public RectTransform background;
    public RectTransform rockIconPF;
    public RectTransform rockIconGreenPF;

    public float radius = 108f;

    // collections
    int[,] grid;
    Transform[,] terrain;
    List<POI> pingList;
    Dictionary<POI, RectTransform> iconMap;

    void Start()
    {
        grid = new int[,] {
            {1,2,1,3,1,1,1,1,1,2},
            {1,0,0,0,0,0,0,0,1,1},
            {1,1,1,0,0,0,0,1,3,1},
            {1,0,1,0,0,0,0,0,0,1},
            {1,1,1,2,0,0,0,0,0,2},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,2,0,0,0,0,3,0,1},
            {1,0,1,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,1,1,1},
            {1,0,03,0,0,0,0,1,1,1},
            {1,0,0,0,0,0,0,1,1,1},
            {1,2,1,1,1,1,1,2,1,1}
        };
        // grid = new int[,] {
        //     {1,2,1,3,1,1,1,1,2},
        //     {1,0,0,0,0,0,0,1,1},
        //     {1,1,1,0,0,0,1,1,1},
        //     {1,0,2,0,0,0,3,0,1},
        //     {1,0,1,0,0,0,0,0,1},
        //     {1,0,0,0,0,0,1,1,1},
        //     {1,2,1,1,1,1,2,1,1}
        // };
        iconMap = new Dictionary<POI, RectTransform>();
        terrain = new Transform[grid.GetLength(0), grid.GetLength(1)];
        pingList = new List<POI>();
        CreateMap();
        CenterPlayer();
        CenterMainCamera();
        CreateRockIcons();
    }

    void Update()
    {
        UpdateRockIcons();
    }

    private float GetAngle(Vector3 ping, Transform player)
    {
        // Find the marked object on te map
        Vector3 mapObject = ping;
        Vector3 mapObjectCompensated = mapObject + new Vector3(0.5f, 0, 0.5f);

        // get the position of the player
        Vector3 playerPosition = player.position;

        // get the direction to the object
        Vector3 directionToObject = (mapObjectCompensated - playerPosition);

        // get the angle between the player rotation and the direction to the object
        float angleToObject = -Vector3.SignedAngle(directionToObject, player.forward, Vector3.up);

        // Debug.Log($"mapObject: {mapObjectCompensated}");
        // Debug.Log($"playerPosition: {playerPosition}");
        // Debug.Log($".playerRotation: {playerRotation}");
        // Debug.Log($".dir: {directionToObject}");
        // Debug.Log($"angle: {angleToObject}");

        return angleToObject;
    }

    private void UpdateRockIcons()
    {
        foreach (POI ping in pingList)
        {
            RectTransform icon = iconMap.FirstOrDefault(i => i.Key == ping).Value;
            float angle = GetAngle(ping.position, player);
            Vector3 position = GetPosition(icon, angle, radius);
            icon.localPosition = position;
            iconMap[ping] = icon;
        }
    }


    private void CreateRockIcons()
    {
        int counter = 0;
        // Iterate pingmap
        foreach (POI ping in pingList)
        {
            // check type of ping
            float angle = GetAngle(ping.position, player);
            var newIcon = Instantiate(ping.prefab, new Vector3(0, 0, 0), Quaternion.identity, background);
            Vector3 position = GetPosition(background, angle, radius);
            newIcon.localPosition = position;
            iconMap.Add(ping, newIcon);
            Debug.Log(ping.position);
            // DBG
            counter ++;
        }
    }

    private Vector3 GetPosition(RectTransform parent, float angle, float radius)
    {
        float angleInRadians = angle * Mathf.Deg2Rad;
        float posX = Mathf.Sin(angleInRadians);
        float posY = Mathf.Cos(angleInRadians);
        Vector3 relativeDirection = new Vector3(posX, posY, 0);
        Vector3 relativePosition = new Vector3(posX, posY, 0) * radius;
        Vector3 parentPosition = parent.position;
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
        // Itreate grid
        for (int countC = 0; countC < grid.GetLength(0); countC++)
        {
            for (int countR = 0; countR < grid.GetLength(1); countR++)
            {
                Transform prefab;
                if (grid[countC, countR] > 0)
                {
                    prefab = pfBlock;
                }
                else
                {
                    prefab = pfFloor;
                }

                // Create object
                Transform newObj = Instantiate(prefab, new Vector3(countC, 0, countR), Quaternion.identity);

                if (grid[countC, countR] == 2)
                {
                    newObj.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.blue;
                    POI newPoi = new POI(new Vector3(countC, 0, countR), rockIconPF);
                    pingList.Add(newPoi);
                    newObj.GetChild(0).gameObject.layer = 8;
                }
                else if (grid[countC, countR] == 3)
                {
                    newObj.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.yellow;
                    POI newPoi = new POI(new Vector3(countC, 0, countR), rockIconGreenPF);
                    pingList.Add(newPoi);
                    newObj.GetChild(0).gameObject.layer = 8;
                }

                newObj.parent = transform;
                terrain[countC, countR] = newObj;
            }
        }
    }
}