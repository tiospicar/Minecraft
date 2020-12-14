using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGeneration : MonoBehaviour
{
    [HideInInspector] public int treeSeed;
    [HideInInspector] public float worldAmplitude;
    [SerializeField] private float amplitude;
    [SerializeField] private float scale;
    private int xPos = 0, zPos = 0;
    [SerializeField] private int heightTreshold;
    private int sizeWidth = 16;
    private int seed;
    public GameObject cube;
    private TerrainChunk chunk;
    private Terrain terrain;

    private void Start()
    {
        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        seed = terrain.seed;
        xPos = (int)transform.position.x;
        zPos = (int)transform.position.z;
        chunk = transform.GetComponent<TerrainChunk>();
        StartCoroutine(generateTrees());
    }

    IEnumerator generateTrees()
    {
        for (int x=0; x<sizeWidth; x++)
        {
            for (int z=0; z<sizeWidth; z++)
            {
                float treeY = Mathf.PerlinNoise((x + treeSeed + xPos) * scale, (z + treeSeed + zPos) * scale);
                float worldY = Mathf.PerlinNoise((x + seed + xPos) * scale, (z + seed + zPos) * scale);
                if ((int)(treeY * amplitude) > heightTreshold && (int)(worldY * worldAmplitude) > heightTreshold)
                {
                    if (Random.Range(1, 15) == 7)
                    {
                        int height = Random.Range(4, 6);
                        for (int i=0; i<height; i++)
                        {
                            chunk.blockType[x, (int)(worldY * worldAmplitude) + 101 + i, z] = 3;
                        }
                        for (int i = 0; i < 5; i++){
                            for (int j = 0; j < 5; j++){
                                for (int k = 0; k < 5; k++){
                                    int globalX = xPos + (x + (i - 2));
                                    int globalZ = zPos + (z + (j - 2));
                                    if (globalX < 0) { globalX = globalX - 16; }
                                    if (globalZ < 0) { globalZ = globalZ - 16; }

                                    TerrainChunk tc = terrain.findChunk(globalX, globalZ);
                                    if (tc != null)
                                    {
                                        if (globalX < 0) { globalX = 15 - (Mathf.Abs(globalX) % 16); }
                                        else { globalX = Mathf.Abs(globalX) % 16; }
                                        if (globalZ < 0) { globalZ = 15 - (Mathf.Abs(globalZ) % 16); }
                                        else { globalZ = Mathf.Abs(globalZ) % 16; }
                                        tc.blockType[globalX, (int)(worldY * worldAmplitude) + 101 + height + k, globalZ] = 4;
                                    }
                                }
                            }
                        }
                        yield return null;

                    }
                }
            }
            
        }
        chunk.recreateTerrain();
    }


}
