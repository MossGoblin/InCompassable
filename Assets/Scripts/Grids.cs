using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Itr = ToolBox.Itr;

public static class Grids
{
    public static int[,] CreateBasicGrid(int floorCellWidth, int floorCellDepth, int cellCols, int cellRows, Cell[,] gridCells)
    {

        int[,] gridBase = new int[floorCellWidth * cellCols, floorCellDepth * cellRows];

        foreach ((int countW, int countD) in Itr.Iteration(floorCellWidth, floorCellDepth))
        {
            Cell currentCell = gridCells[countW, countD];
            foreach ((int cellW, int cellD) in Itr.Iteration(currentCell.Width, currentCell.Depth))
            {
                int posW = countW * cellCols + cellW;
                int posD = countD * cellRows + cellD;
                int value = currentCell.Grid()[cellW, cellD];
                gridBase[posW, posD] = value;
                // TODO MarkPosition
                // MarkPosition(posW, posD, value, value, false);
            }
        }

        Debug.Log("Basic ON");

        return gridBase;
    }

    public static int[,] CreateBordersGrid(int width, int depth, int index)
    {
        // TODO MarkPosition
        int[,] grid = new int[width, depth];
        int podW = 0;
        int posD = 0;
        for (int countW = 0; countW < grid.GetLength(0); countW += 1)
        {
            podW = countW;
            posD = 0;
            grid[podW, posD] = index;
            // MarkPosition(podW, posD, 1, 1, false);
        }

        for (int countW = 0; countW < grid.GetLength(0); countW += 1)
        {
            podW = countW;
            posD = grid.GetLength(1) - 1;
            grid[podW, posD] = index;
            // MarkPosition(podW, posD, 1, 1, false);
        }

        for (int countD = 0; countD < grid.GetLength(1); countD += 1)
        {
            podW = 0;
            posD = countD;
            grid[podW, posD] = index;
            // MarkPosition(podW, posD, 1, 1, false);
        }

        for (int countD = 0; countD < grid.GetLength(1); countD += 1)
        {
            podW = grid.GetLength(0) - 1;
            posD = countD;
            grid[podW, posD] = index;
            // MarkPosition(podW, posD, 1, 1, false);
        }

        Debug.Log("Borders ON");
        return grid;
    }

    public static int[,] GenerateGridWithFill(int width, int depth, int fill)
    {
        int[,] result = new int[width, depth];
        foreach ((int countW, int countD) in Itr.Iteration(width, depth))
        {
            result[countW, countD] = fill;
        }

        return result;
    }

    public static int[,] Init(int[,] array, int index)
    {
        int sizeD = array.GetLength(0);
        int sizeW = array.GetLength(1);

        for (int cd = 0; cd < sizeD; cd += 1)
        {
            for (int cw = 0; cw < sizeW; cw += 1)
            {
                array[cd, cw] = index;
            }
        }

        return array;
    }

    public static bool[,] Init(bool[,] array, bool index)
    {
        int sizeD = array.GetLength(0);
        int sizeW = array.GetLength(1);

        for (int cd = 0; cd < sizeD; cd += 1)
        {
            for (int cw = 0; cw < sizeW; cw += 1)
            {
                array[cd, cw] = index;
            }
        }

        return array;
    }

    internal static int[,] ApplyMaskIndex(int[,] gridBase, bool[,] gridLock, int[,] gridMask, int indexMask, int indexBase)
    {
        // Applies mask to grid - translate indexMask from mask to indexBase in grid
        // sanity check
        if (gridBase.GetLength(0) != gridMask.GetLength(0) ||
            gridBase.GetLength(1) != gridMask.GetLength(1))
        {
            throw new ArgumentException("Grid dimentions don't match");
        }

        int width = gridBase.GetLength(0);
        int depth = gridBase.GetLength(1);

        foreach ((int countW, int countD) in Itr.Iteration(width, depth))
        {
            if (gridMask[countW, countD] == indexMask)
            {
                gridBase[countW, countD] = indexBase;
                gridLock[countW, countD] = false;
            }
        }

        return gridBase;
    }

