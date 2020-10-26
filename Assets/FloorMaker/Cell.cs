using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    private int[,] grid;
    public int Depth { get; private set; }
    public int Width { get; private set; }
    public int Max { get; private set; }
    public int Min { get; private set; }
    private int used;
    private int counted;
    public Cell(int depth, int width, int min, int max)
    {
        this.Depth = depth;
        this.Width = width;
        this.Max = max;
        this.Min = min;

        InitGrid();
    }

    public int[,] Grid()
    {
        return this.grid;
    }
    private void InitGrid()
    {
        this.counted = Depth * Width;
        this.used = UnityEngine.Random.Range(Min, Max) + 1;
        grid = new int[Depth, Width];
        for (int countD = 0; countD < Depth; countD += 1)
        {
            for (int countW = 0; countW < Width; countW += 1)
            {
                grid[countD, countW] = GetCellValue();
            }
        }
    }

    private int GetCellValue()
    {

        // Generate number in counted range
        // If equal or less than used - return 1; decrease used
        // Decrease counted
        int result = 0;
        int index = UnityEngine.Random.Range(0, counted);
        if (index < used)
        {
            result = 1;
            used -= 1;
        }
        counted -= 1;

        return result;
    }
}
