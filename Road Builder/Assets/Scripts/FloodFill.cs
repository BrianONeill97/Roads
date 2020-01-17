using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodFill : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(GetComponent<TiledRoadCreator>()._get3dMousePosition(), 0.0f); ;

        if (Input.GetKeyUp(KeyCode.Space))
        {
            floodFill(hits[0].transform.position);

        }
    }

    void floodFill(Vector3 pos)
    {
        Debug.Log(pos);
        Collider[] hit = Physics.OverlapSphere(pos,0);

        if (hit[0].gameObject.tag != "Road")
        {

            //Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), pos, Quaternion.identity);
            floodFill(new Vector3(pos.x + GetComponent<TiledRoadCreator>().GetSize(hit[0].gameObject).x, pos.y, pos.z));
            floodFill(new Vector3(pos.x, pos.y, pos.z + GetComponent<TiledRoadCreator>().GetSize(hit[0].gameObject).z));
            floodFill(new Vector3(pos.x - GetComponent<TiledRoadCreator>().GetSize(hit[0].gameObject).x, pos.y, pos.z));
            floodFill(new Vector3(pos.x, pos.y, pos.z - GetComponent<TiledRoadCreator>().GetSize(hit[0].gameObject).z));
        }
    }
}
