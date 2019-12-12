using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    public int MAX_ROADS = 0;

    public int turn;
    public int turnTwo;
    public int turnThree;
    public int turnFour;

    bool offsetCornerOne = true;
    bool offsetCornerTwo = true;
    bool offsetCornerThree = true;


    //public GameObject corner;
    public GameObject VerticalRoad;
    public GameObject cornerPiece;
    public GameObject Tree;
    private Vector3 pos;
    Quaternion rot;

    // Start is called before the first frame update

    void Start()
    {
        GetSize(Tree);
        GenerateBlock(VerticalRoad, cornerPiece);
    }

    void GenerateBlock(GameObject road, GameObject corner)
    {
        for (int i = 0; i < MAX_ROADS; i++)
        {
            if (i <= turn)     //Left Side
            {
                ChangeRotation(transform, 0);
                Offset(GetSize(cornerPiece).x, 0, 0, true); //Sets the offset for each to be the size of the gameobject 
            }

            if (i > turn && i <= turnTwo) // Top Side
            {
                //offset to fix rotations Bool is so its only called once
                if (offsetCornerTwo == true)
                {
                    //Sets the offset for each to be the size of the gameobject 
                    Offset(0, 0, GetSize(cornerPiece).z, false);


                    offsetCornerTwo = false;
                }

                ChangeRotation(transform, 90);
                Offset(0, 0, GetSize(cornerPiece).z, false);

            }

            if (i > turnTwo && i <= turnThree) // Right Side
            {
                //offset to fix rotations
                if (offsetCornerOne == true)
                {
                    Offset(0, 0, GetSize(cornerPiece).z, true);

                    offsetCornerOne = false;
                }
                ChangeRotation(transform, 0);
                Offset(GetSize(cornerPiece).x, 0, 0, false);

            }

            if (i > turnThree) // Bottom Side
            {
                //offset to fix rotations
                if (offsetCornerThree == true)
                {
                    Offset(0, 0, GetSize(cornerPiece).z, true);
                    offsetCornerThree = false;
                }
                ChangeRotation(transform, 270);
                Offset(0, 0, GetSize(cornerPiece).z, true);

            }


            //Creates the roads after position and rotation calculations
            if (i == turn || i == turnTwo || i == turnThree || i == turnFour)
            {
                if (i == turnTwo)
                {
                    ChangeRotation(transform, 90);
                }
                if (i == turnThree)
                {
                    ChangeRotation(transform, 180);
                    //offset to fix rotations
                    //Needs to be done to fix the bottom right corner
                    Offset(GetSize(cornerPiece).x, 0, GetSize(cornerPiece).z, false);

                }
                if (i == turnFour)
                {
                    ChangeRotation(transform, 270);
                }

                Instantiate(corner, pos, transform.rotation);
                GenerateTrees(pos, transform);
            }
            else
            {
                Instantiate(road, pos, transform.rotation);
            }
        }
    }

    void GenerateTrees(Vector3 t, Transform rot)
    {
        Instantiate(Tree, t, rot.rotation);
    }

    Vector3 GetSize(GameObject obj)
    {
        Vector3 dimensions;
        float width = obj.GetComponent<Renderer>().bounds.size.x * transform.localScale.x;
        float height = obj.GetComponent<Renderer>().bounds.size.y * transform.localScale.y;
        float depth = obj.GetComponent<Renderer>().bounds.size.z * transform.localScale.z;
        dimensions = new Vector3(width, height, depth);
        return dimensions;
    }

    Quaternion ChangeRotation(Transform rot, int change)
    {
        rot.rotation = Quaternion.Euler(0, change, 0);
        return rot.rotation;
    }

    Vector3 Offset(float xOff, float yOff, float zOff, bool positiveOffset)
    {
        if (positiveOffset == true)
        {
            pos = new Vector3(pos.x + xOff, 0 + yOff, pos.z + zOff);
        }
        else
        {
            pos = new Vector3(pos.x - xOff, 0 - yOff, pos.z - zOff);
        }
        return pos;
    }
}
