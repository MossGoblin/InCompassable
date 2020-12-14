using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hud : MonoBehaviour
{
    // The hud of one of the minimaps
    // props
    // left or right hud ??
    // definitions - width of minimap, width of border, etc
    // a list of pings
    // - - -
    // List of pings to be projected
    private List<POI> pingList;
    //  0 for left, 1 for right

    [SerializeField]
    private int chirality;
    
    public void SetChirality(int chirality)
    {
        this.chirality = chirality;
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public void UpdatePingList(List<POI> pingList)
    {
        this.pingList = pingList;
    }
}
