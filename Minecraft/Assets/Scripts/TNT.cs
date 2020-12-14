using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNT : MonoBehaviour
{
    private TerrainChunk tc;
    public GameObject tntPS;
    private Vector3 pos;

    public void explode()
    {
        int x = (int)pos.x;
        int z = (int)pos.z;
        int y = (int)pos.y;

        var tntPStemp = Instantiate(tntPS, new Vector3(x, y, z), Quaternion.identity);
        Destroy(tntPStemp, 3);

        if (x < 0) { x = Mathf.Abs((int)pos.x % 16); x = 15 - x; }
        else { x = Mathf.Abs((int)pos.x % 16); }
        if (z < 0) { z = Mathf.Abs((int)pos.z % 16); z = 15 - z; }
        else { z = Mathf.Abs((int)pos.z % 16); }

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                for (int k = -1; k < 2; k++)
                {
                    tc.blockType[i + x, j + y, k + z] = 0;
                }
            }
        }
        for (int i = 0; i < 100; i++)
        {
            Vector3 randomPos = Random.insideUnitSphere * 4;
            if ((int)randomPos.x + x < 16 && (int)randomPos.z + z < 16)
            {
                tc.blockType[(int)randomPos.x + x, (int)randomPos.y + y, (int)randomPos.z + z] = 0;
            }

        }
        tc.recreateTerrain();

    }

    public void passData(TerrainChunk tc_, Vector3 pos_)
    {
        tc = tc_;
        pos = pos_;
    }
}
