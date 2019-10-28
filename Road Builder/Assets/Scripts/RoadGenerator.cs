using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    public int MAX_ROADS = 0;

    public int padding;
    int j = 0;

    public int turn;
    public int turnTwo;
    public int turnThree;

    List<GameObject> roadSys = new List<GameObject>();

    //public GameObject corner;
    public GameObject VerticalRoad;
    public GameObject corner;
    private Vector3 pos;
    private Quaternion rot;

    // Start is called before the first frame update

    void Start()
    {
        for (int i = 0; i < MAX_ROADS; i++)
        {
            roadSys.Add(VerticalRoad);

            //When You are any turn the replace the object in the list with a corner object
            if (i == turn || i == turnTwo || i == turnThree)
            {
                roadSys[i] = corner;
                //rot = Quaternion.AngleAxis(90, Vector3.up);
            }

            //Right side of square 
            if (i <= turn)
            {             
                pos = new Vector3(i * padding, pos.y, pos.z);

            }
            //Top of Square
            if (i > turn && i <= turnTwo)
            {
                j++;
                pos = new Vector3(pos.x, pos.y, j * padding);
            }
            //Left side
            if (i > turnTwo && i <= turnThree)
            {
                pos = new Vector3(pos.x = pos.x + -padding, pos.y, pos.z);
            }
            //Bottom side
            if (i > turnThree)
            {
                pos = new Vector3(pos.x, pos.y, pos.z = pos.z + -padding);
            }

            //Creates the Pattern
            Instantiate(roadSys[i], pos, rot);
        }
    }
}
