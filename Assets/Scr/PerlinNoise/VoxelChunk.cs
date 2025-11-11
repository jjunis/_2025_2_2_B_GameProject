using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelChunk : MonoBehaviour
{
    [Header("청크 설정")]
    public int chunkSize = 16;
    public int chunkHeight = 64;

    [Header("perlin Noise 설정")]
    public float noiseScale = 0.1f;
    public int octaves = 3;
    public float persistence = 0.5f;
    public float lacunarity = 2.0f;

    [Header("지형 높이")]
    public int groundLevel = 32;
    public int heightVariation = 16;

    [Header("청크 배치 옵션")]
    public bool autoPositionByChunk = true;                         //청크 좌표로 자동 배치 

    //3D 블록 배열
    private BlockType[,,] blocks;
    private Mesh chunkMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    //청크 위치
    public Vector2Int chunkPosition;

    // Start is called before the first frame update
    void Start()
    {
        SetupMesh();

        if (autoPositionByChunk)        //청크 좌표를 월드 위치로 적용
        {
            transform.position = new Vector3(chunkPosition.x * chunkSize, 0.0f, chunkPosition.y * chunkSize);
        }

        GenerateChunk();
        BuildMesh();
    }


    void SetupMesh()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshCollider = gameObject.AddComponent<MeshCollider>();

        //Verte Color Shader 
        Shader vertexColorShader = Shader.Find("Custom/VertexColor");
        if (vertexColorShader == null)
        {
            Debug.LogWarning("VertexColor 쉐이더를 찾을 수 없습니다. ");
            vertexColorShader = Shader.Find("Unlit/Color");
        }

        meshRenderer.material = new Material(vertexColorShader);

        chunkMesh = new Mesh();
        chunkMesh.name = "VoxelChunk";

        chunkMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;        //128넘어가도 잘 동작하게 하기 위해    
    }

    //3D perlin noise로 동굴 생성 

    bool IsCaveAt(int x, int y, int z)
    {
        float caveScale = 0.05f;
        float caveThreshold = 0.55f;

        float cave1 = Mathf.PerlinNoise(x * caveScale, z * caveScale);
        float cave2 = Mathf.PerlinNoise(x * caveScale + 100, y * caveScale * 0.5f);
        float cave3 = Mathf.PerlinNoise(y * caveScale * 0.5f, z * caveScale + 200f);

        float caveValue = (cave1 + cave2 + cave3) / 3f;

        return caveValue > caveThreshold;
    }

    //돌 & 광맥 
    BlockType GetStoneWithOre(int x, int y, int z)
    {
        float oreNoise = Mathf.PerlinNoise(x * 0.1f + 500f, z * 0.1f + 500f);

        //높이에 따라 광맥 종류 다름
        if (y < 10)
        {
            if (oreNoise > 0.95f)                   //깊은곳
                return BlockType.DiamondOre;
        }

        if (y < 20)
        {
            if (oreNoise > 0.92f)
                return BlockType.GoldOre;
        }

        if (y < 35)
        {
            if (oreNoise > 0.85f)
                return BlockType.IronOre;
        }

        if (oreNoise > 0.75f)
            return BlockType.CoalOre;

        return BlockType.Stone;
    }


    // Perlin Noise  지형 높이 계산
    int GetTerrainHeight(int worldX, int worldZ)
    {
        float amplitude = 1.0f;                                 //진폭
        float frequency = 1.0f;                                 //주파수
        float noiseHeight = 0f;                                 //누적 시킨 노이즈 높이값

        //여러 옥타브 합성
        for (int i = 0; i < octaves; i++)
        {
            float sampleX = worldX * noiseScale * frequency;
            float sampleZ = worldZ * noiseScale * frequency;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ);
            noiseHeight += perlinValue * amplitude;                         //진폭을 적용해서 누적

            amplitude *= persistence;                           //다음 옥타브로 갈 수록 진폭 줄인다. 
            frequency *= persistence;                           //주파수를 조정 
        }

        //높이 범위 조정
        int height = groundLevel + Mathf.RoundToInt(noiseHeight * heightVariation);     //기본 지면 높이 적용 
        return Mathf.Clamp(height, 1, chunkHeight - 1);                 //높이를 최소 1 에서 최대 사이로 제한
    }

    public void GenerateChunk()
    {
        blocks = new BlockType[chunkSize, chunkHeight, chunkSize];

        int waterLevel = 28; //물 높이

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                //월드 좌표 
                int worldX = chunkPosition.x * chunkSize + x;
                int worldZ = chunkPosition.y * chunkSize + z;

                int height = GetTerrainHeight(worldX, worldZ);  //Perlin Noise 로 높이 결정

                for (int y = 0; y < chunkHeight; y++)
                {
                    //동굴 생성
                    bool isCave = IsCaveAt(worldX, y, worldZ);

                    if (y == 0)
                    {
                        blocks[x, y, z] = BlockType.Bedrock;                    //맨 아래는 기반암
                    }
                    else if (isCave && y > 5 && y < height - 1)
                    {
                        blocks[x, y, z] = BlockType.Air;                        //동굴은 비움
                    }
                    else if (y < height - 4)
                    {
                        blocks[x, y, z] = GetStoneWithOre(worldX, y, worldZ);       //깊은 곳 돌 
                    }
                    else if (y < height - 1)
                    {
                        blocks[x, y, z] = BlockType.Dirt;                   //표면 아래는 흙
                    }
                    else if (y == height - 1)
                    {
                        if (height > waterLevel + 1)
                        {
                            blocks[x, y, z] = BlockType.Grass;
                        }
                        else
                        {
                            blocks[x, y, z] = BlockType.Sand;
                        }
                    }
                    else if (y < waterLevel)
                    {
                        blocks[x, y, z] = BlockType.Water;
                    }
                    else
                    {
                        blocks[x, y, z] = BlockType.Air;
                    }

                }

            }
        }

    }

    //한 면 추가
    void AddFace(int x, int y, int z, Vector3 direction, Color color, List<Vector3> vertices, List<int> traingles, List<Color> colors)
    {
        int vertCount = vertices.Count;
        Vector3 pos = new Vector3(x, y, z);

        //방향에 따라 정점 배치
        if (direction == Vector3.up)
        {
            vertices.Add(pos + new Vector3(0, 1, 0));
            vertices.Add(pos + new Vector3(0, 1, 1));
            vertices.Add(pos + new Vector3(1, 1, 1));
            vertices.Add(pos + new Vector3(1, 1, 0));
        }
        else if (direction == Vector3.down)
        {
            vertices.Add(pos + new Vector3(0, 0, 0));
            vertices.Add(pos + new Vector3(1, 0, 0));
            vertices.Add(pos + new Vector3(1, 0, 1));
            vertices.Add(pos + new Vector3(0, 0, 1));
        }
        else if (direction == Vector3.forward)
        {
            vertices.Add(pos + new Vector3(1, 0, 0));
            vertices.Add(pos + new Vector3(0, 0, 0));
            vertices.Add(pos + new Vector3(0, 1, 0));
            vertices.Add(pos + new Vector3(1, 1, 0));
        }
        else if (direction == Vector3.back)
        {
            vertices.Add(pos + new Vector3(1, 0, 0));
            vertices.Add(pos + new Vector3(0, 0, 0));
            vertices.Add(pos + new Vector3(0, 1, 0));
            vertices.Add(pos + new Vector3(1, 1, 0));
        }
        else if (direction == Vector3.right)
        {
            vertices.Add(pos + new Vector3(1, 0, 0));
            vertices.Add(pos + new Vector3(1, 1, 0));
            vertices.Add(pos + new Vector3(1, 1, 1));
            vertices.Add(pos + new Vector3(1, 0, 1));
        }
        else if (direction == Vector3.left)
        {
            vertices.Add(pos + new Vector3(0, 0, 1));
            vertices.Add(pos + new Vector3(0, 1, 1));
            vertices.Add(pos + new Vector3(0, 1, 0));
            vertices.Add(pos + new Vector3(0, 0, 0));
        }

        //삼각형
        traingles.Add(vertCount + 0);
        traingles.Add(vertCount + 1);
        traingles.Add(vertCount + 2);
        traingles.Add(vertCount + 0);
        traingles.Add(vertCount + 2);
        traingles.Add(vertCount + 3);

        //색상
        for (int i = 0; i < 4; i++)
        {
            colors.Add(color);
        }
    }

    bool IsTransparent(int x, int y, int z)          //특정 위치가 투명인지 체크 
    {
        if (x < 0 || x >= chunkSize || y < 0 || y >= chunkHeight || z < 0 || z >= chunkSize)
            return true;

        return blocks[x, y, z] == BlockType.Air;
    }

    //블록의 보이는 면만 추가 
    void AddBlockFaces(int x, int y, int z, BlockType block, List<Vector3> vertices, List<int> triangles, List<Color> colors)
    {
        BlockData blockData = new BlockData(block);

        if (IsTransparent(x, y + 1, z))             //위
        {
            AddFace(x, y, z, Vector3.up, blockData.blockColor, vertices, triangles, colors);
        }
        if (IsTransparent(x, y - 1, z))             //아래
        {
            AddFace(x, y, z, Vector3.down, blockData.blockColor, vertices, triangles, colors);
        }
        if (IsTransparent(x, y, z + 1))             //앞
        {
            AddFace(x, y, z, Vector3.forward, blockData.blockColor, vertices, triangles, colors);
        }
        if (IsTransparent(x, y, z - 1))             //뒤
        {
            AddFace(x, y, z, Vector3.back, blockData.blockColor, vertices, triangles, colors);
        }
        if (IsTransparent(x + 1, y, z))             //오른쪽
        {
            AddFace(x, y, z, Vector3.right, blockData.blockColor, vertices, triangles, colors);
        }
        if (IsTransparent(x - 1, y, z))             //왼쪽
        {
            AddFace(x, y, z, Vector3.left, blockData.blockColor, vertices, triangles, colors);
        }
    }

    public void BuildMesh()             //메쉬 생성
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Color> colors = new List<Color>();

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    BlockType block = blocks[x, y, z];
                    if (block == BlockType.Air) continue;

                    AddBlockFaces(x, y, z, block, vertices, triangles, colors); //6면 체크
                }
            }
        }

        chunkMesh.Clear();
        chunkMesh.vertices = vertices.ToArray();
        chunkMesh.triangles = triangles.ToArray();
        chunkMesh.colors = colors.ToArray();
        chunkMesh.RecalculateNormals();

        meshFilter.mesh = chunkMesh;
        meshCollider.sharedMesh = chunkMesh;
    }

}