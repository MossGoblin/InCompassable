using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Map : MonoBehaviour
{
    // Prefabs
    [Header("Prefabs")]
    public List<Transform> mapElements;

    // Refs
    [Header("Refs")]
    public Transform mapHolder;
    public Transform pathFinder;

    // Dimensions
    [Header("Grid")]
    public int floorCellWidth = 8;
    public int floorCellDepth = 8;
    public int cellCols = 16;
    public int cellRows = 8;
    public int cellMin = 15;
    public int cellMax = 25;
    public int width { get; private set; }
    public int depth { get; private set; }

    // Layers
    [Header("Layers")]
    public int splotchNum = 3;
    public int splotchSize = 3;

    // Players
    [Header("Players")]
    public Transform playerOne;
    public Transform playerTwo;
    public bool visionRange;
    public float visibilityDistance;
    [Range(0.1f, 0.45f)]
    public float maxPointDeviation = 0.2f;


    // Grids
    private Cell[,] gridCells;
    private int[,] gridBase;
    private int[,] gridBorders;
    private int[,] gridNbrs;
    private int[,] gridSplotches;
    private int[,] gridTypes;
    public int[,] finalGrid;
    public Transform[,] gridElements;
    private Dictionary<Array, bool> grids; // to be reworked

    public void Start()
    {
        // Create the map
        CreateMap();
        PositionPlayers();
    }

    public void Update()
    {
        ClearOutOfRangeObstacles();
    }

    private void CreateMap()
    {
        /*
        grid creation

        1. create basic grid
        2. combine nbrs

        pois
        3. create splotches
        4. process down clusters

        5. combine grids

        X. place borders
        */

        InitGrids();

        // 1.
        CreateBasicGrid(); // DONE

        // 5.
        CreateBordersGrid(); // DONE

        // 2.
        CreateNbrsGrid(); // DONE

        // 4.
        ClumpNbrs(); // TODO

        SetBasicGridTypes(); // TODO

        // 3.
        CreateSplotches(); // DONE

        // 6.
        CollapseGrids(); // DONE


        // Mat.
        MaterializeFloor();

        // Color
        ColorizeGrid();

    }

    private void InitGrids()
    {
        // cache dimentions
        this.width = floorCellWidth * cellCols;
        this.depth = floorCellDepth * cellRows;

        grids = new Dictionary<Array, bool>();

        // Init cells grid
        gridCells = new Cell[floorCellWidth, floorCellDepth];

        // Generate floorCellsDepth * floorCellWidth cells
        for (int countW = 0; countW < floorCellWidth; countW += 1)
        {
            for (int countD = 0; countD < floorCellDepth; countD += 1)
            {
                gridCells[countW, countD] = new Cell(cellCols, cellRows, cellMin, cellMax);
            }
        }

        // Init basic grid
        gridBase = new int[width, depth];

        // Init borders grid
        gridBorders = new int[width, depth];

        // Init neighbours grid
        gridNbrs = new int[width, depth];

        // Init final grid
        finalGrid = new int[width, depth];

        // Init object grid
        gridElements = new Transform[width, depth];

        // Init splotch grid
        gridSplotches = new int[width, depth];

        // Init Points Of Interest grid
        gridTypes = new int[width, depth];

    }
    private void CreateBasicGrid()
    {
        for (int countW = 0; countW < floorCellWidth; countW += 1)
        {
            for (int countD = 0; countD < floorCellDepth; countD += 1)
            {
                Cell currentCell = gridCells[countW, countD];
                for (int cellD = 0; cellD < cellRows; cellD += 1)
                {
                    for (int cellW = 0; cellW < cellCols; cellW += 1)
                    {
                        //Debug.Log($"{countD}/{countW}: {cellD}/{cellW}");
                        int posW = countW * cellCols + cellW;
                        int posD = countD * cellRows + cellD;
                        gridBase[posW, posD] = currentCell.Grid()[cellW, cellD];
                    }
                }
            }
        }
        grids.Add(gridBase, true);
        Debug.Log("Basic ON");
    }

    private void SetBasicGridTypes()
    {
        for (int countW = 0; countW < width; countW++)
        {
            for (int countD = 0; countD < depth; countD++)
            {
                gridTypes[countW, countD] = finalGrid[countW, countD];
            }
        }
    }

    private void CreateNbrsGrid()
    {
        for (int countD = 1; countD < depth - 1; countD += 1)
        {
            for (int countW = 1; countW < width - 1; countW += 1)
            {
                int nbrCount = gridBase[countW, countD - 1] +
                                  gridBase[countW, countD + 1] +
                                  gridBase[countW - 1, countD] +
                                  gridBase[countW + 1, countD];

                if (nbrCount == 2)
                {
                    gridNbrs[countW, countD] = 1;
                }
            }
        }

        grids.Add(gridNbrs, true);
        Debug.Log("Nbrs ON");
    }

    private void CreateSplotches()
    {
        int previousColumn = 0;

        int splotchDev = (int)(depth / splotchNum);

        for (int count = 0; count < splotchNum; count += 1)
        {
            int border = 5;

            // choose X
            int rawW = Random.Range(border-1, width-border);

            // choose Y
            int rawD = previousColumn + Random.Range((int)(splotchDev * 1 / 2), (int)(splotchDev * 2));

            // normalize position
            int posW = rawW % (width - border);
            int posD = rawD % (depth - border);
            // TODO exclude the border properly

            // get magnitude
            int splotchMagnitude = (int)Random.Range(splotchSize / 2, splotchSize);

            PlaceSplotch(posW, posD, splotchMagnitude);
            previousColumn = posW;
        }

        grids.Add(gridSplotches, false);
        Debug.Log("Splotches ON");
    }

    private void ClumpNbrs()
    {
        // TODO So far only for coloring
        // Try to find all squares
        for (int countW = 0; countW < width - 2; countW++)
        {
            for (int countD = 0; countD < depth - 2; countD++)
            {
                if ((gridBase[countW, countD] == 1) &&
                    (gridBase[countW + 1, countD] == 1) &&
                    (gridBase[countW, countD + 1] == 1) &&
                    (gridBase[countW + 1, countD + 1] == 1))
                {
                    gridTypes[countW, countD] = 4;
                    gridTypes[countW + 1, countD] = 4;
                    gridTypes[countW, countD + 1] = 4;
                    gridTypes[countW + 1, countD + 1] = 4;
                }
            }
        }
    }

    private void CollapseGrids()
    {
        foreach (KeyValuePair<Array, bool> grid in grids)
        {
            finalGrid = OverlayGrids(finalGrid, (int[,])grid.Key, grid.Value);
        }
    }

    private void CreateBordersGrid()
    {
        for (int countW = 0; countW < gridBase.GetLength(0); countW += 1)
        {
            gridBorders[countW, 0] = 1;
        }

        for (int countW = 0; countW < gridBase.GetLength(0); countW += 1)
        {
            gridBorders[countW, gridBase.GetLength(1) - 1] = 1;
        }

        for (int countD = 0; countD < gridBase.GetLength(1); countD += 1)
        {
            gridBorders[0, countD] = 1;
        }

        for (int countD = 0; countD < gridBase.GetLength(1); countD += 1)
        {
            gridBorders[gridBase.GetLength(0) - 1, countD] = 1;
        }

        grids.Add(gridBorders, true);
        Debug.Log("Borders ON");
    }
    private void MaterializeFloor()
    {
        if (grids.Count == 0)
        {
            return;
        }

        // clear all previously created objects
        foreach (Transform obj in mapHolder)
        {
            Destroy(obj.gameObject);
        }

        int width = gridElements.GetLength(0);
        int depth = gridElements.GetLength(1);

        gridElements = new Transform[width, depth];
        for (int countD = 0; countD < depth; countD += 1)
        {
            for (int countW = 0; countW < width; countW += 1)
            {
                Transform obj;
                if (finalGrid[countW, countD] == 1)
                {
                    obj = mapElements[1];
                }
                else
                {
                    obj = mapElements[0];
                }
                // positionY = number of cells right * cell width + number of indices in the cell
                if (obj != null)
                {
                    int positionX = countW;
                    int positionY = countD;
                    gridElements[positionX, positionY] = Instantiate(obj, new Vector3(positionX, 0, positionY), Quaternion.identity);
                    gridElements[positionX, positionY].parent = mapHolder;
                }
            }
        }
    }

    private void PlaceSplotch(int posW, int posD, int mag)
    {
        for (int countW = posW - mag; countW <= posW + mag; countW += 1)
        {
            for (int countD = posD - mag; countD <= posD + mag; countD += 1)
            {
                if ((countW > 1) && (countW < width - 1) && // withn width (double border excluded)
                    (countD > 1) && (countD < depth - 1) && // within depth (double border excluded)
                    (Math.Sqrt(((countW - posW) * (countW - posW)) + ((countD - posD) * (countD - posD))) <= mag)) // within magnitude of the coordinates
                {
                    gridSplotches[countW, countD] = 1;
                }
            }
        }

        // Find bordering obstacles
        // Increase readius by 1
        // all cells in the larger radius but not in the original one, are on the border
        // check if there are obstacles - if so - mark them as POI
        int rimDepth = 2;
        for (int countW = posW - mag - rimDepth; countW <= posW + mag + rimDepth; countW += 1)
        {
            for (int countD = posD - mag - rimDepth; countD <= posD + mag + rimDepth; countD += 1)
            {
                if ((countW > 1) && (countW < width - 1) && // withn width (double border excluded)
                    (countD > 1) && (countD < depth - 1) && // within depth (double border excluded)
                    (Math.Sqrt(((countW - posW) * (countW - posW)) + ((countD - posD) * (countD - posD))) <= mag + rimDepth)) // within magnitude of the broader coordinates
                {
                    if (gridSplotches[countW, countD] == 0 && gridBase[countW, countD] == 1) // not within the strict coordinates and has an obstacle
                    {
                        gridTypes[countW, countD] = 3;
                    }
                }
            }
        }

        // mark the center as POI
        gridSplotches[posW, posD] = 0;
        gridTypes[posW, posD] = 2;
        gridBase[posW, posD] = 1;
    }

    private void ColorizeGrid()
    {
        // DBG only while WIP
        // Colorize POI

        // Get library and palette
        Palette palette = FindObjectOfType<Palette>();

        // Iterate type grid
        for (int countW = 0; countW < width; countW += 1)
        {
            for (int countD = 0; countD < depth; countD += 1)
            {
                gridElements[countW, countD].GetChild(0).GetComponent<MeshRenderer>().material.color = palette.palette[gridTypes[countW, countD]];
            }
        }
    }

    private int[,] OverlayGrids(int[,] finalGrid, int[,] grid, bool additive)
    {
        for (int countD = 0; countD < depth; countD += 1)
        {
            for (int countW = 0; countW < width; countW += 1)
            {
                if (grid[countW, countD] == 1)
                {
                    if (additive)
                    {
                        finalGrid[countW, countD] = 1;
                    }
                    else
                    {
                        finalGrid[countW, countD] = 0;
                    }
                }
            }
        }

        return finalGrid;
    }

    private void PositionPlayers()
    {
        pathFinder.GetComponent<PathFinder>().CreateSpawns(finalGrid, maxPointDeviation);
    }
    private void ClearOutOfRangeObstacles()
    {
        if (gridElements == null || visionRange == false)
        {
            return;
        }
        foreach (Transform obj in gridElements)
        {
            if (playerOne != null && playerTwo != null)
            {
                float distanceToOne = CheckDistance(obj, playerOne);
                float distanceToTwo = CheckDistance(obj, playerTwo);

                if (distanceToOne <= visibilityDistance || distanceToTwo <= visibilityDistance)
                {
                    obj.transform.gameObject.SetActive(true);
                }
                else
                {
                    obj.transform.gameObject.SetActive(false);
                }
            }

        }
    }
    private float CheckDistance(Transform objectOne, Transform objectTwo)
    {
        float distance;

        float sqrtTwo = Mathf.Sqrt(2f);

        float distX = Mathf.Abs(objectTwo.position.x - objectOne.position.x);
        float distZ = Mathf.Abs(objectTwo.position.z - objectOne.position.z);

        float smaller = Mathf.Min(distX, distZ);
        float larger = Mathf.Max(distX, distZ);

        distance = smaller * sqrtTwo + (larger - smaller);

        return distance;
    }

    public List<Vector3> GetNbrs(Vector3 position)
    {
        int posW = (int)position.x;
        int posD = (int)position.z;

        List<Vector3> nbrs = new List<Vector3>();

        int[] pos = new int[4] { 0, 1, 0, -1 };
        for (int count = 0; count < 4; count += 1)
        {
            int nbrD = posD + pos[count];
            int nbrW = posW + pos[(count + 3) % 4];
            if (finalGrid[nbrW, nbrD] == 0)
            {
                nbrs.Add(new Vector3(nbrW, 0, nbrD));
            }
        }
        return nbrs;
    }

}
