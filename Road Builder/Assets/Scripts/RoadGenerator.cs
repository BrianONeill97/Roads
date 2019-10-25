using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    public int Max_Column = 0;
    public int Max_Row = 0;

    List<GameObject> roadSys = new List<GameObject>();

    public GameObject corner;
    public GameObject road;
    private GameObject lastPart;


    public Vector3 spacing;
    private Quaternion rot;

    int next = 0;
    // Start is called before the first frame update

    void Awake()
    {
        for(int i = 0; i < Max_Column; i ++)
        {
            next = Random.Range(0, 100);
            if(next < 50)
            {
                if (lastPart == corner)
                {
                    roadSys.Add(road);
                    //roadSys[i].transform.RotateAround((90, 0, 0);
                    lastPart = road;
                }
                else
                {
                    roadSys.Add(road);
                    lastPart = road;
                }
            }

            if (next > 50)
            {
                if (lastPart == corner)
                {
                    roadSys.Add(road);
                   // roadSys[i].transform.RotateAround(90, 0, 0);
                    lastPart = road;
                }
                else
                {
                    roadSys.Add(corner);
                    lastPart = corner;
                }
            }
            next = 0;

        }
    }
    void Start()
    {
        for (int i = 0; i < Max_Column; i++)
        {
            Instantiate(roadSys[i], roadSys[i].transform.position + (spacing * i), roadSys[i].transform.rotation);
        }

    }
}
