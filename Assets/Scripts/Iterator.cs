using System.Collections.Generic;

namespace ToolBox
{
    public static class Itr
    {
        public struct Step
        {
            public int outer;
            public int inner;

            public Step(int o, int i)
            {
                outer = o;
                inner = i;
            }
        }
        static public IEnumerable<Step> IterationWithStruct(int bound_i, int bound_b)
        {

            List<Step> loop = new List<Step>();
            for (int i = 0; i < bound_i; i++)
            {
                for (int y = 0; y < bound_b; y++)
                {
                    Step newStep = new Step(i, y);
                    loop.Add(newStep);
                }
            }

            return loop;
        }

        static public IEnumerable<(int i, int y)> Iteration(int bound_i, int bound_b)
        {
            List<(int i, int y)> loop = new List<(int i, int y)>();
            for (int i = 0; i < bound_i; i++)
            {
                for (int y = 0; y < bound_b; y++)
                {
                    loop.Add((i, y));
                }
            }

            return loop;
        }

        static public IEnumerable<(int i, int y, int c)> IterationWithCount(int bound_i, int bound_b)
        {
            List<(int i, int y, int c)> loop = new List<(int i, int y, int c)>();
            int count = 0;
            for (int i = 0; i < bound_i; i++)
            {
                for (int y = 0; y < bound_b; y++)
                {
                    loop.Add((i, y, count));
                    count++;
                }
            }

            return loop;
        }

        static public IEnumerable<(int i, int y)> IterationRange(int lower_i, int higher_i, int lower_y, int higher_b)
        {
            List<(int i, int y)> loop = new List<(int i, int y)>();
            for (int i = lower_i; i < higher_i; i++)
            {
                for (int y = lower_y; y < higher_b; y++)
                {
                    loop.Add((i, y));
                }
            }

            return loop;
        }

        static public int[,] GenerateGridWithFill(int width, int depth, int fill)
        {
            int[,] result = new int[width, depth];
            foreach ((int countW, int countD) in Iteration(width, depth))
            {
                result[countW, countD] = fill;
            }

            return result;
        }

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
}