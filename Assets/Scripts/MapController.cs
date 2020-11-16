using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using ToolBox;

public class MapController : MonoBehaviour
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
    public int splotchRimWidth = 2;

    // Players
    [Header("Players")]
    public Transform playerOne;
    public Transform playerTwo;
    public bool visionRange;
    public float visibilityDistance;
    [Range(0.1f, 0.45f)]
    public float maxPointDeviation = 0.2f;

    [Header("Checks")]
    public int mode;

    // Grids
    private Cell[,] gridCells;
    private int[,] gridBase;
    private int[,] gridTypes;
    public int[,] finalGrid;
    public Transform[,] gridElements;
    private int[,] gridPOI;

    // 0 - do not change
    // 1 - remove
    // 2 - add
    private int[,] gridDiff;

    public void Start()
    {
        SetUpMode();
        // Create the map
        CreateMap();
        // DBG player spawn plug
        PositionPlayers();
    }

    public void Update()
    {
        ClearOutOfRangeObstacles();
        HandleInput();
    }
    private void HandleInput()
    {
        // HERE INPUT
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     CreateNbrsGrid();
        //     CollapseGrids();
        //     ColorizeGrid();
        // }
    }
    public void SetUpMode()
    {
        switch (mode)
        {
            case 1:
                floorCellWidth = 3;
                floorCellDepth = 3;
                cellCols = 2;
                cellRows = 2;
                cellMin = 1;
                cellMax = 3;
                break;
            case 2:
                floorCellWidth = 4;
                floorCellDepth = 8;
                cellCols = 8;
                cellRows = 4;
                cellMin = 10;
                cellMax = 15;
                break;
            default:
                floorCellWidth = 8;
                floorCellDepth = 8;
                cellCols = 16;
                cellRows = 8;
                cellMin = 15;
                cellMax = 25;
                break;
        }
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

        // HERE Grid Init
        InitGrids();

        // 1.
        CreateBasicGrid();

        // 5.
        CreateBordersGrid();

        // 2.
        //CreateNbrsGrid();

        // 4.
        CollapseGrids();
        MarkPatterns();
        ClumpSquares(); // HERE Clump squares

        // 3.
        CreateSplotches();

        // 6.
        CollapseGrids();

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

        // Init cells grid
        gridCells = new Cell[floorCellWidth, floorCellDepth];

        foreach ((int countW, int countD) in Iterator.Iteration(floorCellWidth, floorCellDepth))
        {
            gridCells[countW, countD] = new Cell(cellCols, cellRows, cellMin, cellMax);
        }

        // Init basic grid
        gridBase = new int[width, depth];

        // Init final grid
        finalGrid = new int[width, depth];

        // Init object grid
        gridElements = new Transform[width, depth];

        // Init difference grid
        gridDiff = new int[width, depth];
        gridDiff = FillGrid(gridDiff, 2); // fill in with 2's -- no change to original grid

        // Init Points Of Interest grid
        gridTypes = new int[width, depth];

        // POI grid
        gridPOI = new int[width, depth];

    }
    private void CreateBasicGrid()
    {

        foreach ((int countW, int countD) in Iterator.Iteration(floorCellWidth, floorCellDepth))
        {
            Cell currentCell = gridCells[countW, countD];

            foreach ((int cellW, int cellD) in Iterator.Iteration(cellCols, cellRows))
            {
                int posW = countW * cellCols + cellW;
                int posD = countD * cellRows + cellD;
                int value = currentCell.Grid()[cellW, cellD];
                gridBase[posW, posD] = value;
                MarkPosition(posW, posD, value, value, false);
            }
        }

        Debug.Log("Basic ON");
    }

    private void CreateNbrsGrid()
    {

        foreach ((int countW, int countD) in Iterator.Iteration(width, depth))
        {
            if (gridBase[countW, countD] == 0)
            {
                int nbrCount = NbrCount(gridBase, gridPOI, countW, countD);

                if (nbrCount == 2)
                {
                    MarkPosition(countW, countD, 4, 1, true);
                    // MarkPosition(countW, countD, 1, 1, true);
                }
            }
        }

        // for (int countD = 1; countD < depth - 1; countD += 1)
        // {
        //     for (int countW = 1; countW < width - 1; countW += 1)
        //     {
        //         if (gridBase[countW, countD] == 0)
        //         {
        //             int nbrCount = NbrCount(gridBase, gridPOI, countW, countD);

        //             if (nbrCount == 2)
        //             {
        //                 MarkPosition(countW, countD, 4, 1, true);
        //                 // MarkPosition(countW, countD, 1, 1, true);
        //             }
        //         }
        //     }
        // }
        Debug.Log("Nbrs ON");
    }

    private void MarkPatterns()
    {
        CollapseGrids(); // HERE expand collapse
    }
    private void CreateSplotches()
    {

        // new position picking
        // input - number of splotches; width and depth of map

        int averageDistanceW = (int)(width / (splotchNum + 1)); // X splotches => x+1 spreads around them
        // width - avoid border

        for (int count = 1; count <= splotchNum; count++)
        {
            int posW = count * averageDistanceW + Random.Range(-splotchSize, splotchSize); // A step ahead, random deviation the size of the magnitude
            int posD = Random.Range(2 + splotchSize, depth - 2 - splotchSize);
            // DBG Splotch Size
            // int splotchMagnitude = splotchSize;
            int splotchMagnitude = Random.Range((int)(splotchSize / 2), splotchSize);
            PlaceSplotch(posW, posD, splotchMagnitude);
        }

        Debug.Log("Splotches ON");
    }

    private void ClumpSquares()
    {
        int[,] workingGrid = CopyGrid(finalGrid);

        // create pattern for squares
        Debug.Log("Clumping squares...");
        int[,] squarePattern = new int[,]
        {
            {1, 1},
            {1, 1}
        };

        List<(int, int)> squarePositions = PatternMapper.FindPattern(workingGrid, squarePattern);

        Debug.Log($"Squares: {squarePositions.Count}");
        foreach ((int w, int d) square in squarePositions)
        {
            Debug.Log($"sq: {square.w}/{square.d}");

            MarkArea(square.w, square.d, squarePattern, 4, 0, 1, 2, true, false);
            // foreach ((int countW, int countD) in Iterator.Iteration(squarePattern.GetLength(0), squarePattern.GetLength(0)))
            // {
            //     MarkPosition(new Vector3(square.w + countW, 0, square.d + countD), 4, 2, true);
            // }
        }

        // DBG Duplicate for debugging
        // create pattern for squares
        // int[,] arcPatternOne = new int[,]
        // {
        //     {0, 0, 0},
        //     {0, 1, 0},
        //     {0, 0, 0},
        //     {0, 1, 0},
        //     {0, 0, 0}
        // };

        // List<(int, int)> arcPositionsOne = PatternMapper.FindPattern(workingGrid, arcPatternOne);

        // Debug.Log($"Arcs One: {arcPositionsOne.Count}");
        // foreach ((int w, int d) square in arcPositionsOne)
        // {
        //     // Debug.Log($"ar: {square.w}/{square.d}");

        //     for (int countW = 0; countW < arcPatternOne.GetLength(0); countW++)
        //     {
        //         for (int countD = 0; countD < arcPatternOne.GetLength(1); countD++)
        //         {
        //             MarkPosition(new Vector3(square.w + countW, 0, square.d + countD), 5, 2, true);
        //         }
        //     }
        // }

        // int[,] arcPatternTwo = new int[,]
        // {
        //     {0, 0, 0, 0, 0},
        //     {0, 1, 0, 1, 0},
        //     {0, 0, 0, 0, 0}
        // };

        // List<(int, int)> arcPositionsTwo = PatternMapper.FindPattern(workingGrid, arcPatternTwo);

        // Debug.Log($"Arcs Two: {arcPositionsOne.Count}");
        // foreach ((int w, int d) square in arcPositionsTwo)
        // {
        //     // Debug.Log($"ar: {square.w}/{square.d}");

        //     for (int countW = 0; countW < arcPatternTwo.GetLength(0); countW++)
        //     {
        //         for (int countD = 0; countD < arcPatternTwo.GetLength(1); countD++)
        //         {
        //             MarkPosition(new Vector3(square.w + countW, 0, square.d + countD), 5, 2, true);
        //         }
        //     }
        // }


        // ColorizeGrid();
    }

    private void MarkArea(int posW, int posD, int[,] pattern, int typeOne, int typeTwo, int objOne, int objTwo, bool poiOne, bool poiTwo)
    {
        foreach ((int countW, int countD) in Iterator.Iteration(pattern.GetLength(0), pattern.GetLength(1)))
        {
            if (pattern[countW, countD] == 1)
            {
                MarkPosition(posW + countW, posD + countD, typeOne, objOne, poiOne);
            }
            else
            {
                MarkPosition(posW + countW, posD + countD, typeTwo, objTwo, poiTwo);
            }
        }
    }


    private void CollapseGrids()
    {
        finalGrid = CopyGrid(gridBase);
        finalGrid = OverlayGrids(finalGrid, gridDiff);
    }

    private void CreateBordersGrid()
    {
        int podW = 0;
        int posD = 0;
        for (int countW = 0; countW < gridBase.GetLength(0); countW += 1)
        {
            podW = countW;
            posD = 0;
            MarkPosition(podW, posD, 1, 1, false);
        }

        for (int countW = 0; countW < gridBase.GetLength(0); countW += 1)
        {
            podW = countW;
            posD = gridBase.GetLength(1) - 1;
            MarkPosition(podW, posD, 1, 1, false);
        }

        for (int countD = 0; countD < gridBase.GetLength(1); countD += 1)
        {
            podW = 0;
            posD = countD;
            MarkPosition(podW, posD, 1, 1, false);
        }

        for (int countD = 0; countD < gridBase.GetLength(1); countD += 1)
        {
            podW = gridBase.GetLength(0) - 1;
            posD = countD;
            MarkPosition(podW, posD, 1, 1, false);
        }

        Debug.Log("Borders ON");
    }
    private void MaterializeFloor()
    {
        // clear all previously created objects
        foreach (Transform obj in mapHolder)
        {
            Destroy(obj.gameObject);
        }

        int width = gridElements.GetLength(0);
        int depth = gridElements.GetLength(1);

        gridElements = new Transform[width, depth];

        foreach ((int countW, int countD) in Iterator.Iteration(width, depth))
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

    private void PlaceSplotch(int posW, int posD, int mag)
    {
        Vector3 center = new Vector3(posW, 0, posD);
        Vector3 position = new Vector3(0, 0, 0);
        // iterate the vicinity of the splotch center

        foreach ((int countW, int countD) in Iterator.IterationRange(posW - mag - splotchRimWidth, posW + mag + splotchRimWidth,
                                                                        posD - mag - splotchRimWidth, posD + mag + splotchRimWidth))
        {
            position = new Vector3(countW, 0, countD);
            // if it is on the rim
            if ((GetDistance(position, center) >= (float)mag) &&
            ((GetDistance(position, center) <= (float)mag + splotchRimWidth)))
            {
                if (gridBase[countW, countD] == 1) // if there is a block
                {
                    MarkPosition(countW, countD, 3, 1, true); // mark splotch rim
                }
            }
            else if ((GetDistance(position, center) < mag)) // if it is inside the radius
            {
                MarkPosition(countW, countD, 0, 0, false); // empty splotch radius
            }
        }

        MarkPosition(posW, posD, 2, 1, true); // mark splotch center
    }

    private void ColorizeGrid()
    {
        // DBG only while WIP
        // Colorize POI

        // Get library and palette
        Palette palette = FindObjectOfType<Palette>();

        // Iterate elements grid

        foreach ((int countW, int countD) in Iterator.Iteration(width, depth))
        {
            gridElements[countW, countD].GetChild(0).GetComponent<MeshRenderer>().material.color = palette.palette[gridTypes[countW, countD]];
        }
    }

    private int[,] OverlayGrids(int[,] baseGrid, int[,] diffGrid)
    {

        foreach ((int countW, int countD) in Iterator.Iteration(width, depth))
        {
            if (diffGrid[countW, countD] != 2)
            {
                baseGrid[countW, countD] = diffGrid[countW, countD];
            }
        }

        return baseGrid;
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
                float distanceToOne = GetDistance(obj, playerOne);
                float distanceToTwo = GetDistance(obj, playerTwo);

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

    private float GetDistance(Transform objectOne, Transform objectTwo)
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

    private float GetDistance(Vector3 positionOne, Vector3 positionTwo)
    {
        float distance;

        float sqrtTwo = Mathf.Sqrt(2f);

        float distX = Mathf.Abs(positionTwo.x - positionOne.x);
        float distZ = Mathf.Abs(positionTwo.z - positionOne.z);

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

    private int[,] CopyGrid(int[,] origin)
    {
        int[,] target = new int[origin.GetLength(0), origin.GetLength(1)];

        foreach ((int countA, int countB) in Iterator.Iteration(origin.GetLength(0), origin.GetLength(1)))
        {
            target[countA, countB] = origin[countA, countB];
        }

        return target;
    }

    private void MarkPosition(Vector3 position, int type, int obj, bool poi)
    {
        int posW = (int)position.x;
        int posD = (int)position.z;

        gridTypes[posW, posD] = type;
        gridPOI[posW, posD] = 1;
        gridDiff[posW, posD] = obj;
    }

    /// <summary>
    /// Mark position of a cell for processing
    /// </summary>
    /// <remarks>
    /// posW, posD, object type, empty/block, poi 
    /// </remarks>
    private void MarkPosition(int posW, int posD, int type, int obj, bool poi)
    {
        gridTypes[posW, posD] = type;
        gridPOI[posW, posD] = poi ? 1 : 0;
        gridDiff[posW, posD] = obj;
    }

    private int[,] FillGrid(int[,] origin, int value)
    {
        int[,] target = new int[origin.GetLength(0), origin.GetLength(1)];

        foreach ((int countA, int countB) in Iterator.Iteration(origin.GetLength(0), origin.GetLength(1)))
        {
            target[countA, countB] = value;
        }

        return target;
    }

    private int NbrCount(int[,] grid, int posW, int posD)
    {
        int nbrs = 0;
        if (grid[posW - 1, posD] == 1)
        {
            nbrs++;
        }
        if (grid[posW, posD + 1] == 1)
        {
            nbrs++;
        }
        if (grid[posW + 1, posD] == 1)
        {
            nbrs++;
        }
        if (grid[posW, posD - 1] == 1)
        {
            nbrs++;
        }

        return nbrs;
    }

    private int NbrCount(int[,] grid, int[,] poiGrid, int posW, int posD)
    {
        int nbrs = 0;
        if ((grid[posW - 1, posD] == 1) && (poiGrid[posW - 1, posD] == 0))
        {
            nbrs++;
        }
        if ((grid[posW, posD + 1] == 1) && (poiGrid[posW, posD + 1] == 0))
        {
            nbrs++;
        }
        if ((grid[posW + 1, posD] == 1) && (poiGrid[posW + 1, posD] == 0))
        {
            nbrs++;
        }
        if ((grid[posW, posD - 1] == 1) && (poiGrid[posW, posD - 1] == 0))
        {
            nbrs++;
        }

        return nbrs;
    }
}