    internal static int[,] ApplyMask(int[,] gridBase, int[,] gridMask, int avoidanceIndex)
    {
        // sanity check
        if (gridBase.GetLength(0) != gridMask.GetLength(0) ||
            gridBase.GetLength(1) != gridMask.GetLength(1))
        {
            throw new ArgumentException("Grid dimentions don't match");
        }

        int width = gridBase.GetLength(0);
        int depth = gridBase.GetLength(1);

        foreach ((int countW, int countD) in Itr.Iteration(width, depth))
        {
            if (gridMask[countW, countD] != avoidanceIndex)
            {
                gridBase[countW, countD] = gridMask[countW, countD];
            }
        }

        return gridBase;
    }

    internal static int[,] ApplyMask(int[,] gridBase, int[,] gridMask)
    {
        // sanity check
        if (gridBase.GetLength(0) != gridMask.GetLength(0) ||
            gridBase.GetLength(1) != gridMask.GetLength(1))
        {
            throw new ArgumentException("Grid dimentions don't match");
        }

        int width = gridBase.GetLength(0);
        int depth = gridBase.GetLength(1);

        foreach ((int countW, int countD) in Itr.Iteration(width, depth))
        {
            if (gridMask[countW, countD] > 0)
            {
                gridBase[countW, countD] = gridMask[countW, countD];
            }
        }

        return gridBase;
    }


    public static int[,] MarkPattern(int[,] grid, int posW, int posD, int[,] pattern, int index)
    {
        // Check if the pattern falls on an empty space
        bool interrupted = false;
        foreach ((int countW, int countD) in Itr.Iteration(pattern.GetLength(0), pattern.GetLength(1)))
        {
            if (grid[posW + countW, posD + countD] != 0)
            {
                interrupted = true;
                break;
            }
        }

        // mark the pattern on the grid
        if (!interrupted)
        {
            // mark all the cells of the pattern as 100 - to be avoided in the future
            foreach ((int countW, int countD) in Itr.Iteration(pattern.GetLength(0), pattern.GetLength(1)))
            {
                grid[posW + countW, posD + countD] = 100;
            }
            grid[posW, posD] = index;
            Debug.Log($"marked obj at {posW} / {posD}");
        }

        return grid;
    }

    internal static int[,] AddNbrs(int[,] grid, int minCount)
    {
        int width = grid.GetLength(0);
        int depth = grid.GetLength(1);

        int[,] result = Copy(grid);

        int[] pos = new int[4] { 0, 1, 0, -1 };

        foreach ((int countW, int countD) in Itr.IterationRange(1, width - 1, 1, depth - 1))
        {
            if (result[countW, countD] == 1)
            {
                continue;
            }

            int nbrs = 0;
            bool mark = false;
            for (int count = 0; count < 4; count += 1)
            {
                int nbrW = countW + pos[(count + 3) % 4];
                int nbrD = countD + pos[count];
                if (grid[nbrW, nbrD] != 0)
                {
                    nbrs++;
                    if (nbrs >= minCount)
                    {
                        mark = true;
                        break;
                    }
                }
            }
            if (mark)
            {
                result[countW, countD] = 1;
            }
        }

        return result;
    }
    public static int[,] Copy(int[,] origin)
    {
        int[,] target = new int[origin.GetLength(0), origin.GetLength(1)];

        foreach ((int countA, int countB) in Itr.Iteration(origin.GetLength(0), origin.GetLength(1)))
        {
            target[countA, countB] = origin[countA, countB];
        }

        return target;
    }

    internal static int[,] Flatten(int[,] grid, int flat)
    {
        int width = grid.GetLength(0);
        int depth = grid.GetLength(1);
        int[,] result = new int[width, depth];
        foreach ((int countW, int countD) in Itr.Iteration(width, depth))
        {
            if (grid[countW, countD] != flat)
            {
                result[countW, countD] = 1;
            }
            else
            {
                result[countW, countD] = 0;
            }
        }

        return result;
    }
}
