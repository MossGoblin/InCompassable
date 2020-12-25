using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private List<POI> playerOnePingList;
    private List<POI> playerTwoPingList;
    private List<POI> playerOnePingListOLD;
    private List<POI> playerTwoPingListOLD;
    private Hud hudOne;
    private Hud hudTwo;

    private Player playerOne;
    private Player playerTwo;

    public void StartUp()
    {
        hudOne = huds[0].GetComponent<Hud>();
        hudTwo = huds[1].GetComponent<Hud>();

        SetUpHuds();
        SetUpPingLists();
        hudOne.StartHud();
        hudTwo.StartHud();

    }

    public void Init(List<POI> globalPingList)
    {
        playerOnePingList = new List<POI>();
        playerTwoPingList = new List<POI>();
        playerOnePingListOLD = new List<POI>();
        playerTwoPingListOLD = new List<POI>();
        this.globalPingList = globalPingList;
        // Update();
    }

    void Update()
    {
        // distribute pings to players
        UpdatePingList();
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

        playerOne = hudOne.GetPlayer();
        playerTwo = hudTwo.GetPlayer();
    }

    public void SetUpPingLists()
    {
        CreatePingLists();
        DistributePingLists();
    }

    private void CreatePingLists()
    {
        // clear pingLists
        playerOnePingList = new List<POI>();
        playerTwoPingList = new List<POI>();

        // Update ping lists
        Vector3 playerOnePosition = players[0].position;
        Vector3 playerTwoPosition = players[1].position;
        float playerOneVisionRange = players[0].GetComponent<Player>().visionRange;
        float playerTwoVisionRange = players[1].GetComponent<Player>().visionRange;

        foreach (POI ping in globalPingList)
        {
            playerOnePingList = PingToPlayerList(ping, playerOnePosition, playerOneVisionRange, playerOnePingList);
            playerTwoPingList = PingToPlayerList(ping, playerTwoPosition, playerTwoVisionRange, playerTwoPingList);
        }
    }

    private void UpdatePingList()
    {
        // Stash current ping lists
        playerOnePingListOLD = playerOnePingList;
        playerTwoPingListOLD = playerTwoPingList;
        // Create new lists
        CreatePingLists();
        // Get all new pings for each list
        List<POI> newInPlayerONEList = playerOnePingList.Except(playerOnePingListOLD).ToList();
        List<POI> newInPlayerTWOList = playerTwoPingListOLD.Except(playerTwoPingListOLD).ToList();
        // Get all old from each ping lists
        List<POI> oldInPlayerONEList = playerOnePingListOLD.Except(playerOnePingList).ToList();
        List<POI> oldInPlayerTWOList = playerTwoPingListOLD.Except(playerOnePingList).ToList();

        if (newInPlayerONEList.Count > 0)
        {
            hudOne.AddPings(newInPlayerONEList);
        }

        if (newInPlayerTWOList.Count > 0)
        {
            hudTwo.AddPings(newInPlayerTWOList);
        }

        if (oldInPlayerONEList.Count > 0)
        {
            hudOne.RemovePings(oldInPlayerONEList);
        }

        if (oldInPlayerTWOList.Count > 0)
        {
            hudTwo.RemovePings(oldInPlayerTWOList);
        }
    }

    private List<POI> PingToPlayerList(POI ping, Vector3 playerPosition, float playerVisionRange, List<POI> playerPingList)
    {
        bool visible;
        float distanceToPlayer = TB.GetDistance(playerPosition, ping.position);
        visible = distanceToPlayer <= (int)ping.visibility + playerVisionRange;
        if (visible)
        {
            playerPingList.Add(ping);
        }

        return playerPingList;
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
}


