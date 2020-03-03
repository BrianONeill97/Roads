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



    public int Floors;

    [Header("Floor Properties: ")]
    public int maxFloorSize;
    public int minFloorSize;

    [Header("Window Properties: ")]
    public int maxWindows;

    int xSize;
    int ySize;
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
       create(new Vector3(0, 0, 0));
    }

    public void create(Vector3 pos)
    {
        GameObject building = new GameObject("Building");

        for (int i = 0; i < Floors; i++)
        {
                CreateFloor(new Vector3(pos.x, pos.y + i * GetSize(wall).y, pos.z),building);
                CreateWalls(new Vector3(pos.x, pos.y + i * GetSize(wall).y, pos.z),building);

            if(i == 0 && doorPresent == false)
            {
                int chanceDoorTile = Random.Range(0, wallTiles.Count);
                GameObject doorObj = Instantiate(Door, building.transform);
                doorObj.transform.localPosition = wallTiles[chanceDoorTile].transform.position;
                doorObj.transform.localRotation = wallTiles[chanceDoorTile].transform.rotation;
                Destroy(wallTiles[chanceDoorTile]);
                wallTiles.RemoveAt(chanceDoorTile);
                wallTiles.Insert(chanceDoorTile,doorObj);
                doorPresent = true;
            }
        }

        building.AddComponent<MeshRenderer>();

        // Box Collider 
        building.AddComponent<BoxCollider>();
        building.GetComponent<BoxCollider>().center = new Vector3((xSize/2) * GetSize(floor).x, (Floors * GetSize(wall).y) /2, (ySize / 2) * GetSize(floor).z);
        building.GetComponent<BoxCollider>().size = new Vector3(xSize * GetSize(floor).x, Floors * GetSize(wall).y, ySize * GetSize(floor).z);

        // Rigidbody 
        building.AddComponent<Rigidbody>();
        building.GetComponent<Rigidbody>().useGravity = false;
        building.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        building.tag = "House";
    }



    void CreateFloor(Vector3 pos,GameObject building)
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

    void CreateWalls(Vector3 pos, GameObject building)
    {
        for (int i = 0; i < xSize; i++)
        {
            for (int z = 0; z < ySize; z++)
            {
                if (i == 0)
                {
                    GameObject wall = Instantiate(WallTypeSelector(), building.transform);
                    wall.transform.localPosition = new Vector3(pos.x + (i * GetSize(floor).x) - GetSize(floor).x / 2, pos.y + GetSize(wall).y / 2, pos.z + (z * GetSize(floor).z));
                    wall.transform.localRotation = Quaternion.identity;
                    wallTiles.Add(wall);
                }
                else if (i == xSize - 1)
                {
                    GameObject wall = Instantiate(WallTypeSelector(), building.transform);
                    wall.transform.localPosition = new Vector3(pos.x + ((i * GetSize(floor).x) + GetSize(floor).x / 2), pos.y + GetSize(wall).y / 2, pos.z + (z * GetSize(floor).z));
                    wall.transform.localRotation = Quaternion.Euler(0, 180, 0);
                    wallTiles.Add(wall);
                }

                if (z == 0)
                {
                    GameObject wall = Instantiate(WallTypeSelector(), building.transform);
                    wall.transform.localPosition = new Vector3(pos.x + (i * GetSize(floor).x), pos.y + GetSize(wall).y / 2, pos.z + ((z * GetSize(floor).z) - GetSize(floor).z / 2));
                    wall.transform.localRotation = Quaternion.Euler(0, 270, 0);
                    wallTiles.Add(wall);
                }
                else if (z == ySize - 1)
                {
                    GameObject wall = Instantiate(WallTypeSelector(), building.transform);
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
