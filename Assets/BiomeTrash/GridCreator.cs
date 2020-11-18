using System.Collections;
using System.Collections.Generic;
using ToolBox;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridCreator : MonoBehaviour
{
    [Header("Dimentions")]
    public int width;
    public int depth;

    [Header("Biomes")]
    public bool useSeed;
    public int seed;
    public float rndMin;
    public float rndMax;
    private float threshold;
    public float decrement;
    public int numberOfBiomes;
    public Color[] colors;

    [Header("Tiles")]
    public Transform holder;
    public Transform basicTile;

    private float[,] gridRnd;
    private int[,] gridBiomes;
    private Transform[,] gridObj;

    // Flooding
    List<Vector3> objList;
    // Tiles to be checked
    Dictionary<int, Queue<Vector3>> borders;

    // Claimed tiles
    int[,] tilesChecked;

    public void Start()
    {
        CreateGrid();
        CreateBiomes();
    }

    public void Update()
    {
        // StartCoroutine("FloodGrid");
    }

    private void CreateGrid()
    {
        Randomize();
        CreateObjects();
    }

    private void Randomize()
    {
        // Get random seed
        if (!useSeed)
        {
            seed = Random.Range(100000, 999999);
        }

        Random.InitState(seed);

        gridRnd = new float[width, depth];

        Debug.Log($"seed: {seed}");

        foreach ((int countW, int countD) in Iterator.Iteration(width, depth))
        {
            gridRnd[countW, countD] = Random.Range(rndMin, rndMax);
        }
    }

    private void CreateObjects()
    {
        gridObj = new Transform[width, depth];
        foreach ((int countW, int countD) in Iterator.Iteration(width, depth))
        {
            float height = gridRnd[countW, countD];
            Vector3 position = new Vector3((float)countW, (float)countD, height);
            gridObj[countW, countD] = Instantiate(basicTile, position, Quaternion.identity, holder);
        }
    }

    private void CreateBiomes()
    {
        gridBiomes = new int[width, depth];
        // colors = new Color[numberOfBiomes];
        CreateCenters();
        FloodGrid();
    }

    private void CreateCenters()
    {
        objList = new List<Vector3>();
        int intervals = numberOfBiomes + 1;
        int intervalsLength = ((int)width / (numberOfBiomes * 2));
        int lowerLine = (int)(depth / 6) - 1;
        int upperLine = (int)(depth / 6) * 5 - 1;
        Debug.Log($"lines: {lowerLine}/{upperLine}");

        for (int count = 0; count < numberOfBiomes; count++)
        {
            // =(S4*2 + 1)*$A$2 + S4
            int posW = (count * 2 + 1) * intervalsLength + count;
            int posD = (count % 2 == 0) ? upperLine : lowerLine;
            gridObj[posW, posD].GetChild(0).GetComponent<MeshRenderer>().material.color = colors[count];
            objList.Add(new Vector3(posW, 0, posD));
            Debug.Log($"{count}: {posW}/{posD}");
        }
    }

    private void FloodGrid()
    {

        float threshold = (rndMin + rndMax) / 2;

        // create separate checklists for each biome
        borders = new Dictionary<int, Queue<Vector3>>();
        for (int count = 0; count < numberOfBiomes; count++)
        {
            Queue<Vector3> queue = new Queue<Vector3>();
            queue.Enqueue(objList[count]);
            borders.Add(count, queue);
        }

        // iterate floodpoints
        // for each - make one full run of the border
        bool flooding = true;

        int markedTiles = 0;
        int cycleCounter = 0;
        while (flooding)
        {
            // cycle floodpoints
            for (int biomeNumber = 0; biomeNumber < numberOfBiomes; biomeNumber++)
            {
                // run full cycle from the border
                int borderSize = borders[biomeNumber].Count;

                for (int borderCount = 0; borderCount < borderSize; borderCount++)
                {
                    // safety
                    if (borders[biomeNumber].Count <= 0)
                    {
                        Debug.Log($"Biome {biomeNumber} is done at {markedTiles}");
                    }

                    // extract
                    Vector3 tile = borders[biomeNumber].Dequeue();

                    // process tile
                    if (gridRnd[(int)tile.x, (int)tile.z] <= threshold)
                    {
                        gridBiomes[(int)tile.x, (int)tile.z] = biomeNumber;
                        gridObj[(int)tile.x, (int)tile.z].GetChild(0).GetComponent<MeshRenderer>().material.color = colors[biomeNumber];
                        markedTiles ++;
                    }
                    else
                    {
                        // adjust tile and return to queue
                        gridRnd[(int)tile.x, (int)tile.z] -= decrement;
                        borders[biomeNumber].Enqueue(tile);
                    }

                    // fill in nbrs
                    List<Vector3> nbrs = GetNbrs(tile);
                    foreach (Vector3 nbr in nbrs)
                    {
                        borders[biomeNumber].Enqueue(nbr);
                    }
                }
            }
            
            cycleCounter++;

            // check for cycle break
            if (markedTiles >= (width * depth * 15))
            {
                Debug.Log($"Grace at {markedTiles}/{cycleCounter}");
                flooding = false;
            }
        }
    }

    private List<Vector3> GetNbrs(Vector3 tile)
    {
        List<Vector3> result = new List<Vector3>();
        for (int countW = -1; countW < 2; countW++)
        {
            for (int countD = -1; countD < 2; countD++)
            {
                // if (countW != 0 && countD != 0)
                if (Mathf.Abs(countW) + Mathf.Abs(countD) == 1)
                {
                    if (FreeTile(countW + (int)tile.x, countD + (int)tile.z))
                    {
                        result.Add(new Vector3(countW + (int)tile.x, 0, countD + (int)tile.z));
                    }
                }
            }
        }

        return result;
    }

    private bool FreeTile(int posW, int posD)
    {
        if ((posW < 0) ||
            (posW >= width) ||
            (posD < 0) ||
            (posD >= depth))
        {
            return false;
        }

        for (int count = 0; count < numberOfBiomes; count++)
        {
            if (borders[count].Contains(new Vector3(posW, 0, posD)))
            {
                return false;
            }
        }

        if (gridBiomes[posW, posD] > 0)
        {
            return false;
        }

        return true;
    }
}
