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
        if(Input.GetKeyUp(KeyCode.Space))
        {
            floodFill(transform.position);

        }
    }

    void floodFill(Vector3 pos)
    {
        Debug.Log(pos);
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
