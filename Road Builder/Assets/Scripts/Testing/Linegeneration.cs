using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linegeneration : MonoBehaviour
{
    // Start is called before the first frame update

    Vector3 start = new Vector3(1, 5, 0);
    Vector3 end = new Vector3(2, -1, 0);

    public GameObject cube;

    bool test = false;

    int i = 0;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(start, end, Color.white, 10000f, false);
        GenBlocks();
    }



    void GenBlocks()
    {
        Vector3 startb = new Vector3();
        Vector3 distanceV = end - start;
        Vector3 distanceVNorm = Vector3.Normalize(distanceV);
        float mag = Mathf.Round(Vector3.Magnitude(distanceV));

        Quaternion rot = Quaternion.LookRotation(distanceV, Vector3.up);
        
        //for(int i = 0; i < mag; i++)
        //{
        //    startb = startb + (distanceVNorm * 1);
        //    Instantiate(cube, startb, rot);
        //}

        while(i < mag)
        {
            startb = startb + (distanceVNorm * 1);
            Instantiate(cube, startb, rot);
            i++;
        }
    }

    Vector3 VectorMult(Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }

}
