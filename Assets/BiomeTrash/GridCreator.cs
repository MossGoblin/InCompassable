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
    private Transform[,] gridObj;
    private int[,] gridBiomes;

    // Flooding
    List<Vector3> objList;
    // Tiles to be checked
    Dictionary<int, Stack<Vector3>> tilesToCheck;

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

        threshold = (rndMin + rndMax) / 2;

        bool checking = true;

        tilesToCheck = new Dictionary<int, Stack<Vector3>>();
        tilesChecked = new int[width, depth];
        // put the biome seeds in the queue
        for (int count = 0; count < numberOfBiomes; count++)
        {
            tilesToCheck.Add(count, new Stack<Vector3>());
            tilesToCheck[count].Push(objList[count]);
        }

        int dbgCount = 0;

        while (checking)
        {
            int markedTiles = 0;

            // iterate all biomes
            for (int count = 0; count < numberOfBiomes; count++)
            {
                // pull next tile
                if (tilesToCheck[count].Count == 0)
                {
                    Debug.Log($"Dict {count} empty; checked {markedTiles}");
                    continue;
                }
                Vector3 currentTile = tilesToCheck[count].Pop();

                // enqueue unckecked nbrs
                List<Vector3> nbrs = GetNbrs(currentTile);
                foreach (Vector3 nbr in nbrs)
                {
                    tilesToCheck[count].Push(nbr);
                }

                // check the current tile
                if (gridRnd[(int)currentTile.x, (int)currentTile.z] >= threshold)
                {
                    // If the tile is unsuitable - adjust and add back to the queue
                    gridRnd[(int)currentTile.x, (int)currentTile.z] = Mathf.Min(gridRnd[(int)currentTile.x, (int)currentTile.z], gridRnd[(int)currentTile.x, (int)currentTile.z]- decrement);
                    tilesToCheck[count].Push(currentTile);
                }
                else
                {
                    gridBiomes[(int)currentTile.x, (int)currentTile.z] = count + 1;
                    // DBG InProcess coloring
                    gridObj[(int)currentTile.x, (int)currentTile.z].GetChild(0).GetComponent<MeshRenderer>().material.color = colors[count];
                    markedTiles++;
                }
            }

            int empty = 0;
            for (int count = 0; count < tilesToCheck.Count; count ++)
            {
                if (tilesToCheck[0].Count <= 0)
                {
                    empty++;
                    checking = false;
                }
            }

            if (dbgCount > (width * depth * 10))
            {
                Debug.Log($"Cycled out at {dbgCount}");
                break;
            }

            dbgCount++;

        }
    }

    private List<Vector3> GetNbrs(Vector3 tile)
    {
        List<Vector3> result = new List<Vector3>();
        for (int countW = -1; countW < 2; countW++)
        {
            for (int countD = -1; countD < 2; countD++)
            {
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
            if (tilesToCheck[count].Contains(new Vector3(posW, 0, posD)))
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
