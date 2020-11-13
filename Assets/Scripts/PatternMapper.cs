﻿using System;
using System.Collections.Generic;

public static class PatternMapper
{
    public static List<(int, int)> FindPattern(int[,] grid, int[,] pattern)
    {
        List<(int, int)> result = new List<(int, int)>();
        // 1. iterate the grid
        // 2. apply pattern on each iteration
        // 3. xnor parrent with grid chunk
        // 4. if all is 1 -> record coordinates of leading cell

        int width = grid.GetLength(0);
        int depth = grid.GetLength(1);

        int patternWidth = pattern.GetLength(0);
        int patternDepth = pattern.GetLength(1);

        // 1.
        for (int countW = 0; countW < width - patternWidth + 1; countW++)
        {
            for (int countD = 0; countD < depth - patternDepth + 1; countD++)
            {
                // overlay at position
                bool match = Overlay(grid, pattern, countW, countD);
                if (match)
                {
                    result.Add((countW, countD));
                }
            }
        }

        // DBG
        return result;
    }

    private static bool Overlay(int[,] grid, int[,] pattern, int posW, int posD)
    {
        for (int countW = 0; countW < pattern.GetLength(0); countW ++)
        {
            for (int countD = 0; countD < pattern.GetLength(1); countD ++)
            {
                if (grid[posW + countW, posD + countD] != pattern[countW, countD])
                {
                    return false;
                }
            }
        }

        return true;
    }
}
