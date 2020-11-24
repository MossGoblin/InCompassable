using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Itr = ToolBox.Itr;

public class MapController : MonoBehaviour
{
    // Game Seed
    [Header("Random")]

    [SerializeField]
    private int seed;
    [SerializeField]
    private bool useSeed;
    // Prefabs
    [Header("Prefabs")]
    public Transform floor;
    public List<Transform> mapElements;
    private Dictionary<Vector3, Library.Patterns> massives;

    // Refs
    [Header("Refs")]
    public Transform floorHolder;
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
    public int nbrClumpThreshold;

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

    [Header("Biomes")]
    public int biomesCount;
    private int[,] gridBiomes;
    [SerializeField] Color[] biomeColors;

    // Primary Grids
    private Cell[,] gridCells; // used for density distrubution - // HERE USED
    private int[,] gridBase; // base grid - only empty and full // HERE USED
    private int[,] gridMassives; // positions of all special massives // HERE USED
    public int[,] finalGrid; // final grid with all basics and massives

    // Secondary grids
    private int[,] gridBorders;
    public Transform[,] gridElements; // Game Objects for all basics and massives
    private int[,] gridPOI; // Points of Interest - to be avoided when performing availability checks

    // 0 - do not change
    // 1 - remove
    // 2 - add
    private int[,] gridDiff; // HERE Questionable

    public void Start()
    {
        CheckRandom();
        SetUpMode();
        // Create the map
        CreateMap();
        // DBG player spawn plug
        Debug.Log("Placing players");
        PositionPlayers();
    }

    private void CheckRandom()
    {
        if (!useSeed)
        {
            seed = Random.Range(100000, 999999);
        }
        Random.InitState(seed);
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
        InitGrids();

        CreateBasicGrid();

        CreateNbrsGrid(); // Thicken the soup

        CreateBordersGrid();
        AddBordersToMassives();
        CreateSplotches();

        finalGrid = Grids.Copy(gridBase);
        MarkPatterns(); // HERE mark all massives
        finalGrid = CollapseGrids(finalGrid);

        MaterializeFloor();
        // // 6.
        // finalGrid = AddMassives(finalGrid);

        // // Mat.
        // HBD

        // // Biomes
        CreateBiomes();

        // // Color
        ColorizeGrid(gridBiomes);

    }

    private void AddBordersToMassives()
    {
        gridMassives = Grids.ApplyMaskIndex(gridMassives, gridBorders, (int)Library.Massives.Border, (int)Library.Massives.Border);
    }

    private void CreateBordersGrid()
    {
        gridBorders = Grids.CreateBordersGrid(width, depth, (int)Library.Massives.Border);
    }

    private void CreateNbrsGrid()
    {
        gridBase = Grids.AddNbrs(gridBase, nbrClumpThreshold);
    }


    private void CreateBasicGrid()
    {
        gridCells = CreateCellsGrid(floorCellWidth, floorCellDepth);
        gridBase = Grids.CreateBasicGrid(floorCellWidth, floorCellDepth, cellCols, cellRows, gridCells);
    }

    private Cell[,] CreateCellsGrid(int floorCellWidth, int floorCellDepth)
    {
        Cell[,] gridCells = new Cell[floorCellWidth, floorCellDepth];

        foreach ((int countW, int countD) in Itr.Iteration(floorCellWidth, floorCellDepth))
        {
            gridCells[countW, countD] = new Cell(cellCols, cellRows, cellMin, cellMax, (int)Library.Massives.Basic);
        }

        return gridCells;
    }

    private void InitGrids()
    {
        // cache dimentions
        this.width = floorCellWidth * cellCols;
        this.depth = floorCellDepth * cellRows;

        // Init cells grid
        gridCells = new Cell[floorCellWidth, floorCellDepth];

        // Init basic grid
        gridBase = new int[width, depth];

        // Init final grid
        finalGrid = new int[width, depth];

        // Init object grid
        gridElements = new Transform[width, depth];

        // Init difference grid
        gridDiff = new int[width, depth];
        gridDiff = FillGrid(gridDiff, 2); // fill in with 2's -- no change to original grid

        // POI grid
        gridPOI = new int[width, depth];

        // Biome identities grid
        gridBiomes = new int[width, depth];

        // Massives to be added to the map
        gridMassives = new int[width, depth];
        gridMassives = Grids.Init(gridMassives, 100);

        // used for creating borders
        gridBorders = new int[width, depth];

        massives = new Dictionary<Vector3, Library.Patterns>();

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
            int splotchMagnitude = Random.Range((int)(splotchSize / 2), splotchSize);
            PlaceSplotch(gridMassives, posW, posD, splotchMagnitude);
        }

