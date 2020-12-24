using System;
using System.Collections.Generic;
using UnityEngine;

public class ElementsLibrary : MonoBehaviour
{
    public Dictionary<int, Element> elementPool;
    // [SerializeField] private Transform[] elementPrefabs; // OBS
    // Biomes
    [SerializeField] private Transform biomeLibrary;
    // Icon set
    [SerializeField] private RectTransform[] iconSet;

    // Element Types
    public enum Elements
    {
        Floor = 0,
        Basic = 1,
        Border = 2,
        Square = 3,
        Arc = 4,
        Angle = 5,
        Ex = 6,
        Cross = 7,
        Obelisk = 8,
        RingRim = 9
    }

    // Ranges of visibility
    public enum VisibilityRanges
    {
        Never = 0,
        // Small should be twice minimap range
        Small = 10,
        // Large should be about 3 times small
        Large = 30,
        // For safety Always should be set to map width
        Always = 100,
        // Center is for alway present center spire; should be visible from about 1/4 of the map width
        Center = 25

        /*
          - alvays visible
          - visible from large distance (absolute value)
          - visible from small distance (absolute value)
          - never visible
          - map relative visibility (example: 1/4 width)
        */

    }

    void Awake()
    {
        InitElements();
    }

    private void InitElements()
    {
        elementPool = new Dictionary<int, Element>();
        elementPool.Add((int)Elements.Floor, NewElement(Elements.Floor, false, VisibilityRanges.Never));
        elementPool.Add((int)Elements.Basic, NewElement(Elements.Basic, false, VisibilityRanges.Never));
        elementPool.Add((int)Elements.Border, NewElement(Elements.Border, false, VisibilityRanges.Never));
        elementPool.Add((int)Elements.Square, NewElement(Elements.Square, true, VisibilityRanges.Never));
        elementPool.Add((int)Elements.Arc, NewElement(Elements.Arc, true, VisibilityRanges.Never));
        elementPool.Add((int)Elements.Angle, NewElement(Elements.Angle, true, VisibilityRanges.Never));
        elementPool.Add((int)Elements.Ex, NewElement(Elements.Ex, true, VisibilityRanges.Small)); // small
        elementPool.Add((int)Elements.Cross, NewElement(Elements.Cross, true, VisibilityRanges.Small)); // small
        elementPool.Add((int)Elements.Obelisk, NewElement(Elements.Obelisk, false, VisibilityRanges.Large)); // large
        elementPool.Add((int)Elements.RingRim, NewElement(Elements.RingRim, false, VisibilityRanges.Never)); // large
    }

    private Element NewElement(Elements name, bool hasPattern, VisibilityRanges visibilityRange)
    {
        Element element = new Element();
        element.name = name;
        element.index = (int)name;
        element.hasPattern = hasPattern;
        if (hasPattern)
        {
            element.pattern = GetPattern(element.index);
            element.width = element.pattern.GetLength(0);
            element.depth = element.pattern.GetLength(1);
            element.weight = GetWeight(element.pattern);
            element.density = element.weight / element.width * element.depth;
        }
        element.visibilityRange = visibilityRange;

        return element;


        /*
        Floor = 0,
        Basic = 1,
        Border = 2,
        Square = 3,
        Arc = 4,
        Angle = 5,
        Ex = 6,
        Cross = 7,
        Obelisk = 8,
        RingRim = 9
        */
    }

    public static int GetNumberOfElements()
    {
        return Enum.GetNames(typeof(Elements)).Length;
    }

    public RectTransform GetIcon(int elementIndex)
    {
        // DBG plug - no icons yet
        return iconSet[elementIndex];
        // return null;
    }

    public static int[,] GetPattern(int typeNumber)
    {
        if (typeNumber > Enum.GetNames(typeof(Elements)).Length + 3)
        {
            throw new ArgumentException($"No such Pattern - index {typeNumber}");
        }

        int[,] pattern = new int[0, 0];
        switch (typeNumber)
        {
            case (int)Elements.Square:
                pattern = Square();
                break;
            case (int)Elements.Arc:
                pattern = Arc();
                break;
            case (int)Elements.Angle:
                pattern = Angle();
                break;
            case (int)Elements.Ex:
                pattern = Exes();
                break;
            case (int)Elements.Cross:
                pattern = Crosses();
                break;
        }

        return pattern;
    }

    public static int[,] Crosses()
    {
        int[,] patern = new int[,] {
            {0, 1, 0},
            {1, 1, 1},
            {0, 1, 0}
        };

        return patern;
    }

    public static int[,] Exes()
    {
        int[,] pattern = new int[,] {
            {1, 0, 1},
            {0, 1, 0},
            {1, 0, 1}
        };

        return pattern;
    }

    public static int[,] Arc()
    {
        int[,] pattern = new int[,] {
            {0, 1},
            {1, 0}
        };

        return pattern;
    }

    public static int[,] Angle()
    {
        int[,] pattern = new int[,] {
            {0, 1},
            {1, 1}
        };

        return pattern;
    }

    public static int[,] Square()
    {
        int[,] pattern = new int[,] {
            {1, 1},
            {1, 1}
        };

        return pattern;
    }

    private int GetWeight(int[,] pattern)
    {
        int counter = 0;
        foreach ((int countW, int countD) in TB.Iteration(pattern.GetLength(0), pattern.GetLength(1)))
        {
            if (pattern[countW, countD] == 1)
            {
                counter++;
            }
        }

        return counter;
    }

    public int GetIndex(Elements massive)
    {
        return (int)massive;
    }


    public static int[] GetAngles(int typeNumber)
    {
        if (typeNumber > Enum.GetNames(typeof(Elements)).Length + 3)
        {
            throw new ArgumentException($"No such Pattern - index {typeNumber}");
        }

        int[] angles = new int[0];
        switch (typeNumber)
        {
            case (int)Elements.Square:
                angles = new int[1] { 0 };
                break;
            case (int)Elements.Arc:
                angles = new int[2] { 0, 1 };
                break;
            case (int)Elements.Angle:
                angles = new int[4] { 0, 1, 2, 3 };
                break;
            case (int)Elements.Ex:
                angles = new int[1] { 0 };
                break;
            case (int)Elements.Cross:
                angles = new int[1] { 0 };
                break;
        }

        return angles;
    }

    public Transform GetElement(int biomeIndex, int elementIndex)
    {
        return biomeLibrary.GetComponent<BiomesLibrary>().GetElement(biomeIndex, elementIndex);
    }
}



public struct Element
{
    public ElementsLibrary.Elements name;
    public int index;
    public int width;
    public int depth;

    public ElementsLibrary.VisibilityRanges visibilityRange;
    // pattern
    public bool hasPattern;
    public int[,] pattern;

    // params
    public int weight;
    public int density;
}
