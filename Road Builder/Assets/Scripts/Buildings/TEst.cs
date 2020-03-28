using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TEst : MonoBehaviour
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

    public int Floors;

    int xSize = 0;
    int zSize = 0;
    int windowCount = 0;

    bool doorPresent = false;
    int currentFloor = 0;

    GameObject building;

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

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Building")
        {
            create(new Vector3(0.0f, 0, 0.0f), Quaternion.Euler(0, 0, 0));
        }
    }

    public void create(Vector3 pos, Quaternion rotation)
    {
        building = new GameObject("Building");
        building.transform.position = pos;
        building.transform.rotation = rotation;

        for (int i = 0; i < Floors; i++)
        {
            CreateFloor(pos);
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

    void CreateFloor(Vector3 t_pos)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int z = 0; z < 3; z++)
            {
                GameObject tile = Instantiate(floor, building.transform);
                tile.transform.localPosition = new Vector3((i * GetSize(floor).x),
                                                            t_pos.y,
                                                            z * GetSize(floor).z);

                tile.transform.localRotation = Quaternion.identity;
                floorTiles.Add(tile);
            }
        }
    }

}
