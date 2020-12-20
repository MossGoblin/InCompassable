using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hud : MonoBehaviour
{
    // List of pings to be projected
    private List<POI> pingList;
    //  0 for left, 1 for right

    [SerializeField]
    private int pingNumber;

    // Player ref
    private Transform player;

    // Radar ref
    private Transform raycaster;

    // Self refs
    [SerializeField]
    private RectTransform background;

    private float mapRadius;

    [Range(-500f, 250f)]
    public float buffer = 0f;

    // current icons
    Dictionary<POI, RectTransform> iconMap;



    [SerializeField]
    private int chirality;

    public void SetChirality(int chirality)
    {
        this.chirality = chirality;
    }

    public void SetPlayer(Transform player)
    {
        this.player = player;
    }

    public void SetRadar(Transform raycaster)
    {
        this.raycaster = raycaster;
        mapRadius = background.sizeDelta.x / 2;

    }

    public void StartHud()
    {
        iconMap = new Dictionary<POI, RectTransform>();
        CreateIcons();
    }

    void Update()
    {
        UpdateIcons();
        pingNumber = this.pingList.Count();
    }


    private void CreateIcons()
    {
        // Iterate pingmap
        foreach (POI ping in pingList)
        {
            var newIcon = Instantiate(ping.icon, new Vector3(), Quaternion.identity, background);
            iconMap.Add(ping, newIcon);
        }
        UpdateIcons();
    }

    private void UpdateIcons()
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
            // Debug.Log($"angle: {angleToPing}");
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

    public void UpdatePingList(List<POI> pingList)
    {
        this.pingList = new List<POI>();
        this.pingList = pingList;
    }
}
