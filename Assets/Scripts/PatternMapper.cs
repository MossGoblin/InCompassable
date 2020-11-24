using System;
using System.Collections.Generic;
using Itr = ToolBox.Itr;

public static class PatternMapper
{
    public static List<(int, int)> FindPattern(int[,] grid, int[,] pattern)
    {
        List<(int, int)> result = new List<(int, int)>();
        List<(int, int)> countedPositions = new List<(int, int)>();

        int width = grid.GetLength(0);
        int depth = grid.GetLength(1);

        int patternWidth = pattern.GetLength(0);
        int patternDepth = pattern.GetLength(1);


        // 1.
        foreach ((int countW, int countD) in Itr.IterationRange(1, width - patternWidth, 1, patternDepth))
        {
            if (countedPositions.Contains((countW, countD)))
            {
                continue;
            }
            // overlay at position
            bool match = Overlay(grid, pattern, countW, countD, countedPositions);
            if (match)
            {
                MarkAsCounted(pattern, countW, countD, ref countedPositions);
                result.Add((countW, countD));
            }
        }

        return result;
    }

    private static bool Overlay(int[,] grid, int[,] pattern, int posW, int posD, List<(int, int)> counted)
    {
        foreach ((int countW, int countD) in Itr.Iteration(pattern.GetLength(0), pattern.GetLength(1)))
        {
            if (counted.Contains((posW + countW, posD + countD)) ||
                grid[posW + countW, posD + countD] != pattern[countW, countD])
            {
                return false;
            }
        }

        return true;
    }

    private static void MarkAsCounted(int[,] pattern, int posW, int posD, ref List<(int, int)> counted)
    {
        foreach ((int countW, int countD) in Itr.Iteration(pattern.GetLength(0), pattern.GetLength(1)))
        {
            counted.Add((posW + countW, posD + countD));
        }
    }
}
