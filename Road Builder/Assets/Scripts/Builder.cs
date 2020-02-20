using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    List<GameObject> floorTiles = new List<GameObject>();
    List<GameObject> wallTiles = new List<GameObject>();

    GameObject wall;
    GameObject floor;
    GameObject window;
    GameObject crossWindow;
    GameObject extrudedWindow;
    GameObject windowBay;
    GameObject roof;
    GameObject Door;


    private GameObject building;

    public int Floors;

    [Header("Floor Properties: ")]
    public int maxFloorSize;
    public int minFloorSize;

    [Header("Window Properties: ")]
    public int minWindowsPerLevel;
    public int maxWindowsPerLevel;

    int xSize;
    int ySize;
    int chance;

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

        //for(int i = 0; i < Floors; i++)
        //{
        //    if (i < Floors - 1)
        //    {
        //        CreateFloor(new Vector3(0, i * GetSize(wall).y, 0));
        //        CreateWalls(new Vector3(0, i * GetSize(wall).y, 0));
        //    }
        //    else
        //    {
        //    CreateRoof(new Vector3(0, (i * GetSize(wall).y) - GetSize(wall).y /2,0));
        //    }
        //}

    }

    public void create(Vector3 pos)
    {
        building = new GameObject("Building");

        for (int i = 0; i < Floors; i++)
        {
                CreateFloor(new Vector3(pos.x, pos.y + i * GetSize(wall).y, pos.z));
                CreateWalls(new Vector3(pos.x, pos.y + i * GetSize(wall).y, pos.z));
        }

        building.AddComponent<MeshRenderer>();

        building.AddComponent<BoxCollider>();

        building.AddComponent<Rigidbody>();
        building.GetComponent<Rigidbody>().useGravity = false;

        building.AddComponent<ObjectCollisions>();

        building.tag = "House";
    }



    void CreateFloor(Vector3 pos)
    {
        xSize = Random.Range(minFloorSize, maxFloorSize);
        ySize = Random.Range(minFloorSize, maxFloorSize);

        for (int i = 0; i < xSize; i++)
        {
            for (int z = 0; z < ySize; z++)
            {
                GameObject tile = Instantiate(floor, building.transform);
                tile.transform.localPosition = new Vector3(pos.x + (i * GetSize(floor).x), pos.y, pos.z + (z * GetSize(floor).z));
                tile.transform.localRotation = Quaternion.identity;
                floorTiles.Add(tile);
            }                
        }
        currentFloor++;
    }

    void CreateWalls(Vector3 pos)
    {
        int chance;
        int wallTypeChance;
        GameObject wallType = wall;
        for (int i = 0; i < xSize; i++)
        {
            for (int z = 0; z < ySize; z++)
            {
                chance = Random.Range(0, 3);
                if (chance != 0)
                {
                    wallType = wall;
                }
                else
                {
                    wallTypeChance = Random.Range(0, 3);
                    if (wallTypeChance == 0)
                    {
                        if (currentFloor == 1 && doorPresent == false)
                        {
                            wallType = Door;

                        }
                        else
                        {
                            wallType = window;
                        }
                    }

                    if(wallTypeChance == 1)
                    {
                        wallType = crossWindow;
                    }

                    if (wallTypeChance == 2)
                    {
                        wallType = extrudedWindow;
                    }

                    if (wallTypeChance == 3)
                    {
                        wallType = windowBay;
                    }
                }

                if (i == 0)
                {
                    GameObject wall = Instantiate(wallType, building.transform);
                    wall.transform.localPosition = new Vector3(pos.x + (i * GetSize(floor).x) - GetSize(floor).x / 2, pos.y + GetSize(wall).y / 2, pos.z + (z * GetSize(floor).z));
                    wall.transform.localRotation = Quaternion.identity;
                    wallTiles.Add(wall);
                }
                else if (i == xSize - 1)
                {
                    GameObject wall = Instantiate(wallType, building.transform);
                    wall.transform.localPosition = new Vector3(pos.x + ((i * GetSize(floor).x) + GetSize(floor).x / 2), pos.y + GetSize(wall).y / 2, pos.z + (z * GetSize(floor).z));
                    wall.transform.localRotation = Quaternion.Euler(0, 180, 0);
                    wallTiles.Add(wall);
                }

                if (z == 0)
                {
                    GameObject wall = Instantiate(wallType, building.transform);
                    wall.transform.localPosition = new Vector3(pos.x + (i * GetSize(floor).x), pos.y + GetSize(wall).y / 2, pos.z + ((z * GetSize(floor).z) - GetSize(floor).z / 2));
                    wall.transform.localRotation = Quaternion.Euler(0, 270, 0);
                    wallTiles.Add(wall);
                }
                else if (z == ySize - 1)
                {
                    GameObject wall = Instantiate(wallType, building.transform);
                    wall.transform.localPosition = new Vector3(pos.x + (i * GetSize(floor).x), pos.y + GetSize(wall).y / 2, pos.z + ((z * GetSize(floor).z) + GetSize(floor).z / 2));
                    wall.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    wallTiles.Add(wall);
                }

            }
        }
    }

    void CreateRoof(Vector3 pos)
    {
        for (int i = 0; i < xSize; i++)
        {
            for (int z = 0; z < ySize; z++)
            {

                if (i == 0)
                {
                    wallTiles.Add(Instantiate(roof, new Vector3(pos.x + (i * GetSize(floor).x) - GetSize(floor).x / 2, pos.y + GetSize(wall).y / 2, pos.z + (z * GetSize(floor).z)), Quaternion.Euler(0, 90, 0)));
                }
                else if (i == xSize - 1)
                {
                    wallTiles.Add(Instantiate(roof, new Vector3(pos.x + ((i * GetSize(floor).x) + GetSize(floor).x / 2), pos.y + GetSize(wall).y / 2, pos.z + (z * GetSize(floor).z)), Quaternion.Euler(0, 270, 0)));
                }

                if (z == 0)
                {
                    wallTiles.Add(Instantiate(roof, new Vector3(pos.x + (i * GetSize(floor).x), pos.y + GetSize(wall).y / 2, pos.z + ((z * GetSize(floor).z) - GetSize(floor).z / 2)), Quaternion.identity));
                }
                else if (z == ySize - 1)
                {
                    wallTiles.Add(Instantiate(roof, new Vector3(pos.x + (i * GetSize(floor).x), pos.y + GetSize(wall).y / 2, pos.z + ((z * GetSize(floor).z) + GetSize(floor).z / 2)), Quaternion.Euler(0, 180, 0)));
                }

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
}
