using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    public int seed, treeSeed;
    [SerializeField] private GameObject terrainChunk;
    [SerializeField] private float amplitude;
    [SerializeField] private float scale;
    [SerializeField] private GameObject player;
    [SerializeField] private int renderDistance = 3;
    private List<GameObject> chunks;
    
    private void Update()
    {
        loadChunks();
        unloadChunks();
    }

    // Start is called before the first frame update
    void Awake()
    {
        seed = Random.Range(1, 100000);
        treeSeed = Random.Range(1, 100000);
        chunks = new List<GameObject>();

        createChunk(0, 0);
    }

    public void createChunk(int x, int y)
    {
        GameObject chunk = Instantiate(terrainChunk, new Vector3((x) * 16, 0, (y) * 16), Quaternion.identity);
        chunk.transform.parent = transform;
        chunk.GetComponent<TerrainChunk>().scale = scale;
        chunk.GetComponent<TerrainChunk>().amplitude = amplitude;
        chunk.GetComponent<TerrainChunk>().seed = seed;

        chunk.GetComponent<TreeGeneration>().worldAmplitude = amplitude;
        chunk.GetComponent<TreeGeneration>().treeSeed = treeSeed;
        chunk.name = "chunk" + x.ToString() + y.ToString();
        chunks.Add(chunk);
    }

    public void loadChunks()
    {
        Vector2 currentChunkPosition = findCurrentChunkPosition();


        for (int i=1; i<renderDistance; i++)
        {
            for (int j=1; j< renderDistance; j++)
            {
                string chunkName = "chunk" + currentChunkPosition.x.ToString() + currentChunkPosition.y.ToString();
                if (ifChunkExist(chunkName))
                {
                    chunkName = "chunk" + (currentChunkPosition.x + i).ToString() + currentChunkPosition.y.ToString();
                    if (!ifChunkExist(chunkName))
                    {
                        createChunk((int)currentChunkPosition.x + i, (int)currentChunkPosition.y);
                    }
                    chunkName = "chunk" + (currentChunkPosition.x).ToString() + (currentChunkPosition.y + j).ToString();
                    if (!ifChunkExist(chunkName))
                    {
                        createChunk((int)currentChunkPosition.x, (int)currentChunkPosition.y + j);
                    }
                    chunkName = "chunk" + (currentChunkPosition.x - i).ToString() + currentChunkPosition.y.ToString();
                    if (!ifChunkExist(chunkName))
                    {
                        createChunk((int)currentChunkPosition.x - i, (int)currentChunkPosition.y);
                    }
                    chunkName = "chunk" + (currentChunkPosition.x).ToString() + (currentChunkPosition.y - j).ToString();
                    if (!ifChunkExist(chunkName))
                    {
                        createChunk((int)currentChunkPosition.x, (int)currentChunkPosition.y - j);
                    }


                    chunkName = "chunk" + (currentChunkPosition.x + i).ToString() + (currentChunkPosition.y + j).ToString();
                    if (!ifChunkExist(chunkName))
                    {
                        createChunk((int)currentChunkPosition.x + i, (int)currentChunkPosition.y + j);
                    }
                    chunkName = "chunk" + (currentChunkPosition.x - i).ToString() + (currentChunkPosition.y + j).ToString();
                    if (!ifChunkExist(chunkName))
                    {
                        createChunk((int)currentChunkPosition.x - i, (int)currentChunkPosition.y + j);
                    }
                    chunkName = "chunk" + (currentChunkPosition.x + i).ToString() + (currentChunkPosition.y - j).ToString();
                    if (!ifChunkExist(chunkName))
                    {
                        createChunk((int)currentChunkPosition.x + i, (int)currentChunkPosition.y - j);
                    }
                    chunkName = "chunk" + (currentChunkPosition.x - i).ToString() + (currentChunkPosition.y - j).ToString();
                    if (!ifChunkExist(chunkName))
                    {
                        createChunk((int)currentChunkPosition.x - i, (int)currentChunkPosition.y - j);
                    }
                }
            }
        }

        
    }

    public void unloadChunks()
    {
        Vector2 currentChunkPosition = findCurrentChunkPosition();
        List<string> chunksInUse = new List<string>();
        chunksInUse.Add("chunk" + currentChunkPosition.x.ToString() + currentChunkPosition.y.ToString());

        for (int i=1; i< renderDistance; i++)
        {
            for (int j=1; j< renderDistance; j++)
            {
                chunksInUse.Add("chunk" + (currentChunkPosition.x + i).ToString() + currentChunkPosition.y.ToString());
                chunksInUse.Add("chunk" + currentChunkPosition.x.ToString() + (currentChunkPosition.y + j).ToString());
                chunksInUse.Add("chunk" + (currentChunkPosition.x - i).ToString() + currentChunkPosition.y.ToString());
                chunksInUse.Add("chunk" + currentChunkPosition.x.ToString() + (currentChunkPosition.y - j).ToString());

                chunksInUse.Add("chunk" + (currentChunkPosition.x + i).ToString() + (currentChunkPosition.y + j).ToString());
                chunksInUse.Add("chunk" + (currentChunkPosition.x - i).ToString() + (currentChunkPosition.y + j).ToString());
                chunksInUse.Add("chunk" + (currentChunkPosition.x + i).ToString() + (currentChunkPosition.y - j).ToString());
                chunksInUse.Add("chunk" + (currentChunkPosition.x - i).ToString() + (currentChunkPosition.y - j).ToString());
            }
        }
        

        foreach (GameObject chunk in chunks.ToArray())
        {
            if (chunk == null) { continue; }
            bool exist = false;
            foreach (string name in chunksInUse)
            {
                if (name == null) { continue; }
                if (name == chunk.name)
                {
                    exist = true;
                }
            }
            if (!exist)
            {
                chunks.Remove(chunk);
                Destroy(transform.Find(chunk.name).gameObject);
            }
        }
    }

    public bool ifChunkExist(string chunkName)
    {
        for (int i=0; i<chunks.Count; i++)
        {
            if (chunks[i].name == chunkName)
            {
                return true;
            }
        }
        return false;
    }

    public Vector2 findCurrentChunkPosition()
    {
        int x = (int)player.transform.position.x;
        int z = (int)player.transform.position.z;
        
        
        if (x < 0) { x = x / 16; x = x - 1; }
        else { x = x / 16; }
        if (z < 0) { z = z / 16; z = z - 1; }
        else { z = z / 16; }
        return new Vector2(x, z);
    }

    public TerrainChunk findChunk(int x, int z)
    {
        for (int i=0; i<chunks.Count; i++)
        {
            if (chunks[i].gameObject.transform.position.x/16 == (x / 16) && chunks[i].gameObject.transform.position.z/16 == (z / 16))
            {
                return chunks[i].GetComponent<TerrainChunk>();
            }
        }
        return null;
    }

}
