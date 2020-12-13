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
    private int chirality;
    
    public void SetParity(int chirality)
    {
        this.chirality = chirality;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // get pingList from hudgroup
        // pingList = GetComponentInParent<HudController>().GetPingList(chirality);
    }
}
