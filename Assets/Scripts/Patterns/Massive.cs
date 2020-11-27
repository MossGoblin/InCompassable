using System;
using UnityEngine;
using Itr = ToolBox.Itr;

[Serializable]
public class Massive : MonoBehaviour
{
    // name
    public Library.Patterns name;
    public int width;
    public int depth;
    public Transform prefab;

    // pattern
    private int[,] pattern;
    // params
    public int weight;
    public int density;

    private void Start()
    {
        pattern = Library.GetType(name);
        width = pattern.GetLength(0);
        depth = pattern.GetLength(1);
        weight = GetWeight(pattern);
        density = GetDensity(pattern);
    }

    private int GetWeight(int[,] pattern)
    {
        int count = 0;
        foreach ((int countW, int countD) in Itr.Iteration(width, depth))
        {
            if (pattern[countW, countD] == 1)
            {
                count++;
            }
        };

        return count;
    }

    private int GetDensity(int[,] pattern)
    {
        return weight / pattern.GetLength(0) * pattern.GetLength(1);
    }

}
