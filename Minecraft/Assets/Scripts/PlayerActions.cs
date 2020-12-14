using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject destroyPS;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject tnt;
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // BREAK BLOCK
        {
            animator.SetBool("dig", true);
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
            {
                Vector3 hitBlock = hit.point - (hit.normal * 0.5f);
                int x = (int)hitBlock.x;
                int z = (int)hitBlock.z;

                if (x < 0) { x = Mathf.Abs((int)hitBlock.x % 16); x = 15 - x; }
                else { x = Mathf.Abs((int)hitBlock.x % 16); }
                if (z < 0) { z = Mathf.Abs((int)hitBlock.z % 16); z = 15 - z; }
                else { z = Mathf.Abs((int)hitBlock.z % 16); }
                hit.transform.GetComponent<TerrainChunk>().blockType[x, (int)hitBlock.y + 1, z] = 0;
                hit.transform.GetComponent<TerrainChunk>().recreateTerrain();
                var temp = Instantiate(destroyPS, hit.point, Quaternion.identity);
                Destroy(temp, 3);
            }
        }
        else if (Input.GetMouseButtonDown(1)) // ADD BLOCK
        {
            animator.SetBool("dig", true);
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
            {
                Vector3 hitBlock = hit.point + (hit.normal * 0.5f);
                int x = (int)hitBlock.x;
                int z = (int)hitBlock.z;

                if (hitBlock == transform.position) { return; }

                if (x < 0) { x = Mathf.Abs((int)hitBlock.x % 16); x = 15 - x; }
                else { x = Mathf.Abs((int)hitBlock.x % 16); }
                if (z < 0) { z = Mathf.Abs((int)hitBlock.z % 16); z = 15 - z; }
                else { z = Mathf.Abs((int)hitBlock.z % 16); }
                hit.transform.GetComponent<TerrainChunk>().blockType[x, (int)hitBlock.y + 1, z] = 1;
                hit.transform.GetComponent<TerrainChunk>().recreateTerrain();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            animator.SetBool("dig", true);
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
            {
                Vector3 hitBlock = hit.point + (hit.normal * 0.5f);
                TerrainChunk tc = hit.transform.GetComponent<TerrainChunk>();
                float x=(int)hitBlock.x, z=(int)hitBlock.z;
                if (x < 0)
                { x = x - 0.5f; }
                else { x = x + 0.5f; }
                if (z < 0) { z = z - 0.5f; }
                else { z = z + 0.5f; }
                var tntTemp = Instantiate(tnt, new Vector3(x, (int)hitBlock.y + 0.5f, z), Quaternion.identity);
                tntTemp.GetComponent<TNT>().passData(tc, hitBlock);
                Destroy(tntTemp, 3.05f);
            }
        }
    }
}
