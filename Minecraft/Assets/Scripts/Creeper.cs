using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creeper : MonoBehaviour
{
    private GameObject player;
    private Animator anim;
    private Terrain terrain;
    [SerializeField] GameObject explosionPS;
    TerrainChunk tc;

    // Start is called before the first frame update
    void Start()
    {
        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        player = GameObject.Find("Player");
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < 100)
        {
            Destroy(gameObject);
        }
        float dist = Vector3.Distance(player.transform.position, transform.position);
        if (dist < 4)
        {
            //  Destroy(gameObject, 0.1f);
            //explode();
        }
        else if (dist < 200)
        {
            anim.SetBool("walk", true);
            followPlayer();
        }
        else {
            anim.SetBool("walk", false); 
        }
    }

    void followPlayer()
    {
        float step = 2 * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, player.transform.position - transform.position, step * 5, 0.0f);
        transform.rotation = Quaternion.LookRotation(new Vector3(newDirection.x, 0, newDirection.z));
    }

    public void explode()
    {
        tc = terrain.findChunk((int)transform.position.x, (int)transform.position.z);
        int x = (int)transform.position.x;
        int z = (int)transform.position.z;
        int y = (int)transform.position.y;

        var tntPStemp = Instantiate(explosionPS, new Vector3(x, y, z), Quaternion.identity);
        Destroy(tntPStemp, 3);

        if (x < 0) { x = Mathf.Abs((int)transform.position.x % 16); x = 15 - x; }
        else { x = Mathf.Abs((int)transform.position.x % 16); }
        if (z < 0) { z = Mathf.Abs((int)transform.position.z % 16); z = 15 - z; }
        else { z = Mathf.Abs((int)transform.position.z % 16); }

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
}
