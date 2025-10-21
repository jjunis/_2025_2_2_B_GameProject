using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{

    public static MazeGenerator Instance;

    [Header("미로 설정")]
    public int width = 10;
    public int height = 10;
    public GameObject cellPrefab;
    public float cellSize = 2f;

    [Header("시각화 설정")]
    public bool visualizeGeneration = false;
    public float viaulizationSpeed = 0.05f;
    public Color visitedColor = Color.cyan;
    public Color currentColor = Color.yellow;
    public Color backtrackColor = Color.magenta;

    private MazeCell[,] maze;
    private Stack<MazeCell> cellstack;

    // Start is called before the first frame update
    void Start()
    {
        GenerateMaze();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateMaze()
    {
        maze = new MazeCell[width, height];
        cellstack = new Stack<MazeCell>();

        CreateCells();              //모든 셀 생성

        if (visualizeGeneration)
        {
            StartCoroutine(GenerateWithDFSVisualized());
        }
        else
        {
            GenerateWithDFS();
        }
    }

    void GenerateWithDFS()          //DFS 알고리즘으로 생성
    {
        MazeCell current = maze[0, 0];
        current.visited = true;
        cellstack.Push(current);        //첫번째 현재칸을 스택에 넣는다

        while(cellstack.Count > 0)
        {
            current = cellstack.Peek();

            List<MazeCell> unvisitedNeighbors = GetUnvisitedNeighbors(current); //방문하지 않은 이웃 찾기

            if (unvisitedNeighbors.Count > 0)
            {
                MazeCell next = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];  //랜덤하게 이웃 선택
                RemoveWallBetween(current, next);       //벽 제거
                next.visited = true;
                cellstack.Push(next);
            }
            else
            {
                cellstack.Pop();        //벽 트래킹
            }
        }
    }

    void CreateCells()
    {
        if (cellstack == null)
        {
            Debug.LogError("셀 프리팹이 없음");
            return;
        }

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(x * cellSize, 0 , z * cellSize);
                GameObject CellObj = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                CellObj.name = $"Cell {x} {z}";

                MazeCell cell = CellObj.GetComponent<MazeCell>();
                if (cell == null)
                {
                    Debug.LogError("MazeCell 스크립트 없음");
                    return;
                }
                cell.Initialize(x, z);
                maze[x,z] = cell;
            }
        }
    }

    List<MazeCell> GetUnvisitedNeighbors(MazeCell cell)
    {
        List<MazeCell> neighbors = new List<MazeCell>();

        //상하좌우 체크
        if (cell.x > 0 && !maze[cell.x -1, cell.z].visited)
            neighbors.Add(maze[cell.x - 1, cell.z]);
        if (cell.x < width - 1 && !maze[cell.x + 1, cell.z].visited)
            neighbors.Add(maze[cell.x + 1, cell.z]);
        if (cell.z > 0 && !maze[cell.x, cell.z - 1].visited)
            neighbors.Add(maze[cell.x, cell.z - 1]);
        if (cell.z < width - 1 && !maze[cell.x, cell.z + 1].visited)
            neighbors.Add(maze[cell.x, cell.z + 1]);
        
        return neighbors;
    }

    void RemoveWallBetween(MazeCell current, MazeCell next)
    {
        if(current.x < next.x)      //오른쪽
        {
            current.RemoveWall("right");
            next.RemoveWall("left");
        }  
        else if(current.x > next.x)     //왼쪽
        {
            current.RemoveWall("left");
            next.RemoveWall("right");
        }
        else if (current.z < next.z)
        {
            current.RemoveWall("top");
            next.RemoveWall("bottom");
        }
        else if (current.z > next.z)
        {
            current.RemoveWall("bottom");
            next.RemoveWall("top");
        }
    }

    public MazeCell GetCell(int x, int z)
    {
        if (x >= 0 && x <width && z >= 0 &&  z < height)
            return maze[x, z];

        return null;
    }

    IEnumerator GenerateWithDFSVisualized()
    {
        MazeCell current = maze[0, 0];
        current.visited = true;

        current.SetColor(currentColor);        //+
        cellstack.Clear();                       //+

        cellstack.Push(current);            //첫번째 현재칸을 스택에 넣는다. 

        yield return new WaitForSeconds(viaulizationSpeed);         //+

        int totalCells = width * height;                            //+
        int visitedCount = 1;                                       //+

        while (cellstack.Count > 0)
        {
            current = cellstack.Peek();

            current.SetColor(currentColor);             //+ (현재 칸 강조)
            yield return new WaitForSeconds(viaulizationSpeed);

            List<MazeCell> unvisitedNeighbors = GetUnvisitedNeighbors(current); //방문하지 않은 이웃 찾기

            if (unvisitedNeighbors.Count > 0)
            {
                MazeCell next = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];  //랜덤하게 이웃 선택
                RemoveWallBetween(current, next);       //벽 제거

                current.SetColor(visitedColor);     //+ (현재 칸 방문 완료 색으로)
                next.visited = true;
                visitedCount++;                     //+
                cellstack.Push(next);

                next.SetColor(currentColor);
                yield return new WaitForSeconds(viaulizationSpeed);     //+
            }
            else
            {
                current.SetColor(backtrackColor);           //+
                yield return new WaitForSeconds(viaulizationSpeed);     //+

                current.SetColor(visitedColor);             //+
                cellstack.Pop();        //벽 트래킹
            }

            yield return new WaitForSeconds(viaulizationSpeed);     //+
            ResetAllColors();
            Debug.Log($"미로 생성 완료! 총({visitedCount} / {totalCells} 칸)");
        }
    }

    void ResetAllColors()
    {
        for (int x =0; x < width; x++)
        {
            for(int z =0; z < height; z++)
            {
                maze[x, z].SetColor(Color.white);
            }
        }
    }
}
