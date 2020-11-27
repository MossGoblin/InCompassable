using System;
using UnityEngine;
using Itr = ToolBox.Itr;

[Serializable]
public class Element : MonoBehaviour
{
    // name
    public Library.Elements name;
    public int index { get; private set; }
    public int width;
    public int depth;
    public Transform prefab;

    // pattern
    public bool hasPattern;
    public int[] patternLine;
    public int[,] pattern { get; private set; }

    // params
    public int weight;
    public int density;
    public int[] angles;

    private void Awake()
    {
        Library library =  gameObject.GetComponentInParent<Library>();
        index = library.GetIndex(name);
        if (hasPattern)
        {
            ConstructPattern();
            weight = GetWeight(pattern);
            density = GetDensity(pattern);
        }
    }

    private void ConstructPattern()
    {
        pattern = new int[width, depth];
        foreach ((int countW, int countD) in Itr.Iteration(width, depth))
        {
            int index = countW * depth + countD;
            pattern[countW, countD] = patternLine[index];
        }
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
