using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Builder : MonoBehaviour
{
    List<GameObject> floorTiles = new List<GameObject>();
    List<GameObject> wallTiles = new List<GameObject>();
    List<GameObject> roofTiles = new List<GameObject>();

    GameObject wall;
    GameObject floor;
    GameObject window;
    GameObject crossWindow;
    GameObject extrudedWindow;
    GameObject windowBay;
    GameObject roof;
    GameObject Door;

    GameObject building;



    public int Floors;

    [Header("Floor Properties: ")]
    public int maxFloorSize;
    public int minFloorSize;

    [Header("Window Properties: ")]
    public int maxWindows;

    int xSize = 0;
    int zSize = 0;
    int windowCount = 0;

    bool doorPresent = false;
    int currentFloor = 0;


    private void Awake()
    {
        Door = Resources.Load("BuildingParts/Country/Door") as GameObject;
        wall = Resources.Load("BuildingParts/Country/Wall") as GameObject;
        window = Resources.Load("BuildingParts/Country/WindowBay") as GameObject;
        crossWindow = Resources.Load("BuildingParts/Country/CrossWindow") as GameObject;
        extrudedWindow = Resources.Load("BuildingParts/Country/ExtrudedWindow") as GameObject;
        windowBay = Resources.Load("BuildingParts/Country/WindowBay") as GameObject;

        floor = Resources.Load("BuildingParts/Cube") as GameObject;
        roof = Resources.Load("BuildingParts/Country/Roof") as GameObject;

    }

    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Building")
        {
            create(new Vector3(99.0f, 0, 32.5f),Quaternion.Euler(0,0,0));
        }
    }

    public void create(Vector3 pos,Quaternion rotation)
    {
        building = new GameObject("Building");
        building.transform.position = pos;
        building.transform.rotation = rotation;
        floorTiles.Clear();
        wallTiles.Clear();
        doorPresent = false;
        currentFloor = 0;
        windowCount = 0;

        for (int i = 0; i <= Floors; i++)
        {
            if(i != Floors)
            {
                CreateFloor(new Vector3(pos.x, pos.y + i * GetSize(wall).y, pos.z));
                CreateWalls(new Vector3(pos.x, pos.y + i * GetSize(wall).y, pos.z));


                if (i == 0 && doorPresent == false)
                {
                    GameObject doorObj = Instantiate(Door, building.transform);
                    doorObj.transform.localPosition = wallTiles[0].transform.localPosition;
                    doorObj.transform.localRotation = wallTiles[0].transform.localRotation;
                    Destroy(wallTiles[0].gameObject);
                    wallTiles.RemoveAt(0);
                    wallTiles.Insert(0, doorObj);
                    doorPresent = true;
                }
            }
            else
            {
                firstRoofSide(new Vector3(pos.x, pos.y + i * GetSize(wall).y, pos.z));
                secondRoofSide(new Vector3(pos.x, pos.y + i * GetSize(wall).y, pos.z));
            }
        }

        building.AddComponent<MeshRenderer>();

        // Box Collider 
        building.AddComponent<BoxCollider>();
        building.GetComponent<BoxCollider>().center = new Vector3(0,(Floors * GetSize(wall).y)/2,0);
        building.GetComponent<BoxCollider>().size = new Vector3(xSize * GetSize(floor).x, Floors * GetSize(wall).y, zSize * GetSize(floor).z);
        building.layer = LayerMask.NameToLayer("BuildingLayer");

        // Rigidbody 
        building.AddComponent<Rigidbody>();
        building.GetComponent<Rigidbody>().useGravity = false;
        building.GetComponent<Rigidbody>().isKinematic = false;
        building.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        building.tag = "House";

        building.AddComponent<ObjectCollisions>();
    }



    void CreateFloor(Vector3 pos)
    {
        xSize = Random.Range(minFloorSize, maxFloorSize);
        zSize = Random.Range(minFloorSize, maxFloorSize);

        for (int i = 0; i < xSize; i++)
        {
            for (int z = 0; z < zSize; z++)
            {
                GameObject tile = Instantiate(floor, building.transform);
                tile.transform.localPosition = new Vector3((i * GetSize(floor).x) - GetSize(floor).x, pos.y, z * GetSize(floor).z - GetSize(floor).z);//new Vector3((i * GetSize(floor).x), pos.y, z * GetSize(floor).z);
                tile.transform.localRotation = Quaternion.identity;
                floorTiles.Add(tile);
            }                
        }
        currentFloor++;
    }

    void CreateWalls(Vector3 pos)
    {
        for (int i = 0; i < xSize; i++)
        {
            for (int z = 0; z < zSize; z++)
            {
                if (i == 0)
                {
                    GameObject wall = Instantiate(WallTypeSelector(), building.transform);
                    wall.transform.localPosition = new Vector3((i * GetSize(floor).x) - GetSize(floor).x * 1.5f, pos.y + GetSize(wall).y / 2,(z * GetSize(floor).z) - GetSize(floor).z);
                    wall.transform.localRotation = Quaternion.identity;
                    wallTiles.Add(wall);
                }
                else if (i == xSize - 1)
                {
                    GameObject wall = Instantiate(WallTypeSelector(), building.transform);
                    wall.transform.localPosition = new Vector3((i * GetSize(floor).x) - GetSize(floor).x / 2, pos.y + GetSize(wall).y / 2, (z * GetSize(floor).z) - GetSize(floor).z);
                    wall.transform.localRotation = Quaternion.Euler(0, 180, 0);
                    wallTiles.Add(wall);
                }

                if (z == 0)
                {
                    GameObject wall = Instantiate(WallTypeSelector(), building.transform);
                    wall.transform.localPosition = new Vector3((i * GetSize(floor).x) - GetSize(floor).x, pos.y + GetSize(wall).y / 2, (z * GetSize(floor).z) - GetSize(floor).z * 1.5f);
                    wall.transform.localRotation = Quaternion.Euler(0, 270, 0);
                    wallTiles.Add(wall);
                }
                else if (z == zSize - 1)
                {
                    GameObject wall = Instantiate(WallTypeSelector(), building.transform);
                    wall.transform.localPosition = new Vector3((i * GetSize(floor).x) - GetSize(floor).x, pos.y + GetSize(wall).y / 2, (z * GetSize(floor).z) - GetSize(floor).z * 0.5f);
                    wall.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    wallTiles.Add(wall);
                }

            }
        }
    }

    void createRoof(Vector3 pos)
    {
        float yOffset = 0.725f;
        for (int i = 0; i < xSize; i++)
        {

            for (int z = 0; z < zSize; z++)
            {                
                if(xSize % 2 ==0) // Its even
                {
                    // Upwards Side
                    if (i < xSize / 2)
                    {
                        GameObject tile = Instantiate(roof, building.transform);
                        tile.transform.localPosition = new Vector3(i * GetSize(floor).x + -GetSize(floor).x,  // offset it to the top of the wall
                                                                    pos.y + (yOffset * i),
                                                                    (z * GetSize(floor).z) - GetSize(floor).z); // (currentTile * size of the floor tile(spacing)) - offset to center the 3 tiles
                        tile.transform.localRotation = Quaternion.Euler(0, 90, 0);
                        roofTiles.Add(tile);
                    }
                    else // Downwards side
                    {
                        GameObject tile = Instantiate(roof, building.transform);
                        tile.transform.localPosition = new Vector3(i * GetSize(floor).x + -GetSize(floor).x,  // offset it to the top of the wall
                                                                    pos.y,
                                                                    (z * GetSize(floor).z) - GetSize(floor).z); // (currentTile * size of the floor tile(spacing)) - offset to center the 3 tiles
                        tile.transform.localRotation = Quaternion.Euler(0, 270, 0);
                        roofTiles.Add(tile);
                    }
                }
                else // its odd
                {
                    //Upwards Side
                    if (i < (xSize / 2))
                    {
                        GameObject tile = Instantiate(roof, building.transform);
                        tile.transform.localPosition = new Vector3(i * GetSize(floor).x + -GetSize(floor).x,  // offset it to the top of the wall
                                                                    pos.y,
                                                                    (z * GetSize(floor).z) - GetSize(floor).z); // (currentTile * size of the floor tile(spacing)) - offset to center the 3 tiles
                        tile.transform.localRotation = Quaternion.Euler(0, 90, 0);
                        roofTiles.Add(tile);
                    }

                    // Top Part
                    if (i == (xSize / 2))
                    {
                        GameObject tile = Instantiate(floor, building.transform);
                        tile.transform.localPosition = new Vector3(i * GetSize(floor).x + -GetSize(floor).x,  // offset it to the top of the wall
                                                                    pos.y + 0.625f, // position + height of a wall
                                                                    (z * GetSize(floor).z) - GetSize(floor).z); // (currentTile * size of the floor tile(spacing)) - offset to center the 3 tiles
                        tile.transform.localRotation = Quaternion.Euler(0, 180, 0);
                        roofTiles.Add(tile);
                    }

                    //Downwards Side
                    if (i >= (xSize / 2) + 1.0f) // rotate to the other
                    {
                        GameObject tile = Instantiate(roof, building.transform);
                        tile.transform.localPosition = new Vector3(i * GetSize(floor).x + -GetSize(floor).x,  // offset it to the top of the wall
                                                                    pos.y,
                                                                    (z * GetSize(floor).z) - GetSize(floor).z); // (currentTile * size of the floor tile(spacing)) - offset to center the 3 tiles
                        tile.transform.localRotation = Quaternion.Euler(0, 270, 0);
                        roofTiles.Add(tile); 
                    }
                }            
            }
        }
    }

    void firstRoofSide(Vector3 pos)
    {
        float offset = 0.725f;
        Vector3 StartingPosition = new Vector3(-GetSize(floor).x * 1.5f, pos.y, -GetSize(floor).z * 1.5f);

        int newSizeX = xSize + 1;
        int newSizeZ = zSize + 1;

        for (int x = 0; x < newSizeX /2; x++)
        {
            for (int z = 0; z < newSizeZ; z++)
            {
                        GameObject tile = Instantiate(roof, building.transform);
                        tile.transform.localPosition = new Vector3( StartingPosition.x + (x * GetSize(floor).x),
                                                                    StartingPosition.y + (x * offset),
                                                                    StartingPosition.z + (z * GetSize(floor).z));
                        tile.transform.localRotation = Quaternion.Euler(0, 90, 0);
                        roofTiles.Add(tile);
            }
        }
    }

    void secondRoofSide(Vector3 pos)
    {
        for (int i = 0; i < xSize; i++)
        {
            for (int z = 0; z < zSize; z++)
            {
                GameObject tile = Instantiate(floor, building.transform);
                tile.transform.localPosition = new Vector3((i * GetSize(floor).x) - GetSize(floor).x, pos.y, z * GetSize(floor).z - GetSize(floor).z);//new Vector3((i * GetSize(floor).x), pos.y, z * GetSize(floor).z);
                tile.transform.localRotation = Quaternion.identity;
                floorTiles.Add(tile);
            }
        }

    }
    

    public Vector3 GetSize(GameObject obj)
    {
        Vector3 dimensions;
        float width = obj.GetComponent<Renderer>().bounds.size.x * transform.localScale.x;
        float height = obj.GetComponent<Renderer>().bounds.size.y * transform.localScale.y;
        float depth = obj.GetComponent<Renderer>().bounds.size.z * transform.localScale.z;
        dimensions = new Vector3(width, height, depth);
        return dimensions;
    } // Gets the ACTUAL size of the tile

    GameObject WallTypeSelector()
    {
        //Wall Type being used ( Changes based on a chance between two two values )
        GameObject wallType = wall;

        //Chance when it is going to be window to be different types
        int wallTypeChance;
        int chance = Random.Range(0, 100);
        if (windowCount < maxWindows)
        {
            if (chance < 75)
            {
                wallType = wall;
            }
            else
            {
                wallTypeChance = Random.Range(0, 4);
                switch(wallTypeChance)
                {
                    case 1:
                        wallType = window;
                        windowCount++;
                        break;
                    case 2:
                        wallType = crossWindow;
                        windowCount++;
                        break;
                    case 3:
                        wallType = crossWindow;
                        windowCount++;
                        break;
                    case 4:
                        wallType = extrudedWindow;
                        windowCount++;
                        break;
                }
            }
        }
        else
        {
            wallType = wall;
        }

        return wallType;
    }
}
