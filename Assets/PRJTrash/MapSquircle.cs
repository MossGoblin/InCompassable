using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapSquircle : MonoBehaviour
{


    // prefabs
    public Transform pfBlock;
    public Transform pfFloor;
    public Transform player;
    public Camera main;


    public Transform raycaster;
    public RectTransform background;
    public RectTransform rockIconPF;
    public RectTransform rockIconGreenPF;

    [Range(-50f, 50f)]
    public float buffer = -25f;
    [SerializeField]
    private float mapRadius;

    // collections
    int[,] grid;
    Transform[,] terrain;
    List<POI> pingList;
    Dictionary<POI, RectTransform> iconMap;

    void Start()
    {
        mapRadius = background.sizeDelta.x / 2;

        // grid = new int[,] {
        //     {1,1,1,1,1,1,1,1,1,1},
        //     {1,0,0,0,0,0,0,0,0,1},
        //     {1,0,0,0,0,0,0,0,0,1},
        //     {1,0,0,0,0,0,0,0,0,1},
        //     {1,0,0,0,0,0,0,0,0,1},
        //     {1,0,0,0,0,0,0,0,0,1},
        //     {1,0,0,0,0,0,0,3,0,1},
        //     {1,0,0,0,0,0,0,0,0,1},
        //     {1,0,0,0,0,0,0,0,0,1},
        //     {1,0,0,0,0,0,0,0,0,1},
        //     {1,0,0,0,0,0,0,0,0,1},
        //     {1,1,1,1,1,1,1,1,1,1}
        // };

        grid = new int[,] {
            {1,2,1,1,1,1,1,1,1,2},
            {1,0,0,0,0,0,0,0,1,1},
            {1,1,1,0,0,0,0,1,0,1},
            {1,0,1,0,0,0,0,0,0,1},
            {1,3,1,1,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,1,0,0,0,0,3,0,1},
            {1,0,1,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,1,1,1},
            {1,0,3,0,0,0,0,1,1,1},
            {1,0,0,0,0,0,0,1,1,1},
            {1,1,1,1,1,1,1,2,1,1}
        };

        iconMap = new Dictionary<POI, RectTransform>();
        terrain = new Transform[grid.GetLength(0), grid.GetLength(1)];
        pingList = new List<POI>();
        CreateMap();
        CenterPlayer();
        CenterMainCamera();
        // SetInitialDamper();
        CreateRockIcons();
    }

    private void SetInitialDamper()
    {
        throw new NotImplementedException();
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

        return angleToObject;
    }

    private void CreateRockIcons()
    {
        // Iterate pingmap
        foreach (POI ping in pingList)
        {
            var newIcon = Instantiate(ping.icon, new Vector3(), Quaternion.identity, background);
            iconMap.Add(ping, newIcon);
        }
        UpdateRockIcons();
    }

    private void UpdateRockIcons()
    {
        foreach (POI ping in pingList)
        {
            // 1. from area - get angle between player.front and ping
            // 2. send angle to radar - raycast on the same angle, compared to radar.front
            // 3. from radar - get distance from the raycast
            // 4. send angle and distance to minimap
            // 5. minimap - position icon on the same angle, compared to minimap.front; use distance from radar

            // 1. 
            Vector3 playerLookingVector = player.transform.forward;

            Debug.DrawRay(player.position, playerLookingVector * 10, Color.green, 0.05f);

            Vector3 positionOfPing = new Vector3(ping.position.x, 0, ping.position.z);
            Vector3 directionToPing = (positionOfPing - player.position).normalized;

            // Debug.DrawRay(player.position, Vector3.one, Color.yellow, 0.05f);
            Debug.DrawRay(player.position, directionToPing * 10, Color.yellow, 0.05f);

            float angleToPing = Vector3.SignedAngle(playerLookingVector, directionToPing, player.transform.up);
            Debug.Log($"angle: {angleToPing}");
            // 2. 3.
            // float distanceToIcon = raycaster.GetComponent<Radar>().GetDistanceAtAngle(directionToPing.normalized);
            float distanceToIcon = raycaster.GetComponent<Radar>().GetDistanceAtAngle(angleToPing);

            // 4. 5.
            float dist = mapRadius * distanceToIcon + buffer;
            Vector3 positionOfIcon = GetPosition(background, angleToPing, dist);

            RectTransform icon = iconMap.FirstOrDefault(i => i.Key == ping).Value;
            icon.localPosition = positionOfIcon;
            iconMap[ping] = icon;
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
                    POI newPoi = new POI(new Vector3(countC, 0, countR), rockIconPF, ElementsLibrary.VisibilityRanges.Always); // visibility not used in this context
                    pingList.Add(newPoi);
                    newObj.GetChild(0).gameObject.layer = 8;
                }
                else if (grid[countC, countR] == 3)
                {
                    newObj.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.yellow;
                    POI newPoi = new POI(new Vector3(countC, 0, countR), rockIconGreenPF, ElementsLibrary.VisibilityRanges.Always); // visibility not used in this context
                    pingList.Add(newPoi);
                    newObj.GetChild(0).gameObject.layer = 8;
                }

                newObj.parent = transform;
                terrain[countC, countR] = newObj;
            }
        }
    }
}