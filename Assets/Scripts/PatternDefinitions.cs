using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Patterns
{

    public enum Pattern
    {
        Crosses,
        Exes,
        AngleZero,
        AngleOne,
        AngleTwo,
        AngleThree,
        DiagonalDown,
        DiagonalUp,
        Square
    }
    public static int[,] GetType(int typeNumber)
    {
        if (typeNumber > Enum.GetNames(typeof(Pattern)).Length)
        {
            throw new ArgumentException($"No such Pattern - index {typeNumber}");
        }

        int [,] pattern = new int[0,0];
        switch (typeNumber)
        {
            case 0:
                pattern = Square();
                break;
            case 1:
                pattern = AngleZero();
                break;
            case 2:
                pattern = AngleOne();
                break;
            case 3:
                pattern = AngleTwo();
                break;
            case 4:
                pattern = AngleThree();
                break;
            case 5:
                pattern = DiagonalDown();
                break;
            case 6:
                pattern = DiagonalUp();
                break;
            case 7:
                pattern = Exes();
                break;
            case 8:
                pattern = Crosses();
                break;
        }

        return pattern;
    }

    public static int[,] GetType(Pattern patternName)
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
