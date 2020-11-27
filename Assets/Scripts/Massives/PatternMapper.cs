using System;
using System.Collections.Generic;
using Itr = ToolBox.Itr;

public static class PatternMapper
{
    public static List<(int, int)> FindPattern(ref int[,] grid, ref bool[,] gridLock, int[,] pattern)
    {
        List<(int, int)> result = new List<(int, int)>();
        List<(int, int)> countedPositions = new List<(int, int)>();


        (int width, int depth) = Grids.Dim(grid);

        int patternWidth = pattern.GetLength(0);
        int patternDepth = pattern.GetLength(1);


        // 1.
        foreach ((int countW, int countD) in Itr.IterationRange(1, width - patternWidth, 1, depth - patternDepth))
        {
            if (countedPositions.Contains((countW, countD)) || gridLock[countW, countD])
            {
                continue;
            }
            // overlay at position
            bool match = Overlay(grid, gridLock, pattern, countW, countD, countedPositions);
            if (match)
            {
                MarkAsCounted(pattern, ref grid, ref gridLock, countW, countD, ref countedPositions);
                result.Add((countW, countD));
            }
        }

        return result;
    }

    private static bool Overlay(int[,] grid, bool[,] gridLock, int[,] pattern, int posW, int posD, List<(int, int)> counted)
    {
        foreach ((int countW, int countD) in Itr.Iteration(pattern.GetLength(0), pattern.GetLength(1)))
        {
            if (counted.Contains((posW + countW, posD + countD)) ||             // if the cell has not been checked
                gridLock[posW + countW, posD + countD] ||                       // and the position is not locked
                grid[posW + countW, posD + countD] != pattern[countW, countD])  // and the value at the position matches the value in the pattern (only 1 or 0)
            {
                return false;
            }
        }

        return true;
    }

    private static void MarkAsCounted(int[,] pattern, ref int[,] grid, ref bool[,] gridLock, int posW, int posD, ref List<(int, int)> counted)
    {
        foreach ((int countW, int countD) in Itr.Iteration(pattern.GetLength(0), pattern.GetLength(1)))
        {
            counted.Add((posW + countW, posD + countD)); // mark position as checked
            grid[posW + countW, posD + countD] = 0; // clear position of objects, if any (mainly basics)
            gridLock[posW + countW, posD + countD] = true; // lock position for further modification
        }   
    }
}
