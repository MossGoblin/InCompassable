using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public static class Voronoi
{
    // private List<Vector2> centroidList;

    public static int[,] GenerateRegions(int width, int depth, List<Vector2> centroidList)
    {
        int[,] grid = new int[width, depth];

        foreach ((int countW, int countD) in TB.Iteration(width, depth))
        {
            float minDistance = float.MaxValue;
            int count = 0;
            foreach (Vector2 centroid in centroidList)
            {
                if (Vector3.Distance(new Vector2(countW, countD), centroid) < minDistance)
                {
                    minDistance = Vector3.Distance(new Vector2(countW, countD), centroid);
                    grid[countW, countD] = count;
                }
                count++;
            }
        }

        return grid;
    }
}
