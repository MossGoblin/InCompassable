using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI
{
    public Vector3 position { get; private set; }

    public RectTransform prefab { get; set; }
    public ElementsLibrary.VisibilityRanges visibility { get; private set; }
    public POI(Vector3 pos, RectTransform pref, ElementsLibrary.VisibilityRanges visibility)
    {
        this.position = pos;
        this.prefab = pref;
        this.visibility = visibility;
    }
}
