using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapGen : MonoBehaviour
{
    [System.Serializable]
    public class Cell
    {
        public bool visited = false;
        public GameObject northWall, southWall, eastWall, westWall;
    }

    public GameObject monster;
    public Transform player;

    [Header("Maze Settings")]
    public int width = 10;
    public int height = 10;
    public GameObject cellPrefab;
    public float cellSize = 4f;

    [Header("Openness Settings")]
    [Range(0f, 1f)]
    public float extraWallRemovalChance = 0.0f;

    [Header("Hiding Spots")]
    public GameObject hidingSpotPrefab;
    public int hidingSpotsToSpawn = 10;

    private Cell[,] cells;
    private Stack<Vector2Int> stack = new Stack<Vector2Int>();

    private NavMeshData navMeshData;
    private NavMeshDataInstance navMeshDataInstance;

    void Start()
    {
        GenerateGrid();
        StartCoroutine(GenerateMaze());
    }

    void GenerateGrid()
    {
        cells = new Cell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x * cellSize, 0, y * cellSize);
                GameObject newCell = Instantiate(cellPrefab, pos, Quaternion.identity, transform);

                Cell cell = new Cell();
                cell.northWall = newCell.transform.Find("North")?.gameObject;
                cell.southWall = newCell.transform.Find("South")?.gameObject;
                cell.eastWall = newCell.transform.Find("East")?.gameObject;
                cell.westWall = newCell.transform.Find("West")?.gameObject;

                cells[x, y] = cell;
            }
        }
    }

    IEnumerator GenerateMaze()
    {
        Vector2Int startPos = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        stack.Push(startPos);
        cells[startPos.x, startPos.y].visited = true;

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count > 0)
            {
                Vector2Int chosen = neighbors[Random.Range(0, neighbors.Count)];
                RemoveWall(current, chosen);
                cells[chosen.x, chosen.y].visited = true;
                stack.Push(chosen);
            }
            else
            {
                stack.Pop();
            }

            yield return null;
        }

        if (extraWallRemovalChance > 0f)
        {
            RemoveRandomWalls(extraWallRemovalChance);
        }

        SpawnHidingSpots();

        // Wait a frame to make sure all walls are updated/destroyed
        yield return null;

        // Bake NavMesh at runtime using NavMeshBuilder
        BakeNavMesh();

        SpawnMonster();

        yield break;
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> list = new List<Vector2Int>();

        Vector2Int[] directions = {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0)
        };

        foreach (var dir in directions)
        {
            Vector2Int neighbor = cell + dir;
            if (neighbor.x >= 0 && neighbor.x < width && neighbor.y >= 0 && neighbor.y < height)
            {
                if (!cells[neighbor.x, neighbor.y].visited)
                {
                    list.Add(neighbor);
                }
            }
        }
        return list;
    }

    void RemoveWall(Vector2Int a, Vector2Int b)
    {
        Vector2Int direction = b - a;

        if (direction == new Vector2Int(0, 1))
        {
            if (cells[a.x, a.y].northWall != null) Destroy(cells[a.x, a.y].northWall);
            if (cells[b.x, b.y].southWall != null) Destroy(cells[b.x, b.y].southWall);
        }
        else if (direction == new Vector2Int(0, -1))
        {
            if (cells[a.x, a.y].southWall != null) Destroy(cells[a.x, a.y].southWall);
            if (cells[b.x, b.y].northWall != null) Destroy(cells[b.x, b.y].northWall);
        }
        else if (direction == new Vector2Int(1, 0))
        {
            if (cells[a.x, a.y].eastWall != null) Destroy(cells[a.x, a.y].eastWall);
            if (cells[b.x, b.y].westWall != null) Destroy(cells[b.x, b.y].westWall);
        }
        else if (direction == new Vector2Int(-1, 0))
        {
            if (cells[a.x, a.y].westWall != null) Destroy(cells[a.x, a.y].westWall);
            if (cells[b.x, b.y].eastWall != null) Destroy(cells[b.x, b.y].eastWall);
        }
    }

    void RemoveRandomWalls(float chance)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x < width - 1 && cells[x, y].eastWall != null && Random.value < chance)
                {
                    Destroy(cells[x, y].eastWall);
                    if (cells[x + 1, y].westWall != null)
                        Destroy(cells[x + 1, y].westWall);
                }

                if (y < height - 1 && cells[x, y].northWall != null && Random.value < chance)
                {
                    Destroy(cells[x, y].northWall);
                    if (cells[x, y + 1].southWall != null)
                        Destroy(cells[x, y + 1].southWall);
                }
            }
        }
    }

    void SpawnHidingSpots()
    {
        HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();
        int spawnedCount = 0;
        int attempts = 0;
        int maxAttempts = hidingSpotsToSpawn * 10;

        while (spawnedCount < hidingSpotsToSpawn && attempts < maxAttempts)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            Vector2Int cellPos = new Vector2Int(x, y);

            if (occupiedCells.Contains(cellPos))
            {
                attempts++;
                continue;
            }

            Vector3 spawnPos = new Vector3(x * cellSize, 1, y * cellSize);
            Instantiate(hidingSpotPrefab, spawnPos, Quaternion.identity, transform);

            occupiedCells.Add(cellPos);
            spawnedCount++;
            attempts++;
        }
    }

    void BakeNavMesh()
    {
        // Remove old navmesh instance if exists
        if (navMeshDataInstance.valid)
        {
            navMeshDataInstance.Remove();
        }

        // Collect NavMesh build sources from all child MeshFilters
        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        MeshFilter[] meshFilters = transform.GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.gameObject.activeInHierarchy)
            {
                NavMeshBuildSource src = new NavMeshBuildSource();
                src.shape = NavMeshBuildSourceShape.Mesh;
                src.sourceObject = mf.sharedMesh;
                src.transform = mf.transform.localToWorldMatrix;
                src.area = 0; // Default walkable area
                sources.Add(src);
            }
        }

        Vector3 center = new Vector3((width * cellSize) / 2f - cellSize / 2f, 0, (height * cellSize) / 2f - cellSize / 2f);
        Vector3 size = new Vector3(width * cellSize, 10f, height * cellSize);
        Bounds bounds = new Bounds(center, size);

        NavMeshBuildSettings buildSettings = NavMesh.GetSettingsByID(0);

        NavMeshData navMeshData = NavMeshBuilder.BuildNavMeshData(buildSettings, sources, bounds, transform.position, Quaternion.identity);

        if (navMeshData != null)
        {
            navMeshDataInstance = NavMesh.AddNavMeshData(navMeshData);
            Debug.Log("NavMesh baked at runtime.");
        }
        else
        {
            Debug.LogError("Failed to bake NavMesh at runtime.");
        }
    }

    void SpawnMonster()
    {
        Vector3 spawnPosition = new Vector3(65f, 1f, 65f);
        Quaternion spawnRotation = Quaternion.identity;

        
        GameObject obj = Instantiate(monster, spawnPosition, spawnRotation);


        AIVision vision = obj.GetComponent<AIVision>();
        if (vision != null)
            vision.player = player;


        AIController controller = obj.GetComponent<AIController>();
        if (controller != null)
            controller.player = player;
    }

}