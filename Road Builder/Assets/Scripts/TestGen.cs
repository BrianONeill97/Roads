using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


// ToDo:
// Make it so more roads can be added
// Fill in centre if the road is closed with tiles and populate trees on them (DONE)
//Check if objects are inside each other.
//Add in more roads tiles.
// Maybe fix Parenting issue

public class TestGen : MonoBehaviour
{
    //Maximum Roads
    public int MAX_ROADS = 0;

    //For Turns
    public List<int> Turn;
    public int turn;
    public int turnTwo;
    public int turnThree;
    public int turnFour;

    //Game Objects
    public GameObject Road;
    public GameObject cornerPiece;
    public GameObject Tree;
    public GameObject stump;

    public GameObject plainTile;
    List<GameObject> RoadTiles = new List<GameObject>();

    //For Placing and rotating
    private Vector3 pos;
    Quaternion rot;
    bool isClosed = false;
    bool offsetCornerOne = true;
    bool offsetCornerTwo = true;
    bool offsetCornerThree = true;

    // Start is called before the first frame update

    void Start()
    {
        #if UNITY_EDITOR
        UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        #endif

        GenerateBlock(Road, cornerPiece);

        if (isClosed)
        {
            GenerateCentre();
        }
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
                    Offset(GetSize(cornerPiece).x, 0, GetSize(cornerPiece).z,false);

                }
                if (i == (MAX_ROADS - 1))
                {
                    ChangeRotation(transform, 270);
                }
                RoadTiles.Add(Instantiate(corner, transform.position, transform.rotation));
            }
            else
            {
                 RoadTiles.Add(Instantiate(road, transform.position, transform.rotation) as GameObject);
            }

            if (i == (MAX_ROADS - 1))
            {
                if (GetDistanceBetween(RoadTiles[0].transform.position, RoadTiles[MAX_ROADS - 1].transform.position) <= (GetSize(cornerPiece).z * 2))
                {
                    isClosed = true;
                    
                }
            }
        }
    }

    void Generate(GameObject obj,Vector3 t, Transform rot)
    {
        Instantiate(obj, t, rot.rotation);
    }

    void GenerateCentre()
    {
        int TopLeftTile = (MAX_ROADS / 4) - 1; // get the first tile corner
        int TopRightCorner = ((MAX_ROADS / 4) * 2) - 1;

        Vector3 bottomLeftCornerOfCentre = new Vector3(GetSize(RoadTiles[0]).x,0, GetSize(RoadTiles[0]).z); // Bottom left corner of centre
        Vector3 topLeftPosCentre = new Vector3(RoadTiles[TopLeftTile].transform.position.x, 0, GetSize(RoadTiles[TopLeftTile]).z); //Gets the First turn postion so top left of the centre 
        Vector3 topRightPosCentre = new Vector3(RoadTiles[TopRightCorner].transform.position.x, 0, RoadTiles[TopRightCorner].transform.position.z); // Gets the top right corner of the centre square

        float zCount = Mathf.Floor((GetDistanceBetween(bottomLeftCornerOfCentre, topLeftPosCentre) / GetSize(plainTile).z));   
        float xCount = zCount;


        for (int i = 0; i < xCount; i++)
        {
            for (int j = 0; j < zCount; j++)
            {
               Instantiate(plainTile, new Vector3(bottomLeftCornerOfCentre.x + (i * GetSize(plainTile).x), 0, -bottomLeftCornerOfCentre.z - (j * GetSize(plainTile).z)), Quaternion.identity);
                Vector3 positionForTrees = new Vector3(
                    UnityEngine.Random.Range(bottomLeftCornerOfCentre.x - GetSize(plainTile).x, topLeftPosCentre.x - GetSize(plainTile).x),
                    GetSize(plainTile).y, 
                    UnityEngine.Random.Range(topLeftPosCentre.z - (GetSize(plainTile).z * 2), topRightPosCentre.z + GetSize(plainTile).z)
                    );
                Vector3 positionForStump = new Vector3(
                    UnityEngine.Random.Range(bottomLeftCornerOfCentre.x - GetSize(plainTile).x, topLeftPosCentre.x - GetSize(plainTile).x),
                   GetSize(plainTile).y,
                    UnityEngine.Random.Range(topLeftPosCentre.z - (GetSize(plainTile).z * 2), topRightPosCentre.z + GetSize(plainTile).z)
                    );

               Generate(Tree,positionForTrees, transform);
               Generate(stump, positionForStump, transform);

            }
        }

        isClosed = false;

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
        transform.rotation = rot.rotation;
        return transform.rotation;
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
        transform.position = pos;
        return transform.position;
    }

    float GetDistanceBetween(Vector3 first, Vector3 second)
    {
        float dist = Vector3.Distance(first, second);
        return dist;
    }
}
