﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI
{
    public Vector3 position { get; private set; }

    public RectTransform icon { get; set; }
    public ElementsLibrary.VisibilityRanges visibility { get; private set; }
    public POI(Vector3 pos, RectTransform prefab, ElementsLibrary.VisibilityRanges visibility)
    {
        this.position = pos;
        this.icon = prefab;
        this.visibility = visibility;
    }
}
