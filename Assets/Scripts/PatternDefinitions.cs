using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Patterns
{

    public enum Pattern
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
        if (typeNumber > Enum.GetNames(typeof(Pattern)).Length)
        {
            throw new ArgumentException($"No such Pattern - index {typeNumber}");
        }

        int [,] pattern = new int[0,0];
        switch (typeNumber)
        {
            case (int)Pattern.Square:
                pattern = Square();
                break;
            case (int)Pattern.DiagonalUp:
                pattern = DiagonalUp();
                break;
            case (int)Pattern.DiagonalDown:
                pattern = DiagonalDown();
                break;
            case (int)Pattern.AngleZero:
                pattern = AngleZero();
                break;
            case (int)Pattern.AngleOne:
                pattern = AngleOne();
                break;
            case (int)Pattern.AngleTwo:
                pattern = AngleTwo();
                break;
            case (int)Pattern.AngleThree:
                pattern = AngleThree();
                break;
            case (int)Pattern.Exes:
                pattern = Exes();
                break;
            case (int)Pattern.Crosses:
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
