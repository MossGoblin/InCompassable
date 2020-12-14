using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Transform[] huds;
    public List<POI> globalPingList;
    public List<Element>[] playerPingList; // OBS ?
    public Transform[] players;
    List<POI> playerOnePingList;
    List<POI> playerTwoPingList;
    void Start()
    {
        // INIT
        Init();
    }

    private void Init()
    {
        playerOnePingList = new List<POI>();
        playerTwoPingList = new List<POI>();
    }

    // Update is called once per frame
    void Update()
    {
        // distribute pings to players
        UpdatePingLists();
        DistributePingLists();
    }

    private void UpdatePingLists()
    {
        // Update ping lists
        Vector3 playerOnePosition = players[0].position;
        Vector3 playerTwoPosition = players[1].position;
        float playerOneVisionRange = players[0].GetComponent<Player>().visionRange;
        float playerTwoVisionRange = players[1].GetComponent<Player>().visionRange;

        foreach (POI ping in globalPingList)
        {
            bool visible;
            // player One
            float distanceToPlayerOne = TB.GetDistance(playerOnePosition, ping.position);
            visible = (distanceToPlayerOne + (int)ping.visibility) <= playerOneVisionRange;
            if (visible)
            {
                playerOnePingList.Add(ping);
            }

            // player Two
            float distanceToPlayerTwo = TB.GetDistance(playerTwoPosition, ping.position);
            visible = (distanceToPlayerTwo + (int)ping.visibility) <= playerTwoVisionRange;
            if (visible)
            {
                playerTwoPingList.Add(ping);
            }
        }
    }

    private void DistributePingLists()
    {
        huds[0].GetComponent<Hud>().UpdatePingList(playerOnePingList);
        huds[1].GetComponent<Hud>().UpdatePingList(playerTwoPingList);
    }

    public void SetPingList(List<POI> globalPingList)
    {
        this.globalPingList = globalPingList;
    }

    public void SetPlayers(Transform[] players)
    {
        this.players = players;
        DistributeChirality();
    }

    private void DistributeChirality()
    {
        if (players[0].transform.position.x < players[1].transform.position.x)
        {
            huds[0].GetComponent<Hud>().SetChirality(0);
            huds[1].GetComponent<Hud>().SetChirality(1);
        }
        else if (huds[0].transform.position.x < huds[1].transform.position.x)
        {
            huds[0].GetComponent<Hud>().SetChirality(1);
            huds[1].GetComponent<Hud>().SetChirality(0);
        }
        else
        {
            Debug.LogError("HubGroup : Can not distribute parity");
        }

    }

    // WIP
    // public List<POI> GetPingList(int chirality)
    // {
    //     // iterate global ping list
    //     // for each - check if visible by player
    //     foreach (Element poi in globalPOIList)
    //     {
    //         // checks:
    //         // 1. distance to player + object visibility >= player vision range
    //         // 2. object always visible ??
    //         float distanceToPlayer = GetDistanceToPlayer(poi, playerPingList[chirality]);
    //         float poiVisibility = poi.visibility;
    //         float playerVisionRange = players[chirality].GetComponent<Player>().visionRange;
    //         if ()
    //     }

    // }

    // private float GetDistanceToPlayer(Element, Transform player)
    // {
    //     throw new NotImplementedException();
    // }

    // internal void SetPlayers(Transform[] players)
    // {
    //     this.players = players;
    // }
}


