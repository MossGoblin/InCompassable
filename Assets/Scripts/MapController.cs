using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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

    // Refs
    [Header("Refs")]
    public Transform floorHolder;
    public Transform mapHolder;
    public Transform pathFinder;
    public Transform libraryTransform;
    private ElementsLibrary library;

    [SerializeField] private Transform hudController;

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
    public Transform[] players;
    public bool visionRange;
    public float visibilityDistance;
    [Range(0.1f, 0.45f)]
    public float maxPointDeviation = 0.2f;


    [Header("Canvas")]
    [SerializeField] private RenderTexture leftScreen;
    [SerializeField] private RenderTexture rightScreen;

    [Header("Checks")]
    public int mode;

    [Header("Biomes")]
    public int biomesCount;
    private int[,] gridBiomes;
    [SerializeField] Color[] biomeColors;


    [Header("Patterns")]
    [SerializeField] bool[] patternChecks;

    // Primary Grids
    private Cell[,] gridCells;
    private int[,] gridBase;
    private int[,] gridMassives;
    public int[,] gridAngles;
    private bool[,] gridLock;
    public int[,] finalGrid;

    // Secondary grids
    private int[,] gridBorders;
    public Transform[,] gridFloor; // Game Objects for all floor tiles
    public Transform[,] gridElements; // Game Objects for all basics and massives
    public List<POI> globalPingList;

    public void Start()
    {
        Init();
        CheckRandom();
        SetUpMode();
        // Create the map
        CreateMap();
        // DBG player spawn plug
        Debug.Log("Placing players");
        PositionPlayers();
    }
    public void Update()
    {
        // ClearOutOfRangeObstacles();
    }

    private void Init()
    {
        library = libraryTransform.GetComponent<ElementsLibrary>();
        globalPingList = new List<POI>();
    }

    private void CheckRandom()
    {
        if (!useSeed)
        {
            seed = Random.Range(100000, 999999);
        }
        Random.InitState(seed);
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

        CreateBasicGrid(); // gridBase - generate

        int[,] workingGrid = Grids.Copy(gridBase);
        workingGrid = CreateNbrsGrid(workingGrid); // add nbrs

        (workingGrid, gridLock) = CreateBordersGrid(workingGrid, gridLock); // add borders

        (workingGrid, gridLock) = CreateSplotches(workingGrid, gridLock); // add splotches

        (workingGrid, gridLock, gridAngles) = MarkPatterns(workingGrid, gridLock, gridAngles); // mark all patterns

        finalGrid = new int[width, depth];
        finalGrid = Grids.Copy(workingGrid);

        MaterializeFloor(finalGrid);

        // Biomes
        CreateBiomes();

        // Color
        ColorizeGrid(gridBiomes);

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

        // Biome identities grid
        gridBiomes = new int[width, depth];

        // Massives to be added to the map
        gridMassives = new int[width, depth];
        // Angles for each massive - only the origin is marked; teh value is the angle
        gridAngles = new int[width, depth];

        gridLock = new bool[width, depth];  // locks positions for use  to prevent overlapping of massives -- true = used

        // used for creating borders
        gridBorders = new int[width, depth];

        // Init final grid
        finalGrid = new int[width, depth];

        // Init object grid
        gridElements = new Transform[width, depth];
    }
    private void CreateBasicGrid()
    {
        gridCells = CreateCellsGrid(floorCellWidth, floorCellDepth);
        gridBase = Grids.CreateBasicGrid(floorCellWidth, floorCellDepth, cellCols, cellRows, gridCells);
    }


    private int[,] CreateNbrsGrid(int[,] grid)
    {
        int[,] gridResult = Grids.AddNbrs(grid, nbrClumpThreshold);

        return gridResult;
    }

    private (int[,], bool[,]) CreateBordersGrid(int[,] grid, bool[,] gridLock)
    {
        (grid, gridLock) = Grids.CreateBordersGrid(grid, gridLock, (int)ElementsLibrary.Elements.Border);

        return (grid, gridLock);
    }

    private void AddBordersToMassives()
    {
        gridMassives = Grids.ApplyMaskIndex(gridMassives, gridLock, gridBorders, (int)ElementsLibrary.Elements.Border, (int)ElementsLibrary.Elements.Border);
    }

    private (int[,], bool[,]) CreateSplotches(int[,] grid, bool[,] gridLock)
    {

        // new position picking
        // input - number of splotches; width and depth of map

        int averageDistanceW = (int)(width / (splotchNum + 1)); // X splotches => x+1 spreads around them
        // width - avoid border

        bool goodSpot = false;
        while (!goodSpot)
        {
            for (int count = 1; count <= splotchNum; count++)
            {
                // select a position that will guarantee that the splotch will not overlap another massive
                int posW = count * averageDistanceW + Random.Range(-splotchSize, splotchSize); // A step ahead, random deviation the size of the magnitude
                int posD = Random.Range(2 + splotchSize, depth - 2 - splotchSize);
                int splotchMagnitude = Random.Range((int)(splotchSize / 2), splotchSize);
                // overlap check
                goodSpot = CheckForOverlaps(gridLock, posW, posD, splotchMagnitude);
                if (goodSpot)
                {
                    (grid, gridLock) = PlaceSplotch(grid, posW, posD, splotchMagnitude);
                }
            }
        }

        Debug.Log("Splotches ON");

        return (grid, gridLock);
    }

    private bool CheckForOverlaps(bool[,] grid, int posW, int posD, int mag)
    {
        bool result = true;
        Vector3 center = new Vector3(posW, 0, posD);
        Vector3 position = new Vector3(0, 0, 0);

        // iterate the vicinity of the splotch center
        foreach ((int countW, int countD) in TB.IterationRange(posW - mag - splotchRimWidth, posW + mag + splotchRimWidth,
                                                                        posD - mag - splotchRimWidth, posD + mag + splotchRimWidth))
        {
            if ((TB.GetDistance(position, center) <= mag)) // if it is inside the radius
            {
                if (!grid[countW, countD]) // if the position is not 0, then there is already a massive index there; so the whole checked space is unavailable
                {
                    result = false;
                    break;
                }
            }
            if (!result)
            {
                break;
            }
        }

        return result;
    }

    private (int[,], bool[,]) PlaceSplotch(int[,] grid, int posW, int posD, int mag)
    {
        Vector3 center = new Vector3(posW, 0, posD);
        Vector3 position = new Vector3(0, 0, 0);

        // iterate the vicinity of the splotch center
        foreach ((int countW, int countD) in TB.IterationRange(posW - mag - splotchRimWidth, posW + mag + splotchRimWidth,
                                                                        posD - mag - splotchRimWidth, posD + mag + splotchRimWidth))
        {
            position = new Vector3(countW, 0, countD);
            // if it is on the rim
            if ((TB.GetDistance(position, center) >= (float)mag) &&
            ((TB.GetDistance(position, center) <= (float)mag + splotchRimWidth)))
            {
                if (grid[countW, countD] == 1) // if there is a block
                {
                    grid[countW, countD] = (int)ElementsLibrary.Elements.RingRim;
                    gridLock[countW, countD] = true;

                }
            }
            else if ((TB.GetDistance(position, center) < mag)) // if it is inside the radius
            {
                grid[countW, countD] = 0;
                gridLock[countW, countD] = true;
            }
        }

        grid[posW, posD] = (int)ElementsLibrary.Elements.Obelisk;

        return (grid, gridLock);
    }

    private (int[,], bool[,], int[,]) MarkPatterns(int[,] grid, bool[,] gridLock, int[,] gridAngles)
    {
        int[,] pattern;

        int[] indexOrder = new int[] { 7, 6, 5, 4, 3 };
        for (int count = 0; count < indexOrder.Length; count++)
        // foreach (int index in Enum.GetValues(typeof(Library.Elements)))
        {
            int index = indexOrder[count];
            if (index < 3 || !patternChecks[index - 3] || !library.elementPool[index].hasPattern)
            {
                continue;
            }

            int[] angles = ElementsLibrary.GetAngles(index);
            // there are more than 1 angles for this pattern
            foreach (int angle in angles)
            {
                pattern = ElementsLibrary.GetPattern(index);
                pattern = RotatePattern(pattern, angle);
                List<(int, int)> patternPositions = PatternMapper.FindPattern(ref grid, ref gridLock, pattern);
                Debug.Log($"Found elements: {index} / {patternPositions.Count}");
                foreach ((int width, int depth) position in patternPositions)
                {
                    // Debug.Log($"ptrn {index}: {position.width}/{position.depth}");
                    grid[position.width, position.depth] = index;
                    gridAngles[position.width, position.depth] = angle;
                }
            }
        }

        return (grid, gridLock, gridAngles);
    }

    private int[,] RotatePattern(int[,] pattern, int angle)
    {
        (int width, int depth) = Grids.Dim(pattern);
        int[,] rotatedPattern = Grids.Blank(pattern);

        foreach ((int countW, int countD) in TB.Iteration(width, depth))
        {
            rotatedPattern[countD, width - countW - 1] = pattern[countW, countD];
        }

        return rotatedPattern;
    }

    private Cell[,] CreateCellsGrid(int floorCellWidth, int floorCellDepth)
    {
        Cell[,] gridCells = new Cell[floorCellWidth, floorCellDepth];

        foreach ((int countW, int countD) in TB.Iteration(floorCellWidth, floorCellDepth))
        {
            gridCells[countW, countD] = new Cell(cellCols, cellRows, cellMin, cellMax, (int)ElementsLibrary.Elements.Basic);
        }

        return gridCells;
    }

    private (int[,], bool[,]) OverlayGrids(int[,] overlayGrid, bool[,] gridLock)
    {
        int[,] targetGrid = new int[overlayGrid.GetLength(0), overlayGrid.GetLength(1)];
        Grids.ApplyMask(targetGrid, overlayGrid, gridLock); // apply mask to base only where the gridLock has been marked
        Debug.Log("Collapse grids");
        return (targetGrid, gridLock);
    }


    private void MaterializeFloor(int[,] grid)
    {
        // clear all previously created objects
        foreach (Transform obj in mapHolder)
        {
            Destroy(obj.gameObject);
        }

        (int width, int depth) = Grids.Dim(finalGrid);

        // gridFloor = new Transform[width, depth]; // OBS

        // place floor everywhere
        foreach ((int countW, int countD) in TB.Iteration(width, depth))
        {
            int biomeIndex = gridBiomes[countW, countD];
            Transform floor = library.GetElement(biomeIndex, 0);
            gridElements[countW, countD] = Instantiate(floor, new Vector3(countW, 0, countD), Quaternion.identity, floorHolder);

        }

        // Iterate finalGrid
        // if 0 - skip
        // if not -- place an object

        // DBG dev check total number of objects
        int totalObjects = 0;
        foreach ((int countW, int countD) in TB.Iteration(width, depth))
        {
            Transform obj;

            if (grid[countW, countD] == 0)
            {
                continue;
            }

            int biomeIndex = gridBiomes[countW, countD];
            int elementIndex = grid[countW, countD];
            obj = library.GetElement(biomeIndex, elementIndex);


            int positionX = countW;
            int positionY = countD;
            Quaternion rotation = Quaternion.identity;
            Vector3 position = new Vector3(positionX, 0, positionY);

            // get object offset from object
            gridElements[positionX, positionY] = Instantiate(obj, position, rotation);
            gridElements[positionX, positionY].parent = mapHolder;

            // HERE create a global PING list
            // elements - holds the position of the massive, its visibility and its icon
            // so... a POI object
            ElementsLibrary.VisibilityRanges visibilityRange = library.elementPool[elementIndex].visibilityRange;
            if ((int)visibilityRange != (int)ElementsLibrary.VisibilityRanges.Never)
            {
                RectTransform icon = library.GetIcon(elementIndex);
                POI newPOI = new POI(new Vector3(countW, 0, countD), icon, visibilityRange);
                globalPingList.Add(newPOI);
            }

            totalObjects++;
        }
        hudController.GetComponent<HudController>().Init(globalPingList); // send pingList to hudController
        Debug.Log($"pingList : {globalPingList.Count} / {totalObjects}");
        Debug.Log("Materialize");
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

        foreach ((int countW, int countD) in TB.Iteration(width, depth))
        {
            Transform obj = gridElements[countW, countD];
            if (obj == null)
            {
                continue;
            }
            gridElements[countW, countD].GetChild(0).GetComponent<MeshRenderer>().material.color = biomeColors[grid[countW, countD]];
        }
        Debug.Log("Colorize");
    }

    private void PositionPlayers()
    {
        // Get Players
        int[,] floodGrid = Grids.Flatten(finalGrid, 0);
        players = pathFinder.GetComponent<PathFinder>().CreateSpawns(floodGrid, maxPointDeviation);

        // Set up projectin screens
        players[0].GetComponentInChildren<Camera>().targetTexture = leftScreen;
        players[1].GetComponentInChildren<Camera>().targetTexture = rightScreen;

        // Set up input // TODO Do it smarter
        players[0].GetComponent<Player>().chirality = 0;
        players[1].GetComponent<Player>().chirality = 1;

        // Send the players to the HUD controller
        hudController.GetComponent<HudController>().SetPlayers(players); // set up players
        hudController.GetComponent<HudController>().StartUp();
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

    private int[,] FillGrid(int[,] origin, int value)
    {
        int[,] target = new int[origin.GetLength(0), origin.GetLength(1)];

        foreach ((int countA, int countB) in TB.Iteration(origin.GetLength(0), origin.GetLength(1)))
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