        Debug.Log("Splotches ON");
    }

    private void MarkPatterns()
    {
        int[,] workingGrid = Grids.Copy(gridBase);
        workingGrid = CollapseGrids(finalGrid);
        workingGrid = Grids.Flatten(workingGrid, 0);

        Debug.Log($"Marking squares");

        int[,] pattern = Library.Square();

        List<(int, int)> patternPositions = PatternMapper.FindPattern(workingGrid, pattern);

        foreach ((int w, int d) position in patternPositions)
        {
            Debug.Log($"square: {position.w}/{position.d}");

            // HERE MarkArea
            this.gridMassives = Grids.MarkPattern(this.gridMassives, position.w, position.d, pattern, (int)Library.Massives.Square);
        }


        Debug.Log($"Marking arcs up");

        pattern = Library.DiagonalDown();

        patternPositions = PatternMapper.FindPattern(workingGrid, pattern);

        foreach ((int w, int d) position in patternPositions)
        {
            Debug.Log($"arc up: {position.w}/{position.d}");

            // HERE MarkArea
            this.gridMassives = Grids.MarkPattern(this.gridMassives, position.w, position.d, pattern, (int)Library.Massives.DiagonalDown);
        }
    }

    private int[,] CollapseGrids(int[,] targetGrid)
    {
        targetGrid = Grids.ApplyMask(targetGrid, gridMassives, 100);
        Debug.Log("Collapse grids");
        return targetGrid;
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


        // place floor everywhere
        foreach ((int countW, int countD) in Itr.Iteration(width, depth))
        {
            gridElements[countW, countD] = Instantiate(floor, new Vector3(countW, 0, countD), Quaternion.identity, floorHolder);

        }

        // Iterate finalGrid
        // if 0 - skip
        // if not -- place an object
        // HERE
        foreach ((int countW, int countD) in Itr.Iteration(width, depth))
        {
            Transform obj;

            if (finalGrid[countW, countD] == 0)
            {
                continue;
            }
            if (finalGrid[countW, countD] == 3)
            {
                Debug.Log($"obj at: {countW}/{countD} -- {finalGrid[countW, countD]}");
            }
            obj = mapElements[finalGrid[countW, countD]];

            int positionX = countW;
            int positionY = countD;
            Quaternion rotation = Quaternion.identity;
            Vector3 position = new Vector3(positionX, 0, positionY);

            position = new Vector3(positionX + 0.5f, 0, positionY + 0.5f);
            // obj.GetChild(0).transform.rotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
            gridElements[positionX, positionY] = Instantiate(obj, position, rotation);
            gridElements[positionX, positionY].parent = mapHolder;

        }
        Debug.Log("Materialize");
    }

    private void PlaceSplotch(int[,] grid, int posW, int posD, int mag)
    {
        Vector3 center = new Vector3(posW, 0, posD);
        Vector3 position = new Vector3(0, 0, 0);

        // iterate the vicinity of the splotch center
        foreach ((int countW, int countD) in Itr.IterationRange(posW - mag - splotchRimWidth, posW + mag + splotchRimWidth,
                                                                        posD - mag - splotchRimWidth, posD + mag + splotchRimWidth))
        {
            position = new Vector3(countW, 0, countD);
            // if it is on the rim
            if ((GetDistance(position, center) >= (float)mag) &&
            ((GetDistance(position, center) <= (float)mag + splotchRimWidth)))
            {
                if (gridBase[countW, countD] == 1) // if there is a block
                {
                    MarkPosition(gridMassives, countW, countD, (int)Library.Massives.RingRim); // mark splotch rim
                }
            }
            else if ((GetDistance(position, center) < mag)) // if it is inside the radius
            {
                MarkPosition(gridMassives, countW, countD, 0); // empty splotch radius - mark as 100 for 'to be avoided'
            }
        }

        MarkPosition(gridMassives, posW, posD, (int)Library.Massives.Obelisk); // mark splotch center
    }

    private void CreateBiomes()
    {
        List<Vector2> centroidList = new List<Vector2>();
        for (int count = 0; count < biomesCount; count++)
        {
            int posW = Random.Range(0, width);
            int posD = Random.Range(0, depth);
            centroidList.Add(new Vector2(posW, posD));
        };

        gridBiomes = Voronoi.GenerateRegions(width, depth, centroidList);
        Debug.Log($"Biomes: {centroidList.Count}");
    }

    private void ColorizeGrid(int[,] grid)
    {
        // DBG only while WIP
        // Colorize POI

        // Get library and palette
        Palette palette = FindObjectOfType<Palette>();

        // Iterate elements grid

        foreach ((int countW, int countD) in Itr.Iteration(width, depth))
        {
            Transform obj = gridElements[countW, countD];
            if (obj == null)
            {
                continue;
            }
            // gridElements[countW, countD].GetChild(0).GetComponent<MeshRenderer>().material.color = palette.palette[grid[countW, countD]];
            gridElements[countW, countD].GetChild(0).GetComponent<MeshRenderer>().material.color = biomeColors[grid[countW, countD]];
        }
        Debug.Log("Colorize");
    }

    private int[,] OverlayGrids(int[,] baseGrid, int[,] diffGrid)
    {

        foreach ((int countW, int countD) in Itr.Iteration(width, depth))
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



    /// <summary>
    /// Mark position of a cell for processing
    /// </summary>
    /// <remarks>
    /// posW, posD, object type, empty/block, poi 
    /// </remarks>
    private void MarkPosition(int[,] grid, int posW, int posD, int index)
    {
        grid[posW, posD] = index;
    }

    private int[,] FillGrid(int[,] origin, int value)
    {
        int[,] target = new int[origin.GetLength(0), origin.GetLength(1)];

        foreach ((int countA, int countB) in Itr.Iteration(origin.GetLength(0), origin.GetLength(1)))
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
