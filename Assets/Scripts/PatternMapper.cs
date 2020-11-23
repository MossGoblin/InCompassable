using System;
using System.Collections.Generic;

public static class PatternMapper
{
    public static List<(int, int)> FindPattern(int[,] gridPositive, int[,] pattern)
    {
        List<(int, int)> result = new List<(int, int)>();
        List<(int, int)> counted = new List<(int, int)>();

        // 1. iterate the grid
        // 2. apply pattern on each iteration
        // 3. xnor parrent with grid chunk
        // 4. if all is 1 -> record coordinates of leading cell

        int width = gridPositive.GetLength(0);
        int depth = gridPositive.GetLength(1);

        int patternWidth = pattern.GetLength(0);
        int patternDepth = pattern.GetLength(1);


        // 1.
        for (int countW = 1; countW < width - patternWidth; countW++)
        {
            for (int countD = 1; countD < depth - patternDepth; countD++)
            {
                if (counted.Contains((countW, countD)))
                {
                    continue;
                }
                // overlay at position
                bool match = Overlay(gridPositive, pattern, countW, countD, counted);
                if (match)
                {
                    Mark(gridPositive, pattern, countW, countD, ref counted);
                    result.Add((countW, countD));
                }
            }
        }

        // DBG
        return result;
    }

    private static bool Overlay(int[,] gridPositive, int[,] pattern, int posW, int posD, List<(int, int)> counted)
    {
        for (int countW = 0; countW < pattern.GetLength(0); countW ++)
        {
            for (int countD = 0; countD < pattern.GetLength(1); countD ++)
            {
                if (counted.Contains((posW + countW, posD + countD)) || 
                    gridPositive[posW + countW, posD + countD] != pattern[countW, countD])
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static void Mark(int[,] grid, int[,] pattern, int posW, int posD, ref List<(int, int)> counted)
    {
        for (int countW = 0; countW < pattern.GetLength(0); countW ++)
        {
            for (int countD = 0; countD < pattern.GetLength(1); countD ++)
            {
                counted.Add((posW + countW, posD + countD));
            }
        }
    }
}
