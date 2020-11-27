using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Library
{

    // Massive Types
    public enum Massives
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

    public enum Patterns
    {
        Square = 3,
        Arc = 4,
        Angle = 5,
        Ex = 6,
        Cross = 7
    }
    public static int[,] GetType(int typeNumber)
    {
        if (typeNumber > Enum.GetNames(typeof(Patterns)).Length + 3)
        {
            throw new ArgumentException($"No such Pattern - index {typeNumber}");
        }

        int[,] pattern = new int[0, 0];
        switch (typeNumber)
        {
            case (int)Patterns.Square:
                pattern = Square();
                break;
            case (int)Patterns.Arc:
                pattern = Arc();
                break;
            case (int)Patterns.Angle:
                pattern = Angle();
                break;
            case (int)Patterns.Ex:
                pattern = Exes();
                break;
            case (int)Patterns.Cross:
                pattern = Crosses();
                break;
        }

        return pattern;
    }

    public static int[,] GetType(Patterns patternName)
    {
        int[,] pattern = GetType((int)patternName);
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

    public int[] GetAngles()
    {
        // HERE
        return null;
    }
}
