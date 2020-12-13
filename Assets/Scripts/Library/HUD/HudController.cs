using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Transform[] huds;
    public List<Element> globalPOIList;
    public List<Element>[] playerPingList;
    public Transform[] players;
    void Awake()
    {
        // INIT
        // get players
        // Distribute parity
        DistributeParity();
    }

    // Update is called once per frame
    void Update()
    {
        // update global ping list
    }

    private void DistributeParity()
    {
        if (huds[0].transform.position.x < huds[1].transform.position.x)
        {
            huds[0].GetComponent<Hud>().SetParity(0);
            huds[1].GetComponent<Hud>().SetParity(1);
        }
        else if (huds[0].transform.position.x < huds[1].transform.position.x)
        {
            huds[0].GetComponent<Hud>().SetParity(1);
            huds[1].GetComponent<Hud>().SetParity(0);
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


