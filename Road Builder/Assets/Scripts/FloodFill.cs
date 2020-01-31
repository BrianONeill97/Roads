using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodFill : MonoBehaviour
{
    Vector3 fillPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(GetComponent<TiledRoadCreator>()._get3dMousePosition(), 0.0f); ;
        if (hits.Length > 0)
        {
            fillPoint = hits[0].gameObject.transform.position;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log(hits.Length);
            floodFill(fillPoint);

        }
    }

    void floodFill(Vector3 pos)
    {
        Collider[] hit = Physics.OverlapSphere(pos,0);

        if (hit.Length == 0)
        {
            Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), pos, Quaternion.identity);
            floodFill(new Vector3(pos.x + 1, pos.y, pos.z));
            floodFill(new Vector3(pos.x, pos.y, pos.z + 1));
            floodFill(new Vector3(pos.x - 1, pos.y, pos.z));
            floodFill(new Vector3(pos.x, pos.y, pos.z - 1));
        }
    }
}
