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
    public int numberOfBiomes;
    public Color[] colors;

    [Header("Tiles")]
    public Transform holder;
    public Transform basicTile;

    private float[,] gridRnd;
    private Transform[,] gridObj;
    private int[,] gridBiomes;

    public void Start()
    {
        CreateGrid();
        CreateBiomes();
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
            Debug.Log($"{count}: {posW}/{posD}");
        }
    }


    private void FloodGrid()
    {
        Dictionary<int, List<Vector3>> biomesCheckedTiles;
    }

    private bool FreeTile()
    {
        return true;
    }
}
