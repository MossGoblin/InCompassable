using System;
using System.Collections.Generic;
using UnityEngine;
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
    public bool colorTiles;
    public bool initSpawns;
    public bool findPath;

    // Grid size and resolution
    public int floorCellWidth = 8;
    public int floorCellDepth = 8;
    public int cellCols = 16;
    public int cellRows = 8;
    public int cellMin = 15;
    public int cellMax = 25;

    public int splotchNum = 3;
    public int splotchSize = 3;

    [Range(0f, 0.45f)]
    public float paddingWidthLeft;
    [Range(0f, 0.45f)]
    public float paddingWidthRight;
    [Range(0f, 0.45f)]
    public float paddingDepthBelow;
    [Range(0f, 0.45f)]
    public float paddingDepthAbove;


    // Grids
    private Cell[,] gridCells;

    private int[,] gridBasic;
    private int[,] gridBorders;
    private int[,] gridNbrs;
    private int[,] gridSplotches;
    public int[,] gridPOI;
    private Color[,] gridColors;
    public int[,] finalGrid;
    public Transform[,] gridFillObj;
    private Dictionary<Array, bool> grids;

    // Overall dimentions
    public int width { get; private set; }

    public int depth { get; private set; }

    private void Start()
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

    private void Update()
    {
        HandleInput();
    }

    private void ResetGrids()
    {
        if (!findPath)
        {
            InitGrids();
            CreateBasicGrid();
            CreateBordersGrid();
            CreateNbrsGrid();
            CreateSplotches();
            CombineGrids();
            MaterializeFloor();
        }

        else
        {
            // TODO Create Spawns
            pathFinder.GetComponent<PathFinder>().CreateSpawns(finalGrid, paddingWidthLeft, paddingWidthRight, paddingDepthBelow, paddingDepthAbove);
        }
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
        gridBasic = new int[width, depth];

        // Init borders grid
        gridBorders = new int[width, depth];

        // Init neighbours grid
        gridNbrs = new int[width, depth];

        // Init colors grid
        gridColors = new Color[floorCellWidth * cellCols, floorCellDepth * cellRows];

        // Init final grid
        finalGrid = new int[width, depth];

        // Init object grid
        gridFillObj = new Transform[width, depth];

        // Init splotch grid
        gridSplotches = new int[width, depth];

        // Init Points Of Interest grid
        gridPOI = new int[width, depth];
    }

    private void CreateBasicGrid()
    {
        if (!basicGrid)
        {
            return;
        }
        for (int countW = 0; countW < floorCellWidth; countW += 1)
        {
            for (int countD = 0; countD < floorCellDepth; countD += 1)
            {
                // Iterate within the cell
                float colorR = Random.Range(0f, 1f);
                float colorG = Random.Range(0f, 1f);
                float colorB = Random.Range(0f, 1f);

                Cell currentCell = gridCells[countW, countD];
                for (int cellD = 0; cellD < cellRows; cellD += 1)
                {
                    for (int cellW = 0; cellW < cellCols; cellW += 1)
                    {
                        //Debug.Log($"{countD}/{countW}: {cellD}/{cellW}");
                        int positionY = countD * cellRows + cellD;
                        int positionX = countW * cellCols + cellW;
                        gridBasic[positionX, positionY] = currentCell.Grid()[cellW, cellD];
                        gridColors[positionX, positionY] = new Color(colorR, colorG, colorB);
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

        for (int countW = 0; countW < gridBasic.GetLength(0); countW += 1)
        {
            gridBorders[countW, 0] = 1;
        }

        for (int countW = 0; countW < gridBasic.GetLength(0); countW += 1)
        {
            gridBorders[countW, gridBasic.GetLength(1) - 1] = 1;
        }

        for (int countD = 0; countD < gridBasic.GetLength(1); countD += 1)
        {
            gridBorders[0, countD] = 1;
        }

        for (int countD = 0; countD < gridBasic.GetLength(1); countD += 1)
        {
            gridBorders[gridBasic.GetLength(0) - 1, countD] = 1;
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
                int nbrCount = gridBasic[countW, countD - 1] +
                                  gridBasic[countW, countD + 1] +
                                  gridBasic[countW - 1, countD] +
                                  gridBasic[countW + 1, countD];

                if (nbrCount == 2)
                {
                    gridNbrs[countW, countD] = 1;
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
        for (int cd = 0; cd < depth; cd += 1)
        {
            for (int cw = 0; cw < width; cw += 1)
            {
                if (grid[cw, cd] == 1)
                {
                    if (additive)
                    {
                        finalGrid[cw, cd] = 1;
                    }
                    else
                    {
                        finalGrid[cw, cd] = 0;
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

        int splotchDev = (int)(depth / splotchNum);

        for (int count = 0; count < splotchNum; count += 1)
        {
            // choose X
            int rawD = Random.Range(0, width);

            // choose Y
            int rawW = previousColumn + Random.Range((int)(splotchDev * 2 / 3), (int)(splotchDev * 3 / 2));

            // normalize position
            int posD = rawD % (width - 2);
            int posW = rawW % (depth - 2);

            // get magnitude
            int splotchMag = (int)Random.Range(splotchSize / 2, splotchSize);

            PlaceSplotch(posD, posW, splotchMag);
            previousColumn = posW;
        }

        grids.Add(gridSplotches, false);
    }

    private void PlaceSplotch(int posW, int posD, int mag)
    {
        for (int countW = posW - mag; countW <= posW + mag; countW += 1)
        {
            for (int countD = posD - mag; countD <= posD + mag; countD += 1)
            {
                if ((countW > 1) && (countW < width - 2) && // withn width (double border excluded)
                    (countD > 1) && (countD < depth - 2) && // within depth (double border excluded)
                    (Math.Sqrt(((countW - posW) * (countW - posW)) + ((countD - posD) * (countD - posD))) <= mag)) // within mag of the coordinates
                {
                    gridSplotches[countW, countD] = 1;
                    // DEBUG ONLY
                    gridColors[countW, countD] = Color.red;
                    // mark the splotch place as special (to avoid collision with other artifacts)
                    gridPOI[countW, countD] = 1;
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

        int width = gridFillObj.GetLength(0);
        int depth = gridFillObj.GetLength(1);

        gridFillObj = new Transform[width, depth];
        for (int countD = 0; countD < depth; countD += 1)
        {
            for (int countW = 0; countW < width; countW += 1)
            {
                Transform obj;
                if (finalGrid[countW, countD] == 1)
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
                    gridFillObj[positionX, positionY] = Instantiate(obj, new Vector3(positionX, 0, positionY), Quaternion.identity);
                    gridFillObj[positionX, positionY].parent = tileHolder;
                    if (colorTiles)
                    {
                        gridFillObj[positionX, positionY].GetComponent<MeshRenderer>().material.color = gridColors[positionY, positionX];
                    }
                }
            }
        }
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

    public int GetAllAvailablePositions()
    {
        int counter = 0;
        for (int countW = 0; countW < finalGrid.GetLength(0); countW += 1)
        {
            for (int countD = 0; countD < finalGrid.GetLength(1); countD += 1)
            {
                if (finalGrid[countW, countD] == 1)
                {
                    counter += 1;
                }
            }
        }

        return counter;
    }
}