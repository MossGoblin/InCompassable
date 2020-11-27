using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Library : MonoBehaviour
{

    public Dictionary<int, Element> elementPool;
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

    void Awake()
    {
        elementPool = new Dictionary<int, Element>();
        foreach (Transform element  in transform)
        {
            Element elementData = element.GetComponent<Element>();
            elementPool.Add(elementData.index, elementData);
        }
    }
    public int[,] GetType(int typeNumber)
    {
        return elementPool[typeNumber].pattern;
    }


    // public static int[,] GetType(int typeNumber)
    // {
    //     if (typeNumber > Enum.GetNames(typeof(Elements)).Length + 3)
    //     {
    //         throw new ArgumentException($"No such Pattern - index {typeNumber}");
    //     }

    //     int[,] pattern = new int[0, 0];
    //     switch (typeNumber)
    //     {
    //         case (int)Elements.Square:
    //             pattern = Square();
    //             break;
    //         case (int)Elements.Arc:
    //             pattern = Arc();
    //             break;
    //         case (int)Elements.Angle:
    //             pattern = Angle();
    //             break;
    //         case (int)Elements.Ex:
    //             pattern = Exes();
    //             break;
    //         case (int)Elements.Cross:
    //             pattern = Crosses();
    //             break;
    //         default:
    //             break;
    //     }

    //     return pattern;
    // }

    public int[,] GetType(Elements patternName)
    {
        int[,] pattern = GetType((int)patternName);
        return pattern;
    }

    public int GetIndex(Elements massive)
    {
        return (int)massive;
    }

    // public static int[,] Crosses()
    // {
    //     int[,] patern = new int[,] {
    //         {0, 1, 0},
    //         {1, 1, 1},
    //         {0, 1, 0}
    //     };

    //     return patern;
    // }

    // public static int[,] Exes()
    // {
    //     int[,] pattern = new int[,] {
    //         {1, 0, 1},
    //         {0, 1, 0},
    //         {1, 0, 1}
    //     };

    //     return pattern;
    // }

    // public static int[,] Arc()
    // {
    //     int[,] pattern = new int[,] {
    //         {0, 1},
    //         {1, 0}
    //     };

    //     return pattern;
    // }

    // public static int[,] Angle()
    // {
    //     int[,] pattern = new int[,] {
    //         {0, 1},
    //         {1, 1}
    //     };

    //     return pattern;
    // }

    // public static int[,] Square()
    // {
    //     int[,] pattern = new int[,] {
    //         {1, 1},
    //         {1, 1}
    //     };

    //     return pattern;
    // }

    public int[] GetAngles(int typeNumber)
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

    public int[] GetAngles(Elements patternName)
    {
        int[] angles = GetAngles((int)patternName);
        return angles;
    }
}
