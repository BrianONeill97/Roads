using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linegeneration : MonoBehaviour
{
    // Start is called before the first frame update

    Vector3 start = new Vector3();
    Vector3 end = new Vector3();
    Vector3 veryFirstPoint = new Vector3();


    public GameObject cube;

    bool firstTime = true;

    int i = 0;

    Camera thisCamera;

    List<Vector3> list;

    //Test
    Vector3 startb;
    Vector3 distanceV;
    Vector3 distanceVNorm;
    float mag;
    Quaternion rot;

    //if the distance of the mouse if great enough from the start gen the blocks

    void Start()
    {
        thisCamera = Camera.main;

    }

    // Update is called once per frame
    void Update()
    {
        if(firstTime == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //start = new Vector3();
                start = Input.mousePosition;
                start.z = (thisCamera.farClipPlane - 5) / 2;
                start = thisCamera.ScreenToWorldPoint(start);
                veryFirstPoint = start;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                //start = new Vector3();
                start = end;
                start.z = (thisCamera.farClipPlane - 5) / 2;
                start = thisCamera.ScreenToWorldPoint(start);
            }
        }


        if(Input.GetMouseButton(0))
        {
            end = Input.mousePosition;
            end.z = (thisCamera.farClipPlane - 5) / 2;
            end = thisCamera.ScreenToWorldPoint(end);
            Debug.DrawLine(start, end, Color.white, 10000f, false);

            GenBlocks();

        }

        if(Input.GetMouseButtonUp(0))
        {
            //startb = new Vector3();
            mag = 0;
            distanceV = new Vector3();
            distanceVNorm = new Vector3();
            i = 0;
        }
    }




    void GenBlocks()
    {
        distanceV = end - start;
        distanceVNorm = Vector3.Normalize(distanceV);
        mag = Mathf.Round(Vector3.Magnitude(distanceV));

        rot = Quaternion.LookRotation(distanceV, Vector3.up);
        while(i < mag)
        {

            startb = startb + (distanceVNorm * 1);
            startb.z = (thisCamera.farClipPlane - 5) / 2;
            GameObject newObj = Instantiate(cube, transform);
            newObj.transform.localPosition = startb;
            newObj.transform.localRotation = rot;
            i++;
        }
    }
}
