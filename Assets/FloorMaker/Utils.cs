using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static int[,] InitArray(int[,] array, int def)
    {
        int sizeD = array.GetLength(0);
        int sizeW = array.GetLength(1);

        for (int cd = 0; cd < sizeD; cd += 1)
        {
            for (int cw = 0; cw < sizeW; cw += 1)
            {
                array[cd, cw] = def;
            }
        }

        return array;
    }
}
