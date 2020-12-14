using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk : MonoBehaviour
{
    [HideInInspector] public int seed;
    [HideInInspector] public float amplitude;
    [HideInInspector] public float scale;
    [HideInInspector] public int xPos = 0, zPos = 0;
    private int numberOfMats = 9;

    static int sizeWidth = 16, sizeHeight = 256;
    Mesh worldMesh;
    public List<int> sideGrassTriangle, sideDirtTriangles, topTriangles;
    public List<int>[] topTris;
    public List<int>[] sideTris;
    public List<Vector3> vertices;
    public List<Vector2> uvs;
    List<Vector3> temp = new List<Vector3>();
    public int[, ,] blockType = new int[sizeWidth, sizeHeight, sizeWidth];
    public GameObject grass;
    [SerializeField] private GameObject creeper;
    

    //0=AIR
    //1=DIRT

    void Start()
    {
        for (int i=0; i< sizeWidth; i++)
        {
            for (int j=0; j< sizeHeight; j++)
            {
                for (int k=0; k< sizeWidth; k++)
                {
                    blockType[i, j, k] = 0;
                }
            }
        }
        topTris = new List<int>[numberOfMats];
        sideTris = new List<int>[numberOfMats];

        for (int i=0; i< numberOfMats; i++)
        {
            topTris[i] = new List<int>();
            sideTris[i] = new List<int>();
        }

        xPos = (int)transform.position.x;
        zPos = (int)transform.position.z;

        uvs = new List<Vector2>();
        worldMesh = new Mesh();
        sideGrassTriangle = new List<int>();
        sideDirtTriangles = new List<int>();
        topTriangles = new List<int>();
        vertices = new List<Vector3>();
        GetComponent<MeshFilter>().mesh = worldMesh;
        generateTerrain();
        createWater();
        StartCoroutine(createTerrain());
    }
    
    public void destroyTerrain()
    {
        vertices.Clear();
        sideGrassTriangle.Clear();
        sideDirtTriangles.Clear();
        topTriangles.Clear();
        uvs.Clear();
        updateTerrain();
    }
    
    public void generateTerrain()
    {
        for (int x = 0; x < sizeWidth; x++)
        {
            for (int z = 0; z < sizeWidth; z++)
            {
                float y = Mathf.PerlinNoise((x + seed + xPos) * scale, (z + seed + zPos) * scale);
                blockType[x, 0, z] = 8;
                for (int d = 1; d < (int)(y * amplitude)+96; d++)
                {
                    blockType[x, d, z] = 7;
                }
                for (int d = (int)(y * amplitude) + 95; d < (int)(y * amplitude) + 100; d++)
                {
                    blockType[x, d, z] = 2;
                }
                if ((int)(y * amplitude) < 7)
                {
                    blockType[x, (int)(y * amplitude) + 100, z] = 5; //SAND
                }
                else
                {
                    if (blockType[x, (int)(y * amplitude) + 101, z] == 0)
                    {
                        if (Random.Range(1, 100) == 51)
                        {
                            Instantiate(creeper, new Vector3(x + xPos, (int)(y * amplitude) + 100, z + zPos), Quaternion.identity);
                        }
                    }
                    blockType[x, (int)(y * amplitude) + 100, z] = 1; //GRASS
                }
            }
        }
    }

    public void recreateTerrain()
    {
        vertices.Clear();
        uvs.Clear();
        for (int i = 0; i < numberOfMats; i++)
        {
            topTris[i].Clear();
            sideTris[i].Clear();
        }

        for (int x = 0; x < sizeWidth; x++)
        {
            for (int z = 0; z < sizeWidth; z++)
            {
                for (int y = 0; y < sizeHeight - 1; y++)
                {
                    if (blockType[x, y, z] != 0) // IF BLOCK IS NOT AIR
                    {
                        // BUILD BLOCK
                        buildBlock(x, y, z, blockType[x, y, z]);
                    }
                }
            }
        }
        updateTerrain();
    }

    public IEnumerator createTerrain()
    {
        vertices.Clear();
        uvs.Clear();
        for (int i = 0; i < numberOfMats; i++)
        {
            topTris[i].Clear();
            sideTris[i].Clear();
        }

        for (int x = 0; x < sizeWidth; x++)
        {
            for (int z = 0; z < sizeWidth; z++)
            {
                for (int y = 0; y < sizeHeight - 1; y++)
                {
                    if (blockType[x, y, z] != 0) // IF BLOCK IS NOT AIR
                    {
                        // BUILD BLOCK
                        // 1 - grassBlock
                        // 2 - dirt
                        // 3 - wood
                        // 4 - leaves
                        // 5 - sand
                        // 6 - water
                        // 7 - stone
                        // 8 - bedrock
                        // 9 - tnt
                        buildBlock(x, y, z, blockType[x, y, z]);
                    }
                    yield return null;
                }
                
            }
        }
        
        updateTerrain();
    }


    public void createWater()
    {
        for (int x=0; x<sizeWidth; x++) 
        { 
            for (int z=0; z<sizeWidth; z++)
            {
                if (blockType[x, 106, z] == 0)
                {
                    if (blockType[x, 106, z] == 0) // ADD TOP
                    {
                        blockType[x, 106, z] = 6;
                    }
                }
            }
        }
    }

    public void buildBlock(int x, int y, int z, int type)
    {
        type--;
        if (blockType[x, y + 1, z] == 0 || blockType[x, y + 1, z] == 6) // ADD TOP
        {
            if (blockType[x, y, z] == 6)
            {
                vertices.Add(new Vector3(x + 1, y - 0.2f, z + 1));
                vertices.Add(new Vector3(x + 1, y - 0.2f, z));
                vertices.Add(new Vector3(x, y - 0.2f, z + 1));
                vertices.Add(new Vector3(x, y -0.2f, z));
            }
            else
            {
                vertices.Add(new Vector3(x + 1, y, z + 1));
                vertices.Add(new Vector3(x + 1, y, z));
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x, y, z));
            }
            
            topTris[type].Add(vertices.Count - 4);
            topTris[type].Add(vertices.Count - 3);
            topTris[type].Add(vertices.Count - 2);
            topTris[type].Add(vertices.Count - 2);
            topTris[type].Add(vertices.Count - 3);
            topTris[type].Add(vertices.Count - 1);
            uvs.Add(new Vector2(vertices[vertices.Count - 4].x, vertices[vertices.Count - 4].z));
            uvs.Add(new Vector2(vertices[vertices.Count - 3].x, vertices[vertices.Count - 3].z));
            uvs.Add(new Vector2(vertices[vertices.Count - 2].x, vertices[vertices.Count - 2].z));
            uvs.Add(new Vector2(vertices[vertices.Count - 1].x, vertices[vertices.Count - 1].z));
        }
        if (blockType[x,y,z] != 6)
        {
            if (y > 0 && (blockType[x, y - 1, z] == 0 || blockType[x, y - 1, z] == 6)) // ADD BOT
            {
                vertices.Add(new Vector3(x + 1, y - 1, z + 1));
                vertices.Add(new Vector3(x + 1, y - 1, z));
                vertices.Add(new Vector3(x, y - 1, z + 1));
                vertices.Add(new Vector3(x, y - 1, z));
                topTris[type].Add(vertices.Count - 2);
                topTris[type].Add(vertices.Count - 3);
                topTris[type].Add(vertices.Count - 4);
                topTris[type].Add(vertices.Count - 1);
                topTris[type].Add(vertices.Count - 3);
                topTris[type].Add(vertices.Count - 2);
                uvs.Add(new Vector2(vertices[vertices.Count - 4].x, vertices[vertices.Count - 4].z));
                uvs.Add(new Vector2(vertices[vertices.Count - 3].x, vertices[vertices.Count - 3].z));
                uvs.Add(new Vector2(vertices[vertices.Count - 2].x, vertices[vertices.Count - 2].z));
                uvs.Add(new Vector2(vertices[vertices.Count - 1].x, vertices[vertices.Count - 1].z));
            }
            if ((x < sizeWidth - 1 && y > 0 && (blockType[x + 1, y, z] == 0 || blockType[x + 1, y, z] == 6)) || x == sizeWidth - 1) // ADD LEFT
            {
                vertices.Add(new Vector3(x + 1, y - 1, z + 1));
                vertices.Add(new Vector3(x + 1, y - 1, z));
                vertices.Add(new Vector3(x + 1, y, z + 1));
                vertices.Add(new Vector3(x + 1, y, z));

                sideTris[type].Add(vertices.Count - 1);
                sideTris[type].Add(vertices.Count - 2);
                sideTris[type].Add(vertices.Count - 3);
                sideTris[type].Add(vertices.Count - 4);
                sideTris[type].Add(vertices.Count - 3);
                sideTris[type].Add(vertices.Count - 2);

                uvs.Add(new Vector2(vertices[vertices.Count - 4].z, vertices[vertices.Count - 4].y));
                uvs.Add(new Vector2(vertices[vertices.Count - 3].z, vertices[vertices.Count - 3].y));
                uvs.Add(new Vector2(vertices[vertices.Count - 2].z, vertices[vertices.Count - 2].y));
                uvs.Add(new Vector2(vertices[vertices.Count - 1].z, vertices[vertices.Count - 1].y));
            }
            if ((x > 0 && (blockType[x - 1, y, z] == 0 || blockType[x - 1, y, z] == 6)) || x == 0) // ADD RIGHT
            {
                vertices.Add(new Vector3(x, y - 1, z + 1));
                vertices.Add(new Vector3(x, y - 1, z));
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x, y, z));
                sideTris[type].Add(vertices.Count - 3);
                sideTris[type].Add(vertices.Count - 2);
                sideTris[type].Add(vertices.Count - 1);
                sideTris[type].Add(vertices.Count - 2);
                sideTris[type].Add(vertices.Count - 3);
                sideTris[type].Add(vertices.Count - 4);

                uvs.Add(new Vector2(vertices[vertices.Count - 4].z, vertices[vertices.Count - 4].y));
                uvs.Add(new Vector2(vertices[vertices.Count - 3].z, vertices[vertices.Count - 3].y));
                uvs.Add(new Vector2(vertices[vertices.Count - 2].z, vertices[vertices.Count - 2].y));
                uvs.Add(new Vector2(vertices[vertices.Count - 1].z, vertices[vertices.Count - 1].y));
            }
            if ((z < sizeWidth - 1 && y > 0 && (blockType[x, y, z + 1] == 0 || blockType[x, y, z + 1] == 6)) || z == sizeWidth - 1) // ADD FRONT
            {
                vertices.Add(new Vector3(x + 1, y - 1, z + 1));
                vertices.Add(new Vector3(x, y - 1, z + 1));
                vertices.Add(new Vector3(x + 1, y, z + 1));
                vertices.Add(new Vector3(x, y, z + 1));

                sideTris[type].Add(vertices.Count - 3);
                sideTris[type].Add(vertices.Count - 2);
                sideTris[type].Add(vertices.Count - 1);
                sideTris[type].Add(vertices.Count - 2);
                sideTris[type].Add(vertices.Count - 3);
                sideTris[type].Add(vertices.Count - 4);

                uvs.Add(new Vector2(vertices[vertices.Count - 4].x, vertices[vertices.Count - 4].y));
                uvs.Add(new Vector2(vertices[vertices.Count - 3].x, vertices[vertices.Count - 3].y));
                uvs.Add(new Vector2(vertices[vertices.Count - 2].x, vertices[vertices.Count - 2].y));
                uvs.Add(new Vector2(vertices[vertices.Count - 1].x, vertices[vertices.Count - 1].y));
            }
            if ((z > 0 && (blockType[x, y, z - 1] == 0 || blockType[x, y, z - 1] == 6)) || z == 0) // ADD BACK 
            {
                vertices.Add(new Vector3(x + 1, y - 1, z));
                vertices.Add(new Vector3(x, y - 1, z));
                vertices.Add(new Vector3(x + 1, y, z));
                vertices.Add(new Vector3(x, y, z));

                sideTris[type].Add(vertices.Count - 1);
                sideTris[type].Add(vertices.Count - 2);
                sideTris[type].Add(vertices.Count - 3);
                sideTris[type].Add(vertices.Count - 4);
                sideTris[type].Add(vertices.Count - 3);
                sideTris[type].Add(vertices.Count - 2);

                uvs.Add(new Vector2(vertices[vertices.Count - 4].x, vertices[vertices.Count - 4].y));
                uvs.Add(new Vector2(vertices[vertices.Count - 3].x, vertices[vertices.Count - 3].y));
                uvs.Add(new Vector2(vertices[vertices.Count - 2].x, vertices[vertices.Count - 2].y));
                uvs.Add(new Vector2(vertices[vertices.Count - 1].x, vertices[vertices.Count - 1].y));
            }
        }
        
    }

    public void updateTerrain()
    {

        worldMesh.Clear();
        worldMesh.vertices = vertices.ToArray();
        int subMeshCount = numberOfMats*2;
        worldMesh.subMeshCount = subMeshCount;
        int index = 0;
        for (int i = 0; i < numberOfMats; i++) 
        {
            worldMesh.SetTriangles(topTris[i], index);
            worldMesh.SetTriangles(sideTris[i], index+1);
            index += 2;
        }

        worldMesh.uv = uvs.ToArray();
        worldMesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = worldMesh;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i=0; i<temp.Count; i++)
        {
            Gizmos.DrawCube(temp[i], new Vector3(0.1f, 0.1f, 0.1f));
        }
    }


}
