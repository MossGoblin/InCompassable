using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Library
{

    // Massive Types
    public enum Massives
    {
        Floor,
        Basic,
        Border,
        Square,
        DiagonalUp,
        DiagonalDown,
        AngleZero,
        AngleOne,
        AngleTwo,
        AngleThree,
        Ex,
        Cross,
        Obelisk,
        RingRim
    }

    public enum Patterns
    {
        Square, 
        DiagonalUp,
        DiagonalDown,
        AngleZero,
        AngleOne,
        AngleTwo,
        AngleThree,
        Exes,
        Crosses,
    }
    public static int[,] GetType(int typeNumber)
    {
        if (typeNumber > Enum.GetNames(typeof(Patterns)).Length)
        {
            throw new ArgumentException($"No such Pattern - index {typeNumber}");
        }

        int [,] pattern = new int[0,0];
        switch (typeNumber)
        {
            case (int)Patterns.Square:
                pattern = Square();
                break;
            case (int)Patterns.DiagonalUp:
                pattern = DiagonalUp();
                break;
            case (int)Patterns.DiagonalDown:
                pattern = DiagonalDown();
                break;
            case (int)Patterns.AngleZero:
                pattern = AngleZero();
                break;
            case (int)Patterns.AngleOne:
                pattern = AngleOne();
                break;
            case (int)Patterns.AngleTwo:
                pattern = AngleTwo();
                break;
            case (int)Patterns.AngleThree:
                pattern = AngleThree();
                break;
            case (int)Patterns.Exes:
                pattern = Exes();
                break;
            case (int)Patterns.Crosses:
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

    public static int[,] DiagonalDown()
    {
        int[,] pattern = new int[,] {
            {1, 0},
            {0, 1}
        };

        return pattern;
    }

    public static int[,] DiagonalUp()
    {
        int[,] pattern = new int[,] {
            {0, 1},
            {1, 0}
        };

        return pattern;
    }

    public static int[,] AngleZero()
    {
        int[,] pattern = new int[,] {
            {0, 1},
            {1, 1}
        };

        return pattern;
    }

    public static int[,] AngleOne()
    {
        int[,] pattern = new int[,] {
            {1, 0},
            {1, 1}
        };

        return pattern;
    }

    public static int[,] AngleTwo()
    {
        int[,] pattern = new int[,] {
            {1, 1},
            {0, 1}
        };

        return pattern;
    }

    public static int[,] AngleThree()
    {
        int[,] pattern = new int[,] {
            {1, 1},
            {1, 0}
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
}
