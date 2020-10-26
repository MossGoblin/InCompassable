using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Floor : MonoBehaviour
{
    // Refs
    public Transform obstacle;
    public Transform empty;
    public Transform tileHolder;
    public Transform background;
    public Transform pathFinder;

    // Settings
    public bool basicGrid = true;
    public bool fillInBorders;
    public bool processNeighbours;
    public bool createSplotches;
    public bool showEmpties;
    public bool initSpawns;

    public int splotchNum = 3;
    public int splotchSize = 3;

    // Grid size and resolution
    public int floorCellDepth = 8;
    public int floorCellWidth = 8;
    public int cellRows = 8;
    public int cellCols = 16;
    public int cellMin = 15;
    public int cellMax = 25;

    // Grids
    private Cell[,] gridCells;
    private int[,] gridBasic;
    private int[,] gridBorders;
    private int[,] gridNbrs;
    private int[,] gridSplotches;
    private int[,] gridPOI;
    private Color[,] gridColors;
    public int[,] finalGrid;
    private Transform[,] gridFillObj;
    private Dictionary<Array, bool> grids;

    // Overall dimentions
    public int depth { get; private set; }
    public int width { get; private set; }

    void Start()
    {
        ResetGrids();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetGrids();
        }
    }
    void Update()
    {
        HandleInput();
    }

    private void ResetGrids()
    {
        InitGrids();
        CreateBasicGrid();
        CreateBordersGrid();
        CreateNbrsGrid();
        CreateSplotches();
        CombineGrids();
        MaterializeFloor();

        if (initSpawns)
        {
            pathFinder.GetComponent<PathFinder>().FindPath();
        }
    }

    private void InitGrids()
    {
        // cache dimentions
        this.depth = floorCellDepth * cellRows;
        this.width = floorCellWidth * cellCols;

        grids = new Dictionary<Array, bool>();

        // Init cells grid
        gridCells = new Cell[floorCellDepth, floorCellWidth];
        // Generate floorCellsDepth * floorCellWidth cells
        for (int countD = 0; countD < floorCellDepth; countD += 1)
        {
            for (int countW = 0; countW < floorCellWidth; countW += 1)
            {
                gridCells[countD, countW] = new Cell(cellRows, cellCols, cellMin, cellMax);
            }
        }

        // Init basic grid
        gridBasic = new int[depth, width];

        // Init borders grid
        gridBorders = new int[depth, width];

        // Init neighbours grid
        gridNbrs = new int[depth, width];

        // Init colors grid
        gridColors = new Color[floorCellDepth * cellRows, floorCellWidth * cellCols];

        // Init final grid
        finalGrid = new int[depth, width];

        // Init object grid
        gridFillObj = new Transform[depth, width];

        // Init splotch grid
        gridSplotches = new int[depth, width];

        // Init Points Of Interest grid
        gridPOI = new int[depth, width];
    }

    private void CreateBasicGrid()
    {
        if (!basicGrid)
        {
            return;
        }
        for (int countD = 0; countD < floorCellDepth; countD += 1)
        {
            for (int countW = 0; countW < floorCellWidth; countW += 1)
            {
                // Iterate within the cell
                float colorR = Random.Range(0f, 1f);
                float colorG = Random.Range(0f, 1f);
                float colorB = Random.Range(0f, 1f);

                Cell currentCell = gridCells[countD, countW];
                for (int cellD = 0; cellD < cellRows; cellD += 1)
                {
                    for (int cellW = 0; cellW < cellCols; cellW += 1)
                    {
                        Debug.Log($"{countD}/{countW}: {cellD}/{cellW}");
                        int positionX = countW * cellCols + cellW;
                        int positionY = countD * cellRows + cellD;
                        gridBasic[positionY, positionX] = currentCell.Grid()[cellD, cellW];
                        gridColors[positionY, positionX] = new Color(colorR, colorG, colorB);
                    }
                }
            }
        }
        grids.Add(gridBasic, true);
        Debug.Log("Basic ON");
    }

    private void CreateBordersGrid()
    {
        if (!fillInBorders)
        {
            return;
        }

        for (int countD = 0; countD < gridBasic.GetLength(0); countD += 1)
        {
            gridBorders[countD, 0] = 1;
        }

        for (int countD = 0; countD < gridBasic.GetLength(0); countD += 1)
        {
            gridBorders[countD, gridBasic.GetLength(1) - 1] = 1;
        }

        for (int countW = 0; countW < gridBasic.GetLength(1); countW += 1)
        {
            gridBorders[0, countW] = 1;
        }

        for (int countW = 0; countW < gridBasic.GetLength(1); countW += 1)
        {
            gridBorders[gridBasic.GetLength(0) - 1, countW] = 1;
        }

        grids.Add(gridBorders, true);
        Debug.Log("Borders ON");
    }

    private void CreateNbrsGrid()
    {
        if (!processNeighbours)
        {
            return;
        }

        for (int countD = 1; countD < depth - 1; countD += 1)
        {
            for (int countW = 1; countW < width - 1; countW += 1)
            {
                int numberCount = gridBasic[countD - 1, countW] +
                                  gridBasic[countD + 1, countW] +
                                  gridBasic[countD, countW - 1] +
                                  gridBasic[countD, countW + 1];

                if (numberCount == 2)
                {
                    gridNbrs[countD, countW] = 1;
                }
            }
        }

        grids.Add(gridNbrs, true);
        Debug.Log("Nbrs ON");
    }

    private void CombineGrids()
    {
        foreach (KeyValuePair<Array, bool> grid in grids)
        {
            finalGrid = OverlayGrids(finalGrid, (int[,])grid.Key, grid.Value);
        }
    }

    private int[,] OverlayGrids(int[,] finalGrid, int[,] grid, bool additive)
    {
        int[,] clone = new int[depth, width];
        for (int cd = 0; cd < depth; cd += 1)
        {
            for (int cw = 0; cw < width; cw += 1)
            {
                if (grid[cd, cw] == 1)
                {
                    if (additive)
                    {
                        finalGrid[cd, cw] = 1;
                    }

                    else
                    {
                        finalGrid[cd, cw] = 0;
                    }
                }
            }
        }

        return finalGrid;
    }

    private void CreateSplotches()
    {
        if (!createSplotches)
        {
            return;
        }

        int previousColumn = 0;

        int splotchDev = (int)(width / splotchNum);

        for (int count = 0; count < splotchNum; count += 1)
        {
            // choose X
            int rawD = Random.Range(0, depth);

            // choose Y
            int rawW = previousColumn + Random.Range((int)(splotchDev * 2 / 3), (int)(splotchDev * 3 / 2));

            // normalize position
            int posD = rawD % (depth - 2);
            int posW = rawW % (width - 2);

            // get magnitude
            int splotchMag = (int)Random.Range(splotchSize / 2, splotchSize);

            Splotch(posD, posW, splotchMag);
            previousColumn = posW;
        }

        grids.Add(gridSplotches, false);
    }

    private void Splotch(int posD, int posW, int mag)
    {
        gridPOI[posD, posW] = 1;

        for (int countD = posD - mag; countD <= posD + mag; countD += 1)
        {
            for (int countW = posW - mag; countW <= posW + mag; countW += 1)
            {
                if ((countD > 0) && (countD < depth - 1) && // within depth (border excluded)
                    (countW > 0) && (countW < width - 1) && // withn width (border excluded)
                    (Math.Sqrt(((countD - posD) * (countD - posD)) + ((countW - posW) * (countW - posW))) <= mag)) // within mag of the coordinates
                {
                    gridSplotches[countD, countW] = 1;
                    // DEBUG ONLY
                    gridColors[countD, countW] = Color.red;
                }
            }
        }
    }

    private void MaterializeFloor()
    {
        if (grids.Count == 0)
        {
            return;
        }

        // clear all previously created objects
        foreach (Transform obj in tileHolder)
        {
            Destroy(obj.gameObject);
        }

        int depth = gridFillObj.GetLength(0);
        int width = gridFillObj.GetLength(1);

        gridFillObj = new Transform[depth, width];
        for (int countD = 0; countD < depth; countD += 1)
        {
            for (int countW = 0; countW < width; countW += 1)
            {
                Transform obj;
                if (finalGrid[countD, countW] == 1)
                {
                    obj = obstacle;
                }
                else
                {
                    if (showEmpties)
                    {
                        obj = empty;
                    }
                    else obj = null;

                }
                // positionY = number of cells right * cell width + number of indeces in the cell
                if (obj != null)
                {
                    int positionX = countW;
                    int positionY = countD;
                    gridFillObj[positionY, positionX] = Instantiate(obj, new Vector3(positionX, 0, positionY), Quaternion.identity);
                    gridFillObj[positionY, positionX].GetComponent<MeshRenderer>().material.color = gridColors[positionY, positionX];
                    gridFillObj[positionY, positionX].parent = tileHolder;
                }
            }
        }
    }
    private int[,] CloneArray(int[,] grid)
    {
        int sd = grid.GetLength(0);
        int sw = grid.GetLength(1);
        int[,] clone = new int[sd, sw];
        for (int cd = 0; cd < sd; cd += 1)
        {
            for (int cw = 0; cw < cd; cw += 1)
            {
                clone[cd, cw] = grid[cd, cw];
            }
        }

        return clone;
    }
}

