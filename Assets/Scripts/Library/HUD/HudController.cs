using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Transform[] huds;

    [SerializeField] Transform raycaster;

    public List<POI> globalPingList;
    public List<Element>[] playerPingList; // OBS ?
    public Transform[] players;
    List<POI> playerOnePingList;
    List<POI> playerTwoPingList;
    private Hud hudOne;
    private Hud hudTwo;

    public void StartUp()
    {
        hudOne = huds[0].GetComponent<Hud>();
        hudTwo = huds[1].GetComponent<Hud>();

        SetUpPingLists();
        SetUpHuds();
        hudOne.StartHud();
        hudTwo.StartHud();

    }

    public void Init(List<POI> globalPingList)
    {
        playerOnePingList = new List<POI>();
        playerTwoPingList = new List<POI>();
        this.globalPingList = globalPingList;

        // Update();
    }

    // Update is called once per frame
    void Update()
    {
        // distribute pings to players
        SetUpPingLists();
    }

    public void SetUpPingLists()
    {
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
            visible = distanceToPlayerOne <= (int)ping.visibility + playerOneVisionRange;
            if (visible)
            {
                playerOnePingList.Add(ping);
            }

            // player Two
            float distanceToPlayerTwo = TB.GetDistance(playerTwoPosition, ping.position);
            visible = distanceToPlayerTwo <= (int)ping.visibility + playerTwoVisionRange;
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

    public void SetPlayers(Transform[] players)
    {
        this.players = players;
    }

    private void SetUpHuds()
    {
        if (players[0].transform.position.x < players[1].transform.position.x)
        {
            hudOne.SetChirality(0);
            hudOne.SetPlayer(players[0]);

            hudTwo.SetChirality(1);
            hudTwo.SetPlayer(players[1]);
        }
        else if (huds[0].transform.position.x < huds[1].transform.position.x)
        {
            hudOne.SetChirality(1);
            hudOne.SetPlayer(players[1]);

            hudTwo.SetChirality(0);
            hudTwo.SetPlayer(players[0]);
        }
        else
        {
            Debug.LogError("HubGroup : Error is players position");
        }

        hudOne.SetRadar(raycaster);
        hudTwo.SetRadar(raycaster);
    }
}


