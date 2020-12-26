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

    // Camera ref
    [SerializeField]
    public Camera hudCamera;

    // Player ref
    private Transform player;

    // Radar ref
    private Transform raycaster;

    // Self refs
    [SerializeField]
    private RectTransform background;

    // Holder for hidden icons
    [SerializeField]
    private Transform hiddenHolder;

    [SerializeField]
    private float mapRadius;

    [SerializeField]
    [Range(0f, 500f)]
    private float multiplier = 160;

    [SerializeField]
    [Range(-500f, 250f)]
    private float buffer = -40f;

    // current icons
    Dictionary<POI, RectTransform> iconMap;

    [SerializeField]
    private int chirality;

    // Debug range slide
    // FIX one of those will be used
    [SerializeField]
    [Range(2f, 10f)]
    private float hudRange = 5f;

    [SerializeField]
    [Range(0f, 1f)]
    private float hudBuffer = 0f;

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
        UpdateIcons();

        // disable for DBG
        if (player.GetComponent<Player>().chirality == 1)
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        pingNumber = this.pingList.Count();
        UpdateIcons();
        HideIconsForVisibleObjects(); // FIX not working
    }

    private void UpdateIcons()
    {
        ClearIconMap();
        foreach (POI ping in pingList)
        {
            // 1. from area - get angle between player.front and ping
            // 2. send angle to radar - raycast on the same angle, compared to radar.front
            // 3. from radar - get distance from the raycast
            // 4. send angle and distance to minimap
            // 5. minimap - position icon on the same angle, compared to minimap.front; use distance from radar
            if (iconMap.ContainsKey(ping))
            {
                continue;
            }

            var icon = Instantiate(ping.icon, new Vector3(), Quaternion.identity, background);

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
            float distanceToIcon = raycaster.GetComponent<Radar>().GetDistanceAtAngle(angleToPing);

            // 4. 5.
            float dist = multiplier * distanceToIcon + buffer;
            // float dist = mapRadius * distanceToIcon + buffer;
            Vector3 positionOfIcon = GetPosition(background, angleToPing, dist);

            icon.localPosition = positionOfIcon;
            iconMap.Add(ping, icon);

            // HERE Hide icons for close objects
            // OBS
            // check if the distance from the center of the hudded area to the object is too small
            // .. if it is, then the object is visible in the hud - hide the icon
        }
    }

    // HERE
    private void HideIconsForVisibleObjects()
    {
        foreach (POI ping in iconMap.Keys)
        {
            Vector3 screenPoint = hudCamera.WorldToViewportPoint(ping.position);
            bool onScreen = screenPoint.z > hudBuffer && screenPoint.z < 1 - hudBuffer && screenPoint.x > hudBuffer && screenPoint.x < 1 - hudBuffer;
            if (onScreen)
            {
                iconMap[ping].SetParent(hiddenHolder);
            }
            else
            {
                iconMap[ping].SetParent(background);
            }
        }
    }

    public Player GetPlayer()
    {
        return player.GetComponent<Player>();
    }

    private void ClearIconMap()
    {
        foreach (RectTransform icon in iconMap.Values)
        {
            Destroy(icon.gameObject);
        }
        iconMap = new Dictionary<POI, RectTransform>();
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

    public void AddPings(List<POI> newPings)
    {
        foreach (POI ping in newPings)
        {
            pingList.Add(ping);
        }
    }

    public void RemovePings(List<POI> oldPings)
    {
        foreach (POI ping in oldPings)
        {
            pingList.Remove(ping);
        }
    }

    public void SetUpCamera(Camera camera)
    {
        this.hudCamera = camera;
    }
}
