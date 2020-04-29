using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TEst : MonoBehaviour
{
    // Overall Building Container
    GameObject Buildings;

    // Floor Lists
    List<GameObject> floorTiles = new List<GameObject>();

    // Wall Tiles
    List<GameObject> wallTiles = new List<GameObject>();

    // Roof Tiles
    List<GameObject> roofTiles = new List<GameObject>();

    // Corners 
    List<GameObject> Corners = new List<GameObject>();

    // Pieces
    GameObject wall;
    GameObject floor;
    GameObject window;
    GameObject crossWindow;
    GameObject extrudedWindow;
    GameObject windowBay;
    GameObject roof;
    GameObject Door;
    GameObject Fence;
    GameObject mailbox;
    GameObject pathTile;
    GameObject indoorLight;

    //Properties
    int Floors;
    //[HideInInspector]
    public Vector2Int Size;
    Vector3 position;


    private void Awake()
    {
        wall = Resources.Load("BuildingParts/Wall") as GameObject;
        floor = Resources.Load("BuildingParts/Cube") as GameObject;
        roof = Resources.Load("BuildingParts/Roof") as GameObject;
        Fence = Resources.Load("BuildingParts/Garden/Fence") as GameObject;
        mailbox = Resources.Load("BuildingParts/Garden/Mailbox") as GameObject;
        pathTile = Resources.Load("PathTile") as GameObject;
        indoorLight = Resources.Load("IndoorLight") as GameObject;
    }

    void Start()
    {
        Buildings = new GameObject("Buildings");
        if (SceneManager.GetActiveScene().name == "Building")
        {
            createBuilding(new Vector3(position.x, position.y + (Floors * Utility.GetSize(wall).y) / 2.0f, position.z), Quaternion.Euler(0, 0, 0));

            //Corners[0].GetComponent<Renderer>().material.color = Color.red;
            //Corners[1].GetComponent<Renderer>().material.color = Color.green;
            //Corners[2].GetComponent<Renderer>().material.color = Color.yellow;
            //Corners[3].GetComponent<Renderer>().material.color = Color.blue;
        }
        else
        {
            Buildings.transform.SetParent(GameObject.Find("Track").transform);
        }
    }

    public void createBuilding(Vector3 pos, Quaternion rotation)
    {
        Floors = Random.Range(1, 5);
        Size.x = Random.Range(3, 6);
        Size.y = Random.Range(3, 6);
        GameObject building = new GameObject("Building"); // Creates the new building object
        EmptyLists();  // Empties all the previous containers
        building.transform.SetParent(Buildings.transform);
        for (int i = 0; i <= Floors; i++)
        {
            if (i < Floors)
            {
                GameObject m_current_room = new GameObject("Floor" + i.ToString()); // Create the coom you are making
                m_current_room.transform.parent = building.transform; // add the room as a child of the parent
                CreateFloor(m_current_room, Size, i); // Creates the floor for the room
                CreateWalls(m_current_room, Size, i); // Creates the walls for the room

                if (i != 0)
                {
                    Utility.combineMesh(m_current_room);
                }

                if(i == Floors -1)
                {
                    CreateHouseLight(m_current_room,i + 1);
                }
            }
            else if(i == Floors)
            {

                CreateRoof(building, i);
            }
        }

        CreateGarden(building);
        CreateMailbox(building);
        CreatePath(building);
        AddBuildingComponents(building, pos, rotation); // Adds the buildings RB,Collider and other components
    }

    void CreateFloor(GameObject t_room, Vector2Int t_size, int currentFloor)
    {
        Vector2 offset = new Vector2(t_size.x, t_size.y) / 2.0f;

        for (int i = 0; i < t_size.x; i++)
        {
            for (int z = 0; z < t_size.y; z++)
            {
                GameObject tile = Instantiate(floor, t_room.transform);
                tile.transform.localPosition = new Vector3((i * Utility.GetSize(floor).x) - offset.x,
                                                            currentFloor * Utility.GetSize(wall).y,
                                                            (z * Utility.GetSize(floor).z) - offset.y);

                tile.transform.localRotation = Quaternion.identity;
                floorTiles.Add(tile);
            }
        }

        Corners.Add(floorTiles[0]);
        Corners.Add(floorTiles[Size.y - 1]);
        Corners.Add(floorTiles[floorTiles.Count - 1]);
        Corners.Add(floorTiles[floorTiles.Count - Size.y]);
    }

    void CreateWalls(GameObject t_room, Vector2Int t_size, int currentFloor)
    {
        Vector2 offset = new Vector2(t_size.x, t_size.y) / 2.0f;

        for (int i = 0; i < t_size.x; i++)
        {
            for (int z = 0; z < t_size.y; z++)
            {
                if (i == 0)
                {
                    GameObject tile = Instantiate(WallTypeSelector(), t_room.transform);
                    tile.transform.localPosition = new Vector3(offset.x - Utility.GetSize(floor).x / 2.0f,
                                                                ((currentFloor * Utility.GetSize(wall).y)) + (Utility.GetSize(wall).y / 2.0f),
                                                                z * Utility.GetSize(floor).z - (offset.y * Utility.GetSize(floor).z));

                    tile.transform.localRotation = Quaternion.Euler(0, 180, 0);
                    wallTiles.Add(tile);
                }
                else if (i == t_size.x - 1)
                {
                    GameObject tile = Instantiate(WallTypeSelector(), t_room.transform);
                    tile.transform.localPosition = new Vector3(-offset.x - Utility.GetSize(floor).x / 2.0f,
                                                                ((currentFloor * Utility.GetSize(wall).y)) + (Utility.GetSize(wall).y / 2.0f),
                                                                z * Utility.GetSize(floor).z - (offset.y * Utility.GetSize(floor).z));

                    tile.transform.localRotation = Quaternion.identity;
                    wallTiles.Add(tile);
                }

                if (z == 0)
                {
                    GameObject tile = Instantiate(WallTypeSelector(), t_room.transform);
                    tile.transform.localPosition = new Vector3(i * Utility.GetSize(floor).x - (offset.x * Utility.GetSize(floor).x),
                                                                ((currentFloor * Utility.GetSize(wall).y)) + (Utility.GetSize(wall).y / 2.0f),
                                                                -Utility.GetSize(floor).x / 2.0f - (offset.y * Utility.GetSize(floor).z));

                    tile.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    wallTiles.Add(tile);
                }
                else if (z == t_size.y - 1)
                {
                    GameObject tile = Instantiate(WallTypeSelector(), t_room.transform);
                    tile.transform.localPosition = new Vector3(i * Utility.GetSize(floor).x - (offset.x * Utility.GetSize(floor).x),
                                                                ((currentFloor * Utility.GetSize(wall).y)) + (Utility.GetSize(wall).y / 2.0f),
                                                                -Utility.GetSize(floor).x / 2.0f + (offset.y * Utility.GetSize(floor).z));

                    tile.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    wallTiles.Add(tile);
                }
            }
        }

        if (currentFloor == 0)
        {
            // this is a list of the potential places for the door
            List<GameObject> DoorCandidates = new List<GameObject>();
            for (int k = 0; k < wallTiles.Count; k++)
            {
                if (wallTiles[k].transform.localRotation == Quaternion.identity)
                {
                    // if the wall is facing a specific direction
                    DoorCandidates.Add(wallTiles[k]);
                }
            }
            //Selects the door randomly across the door candidtates
            int num = Random.Range(0, DoorCandidates.Count);
            Destroy(DoorCandidates[num].gameObject);
            GameObject m_door = Instantiate(Door, t_room.transform);
            m_door.transform.localPosition = DoorCandidates[num].transform.localPosition;
            m_door.transform.localRotation = DoorCandidates[num].transform.localRotation;
            DoorCandidates.Insert(num, m_door);
        } //This is used for selecting the possible door spawn
    }

    void CreateRoof(GameObject building, int currentFloor)
    {
        // num for roof
        int num = 99;

        // Mesh and Roof Pieces
        GameObject m_roof = new GameObject("Roof");
        m_roof.transform.parent = building.transform;

        MeshRenderer meshRenderer = m_roof.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Particles/Standard Surface"));

        MeshFilter meshFilter = m_roof.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();

        // Getting the Corners of the building
        Vector3 BottomLeftCorner = new Vector3(Corners[1].transform.localPosition.x - Utility.GetSize(floor).x, currentFloor * Utility.GetSize(wall).y, Corners[1].transform.localPosition.z + Utility.GetSize(floor).x);
        Vector3 BottomRightCorner = new Vector3(Corners[0].transform.localPosition.x - Utility.GetSize(floor).x, currentFloor * Utility.GetSize(wall).y, Corners[0].transform.localPosition.z - Utility.GetSize(floor).x);
        Vector3 TopLeftCorner = new Vector3(Corners[2].transform.localPosition.x + Utility.GetSize(floor).x, currentFloor * Utility.GetSize(wall).y, Corners[2].transform.localPosition.z + Utility.GetSize(floor).x);
        Vector3 TopRightCorner = new Vector3(Corners[3].transform.localPosition.x + Utility.GetSize(floor).x, currentFloor * Utility.GetSize(wall).y, Corners[3].transform.localPosition.z - Utility.GetSize(floor).x);
        float commonHeight = BottomLeftCorner.y;

        // Chose the roof depeding on the settings
        if (SceneManager.GetActiveScene().name != "Building")
        {
            num = Random.Range(0, 7);
            if (num == 0)
            {
                BasicRoof(mesh, BottomLeftCorner, BottomRightCorner, TopLeftCorner, TopRightCorner, commonHeight);
            }
            else if (num == 1)
            {
                slantedRoofSmall(mesh, BottomLeftCorner, BottomRightCorner, TopLeftCorner, TopRightCorner, commonHeight);
            }
            else if (num == 2)
            {
                flatRoof(mesh, BottomLeftCorner, BottomRightCorner, TopLeftCorner, TopRightCorner, commonHeight);
            }
            else if(num == 3)
            {
                pyramidRoof(mesh, BottomLeftCorner, BottomRightCorner, TopLeftCorner, TopRightCorner, commonHeight);
            }
            else if (num == 4)
            {
                barnRoof(mesh, BottomLeftCorner, BottomRightCorner, TopLeftCorner, TopRightCorner, commonHeight);
            }
            else if (num == 5)
            {
                RoundedRoof(mesh, BottomLeftCorner, BottomRightCorner, TopLeftCorner, TopRightCorner, commonHeight);
            }
            else if (num == 6)
            {
                twoTierRoof(mesh, BottomLeftCorner, BottomRightCorner, TopLeftCorner, TopRightCorner, commonHeight);
            }

        }
        else
        {
            BasicRoof(mesh, BottomLeftCorner, BottomRightCorner, TopLeftCorner, TopRightCorner, commonHeight);
        }

        meshFilter.mesh = mesh;     

        Vector3 midPoint = new Vector3(((TopRightCorner.x + TopLeftCorner.x) / 2),BottomLeftCorner.y, ((TopRightCorner.z + TopLeftCorner.z) / 2));

        if(num == 0)
        {
            DormerWindow(building, new Vector3(Random.Range(Corners[1].transform.localPosition.x + 0.5f, Corners[2].transform.localPosition.x - 0.5f),
                                                BottomLeftCorner.y,
                                                BottomLeftCorner.z - Size.y * 1.0f),
                                                Quaternion.Euler(0, 0, 0));

            DormerWindow(building, new Vector3(Random.Range(Corners[1].transform.localPosition.x + 0.5f, Corners[2].transform.localPosition.x - 0.5f),
                                                BottomLeftCorner.y,
                                                BottomLeftCorner.z - Size.y / 1.2f),
                                                Quaternion.Euler(0, 180, 0));

        }
        else
        {
            //Chimney(building, new Vector3(Random.Range(BottomLeftCorner.x, TopRightCorner.x),
            //                                BottomLeftCorner.y,
            //                                Random.Range(BottomLeftCorner.z, TopRightCorner.z)));
        }

    }

    //Roof Types
    void BasicRoof(Mesh mesh, Vector3 BottomLeft, Vector3 BottomRight, Vector3 TopLeft, Vector3 TopRight, float height)
    {
        mesh.vertices = new Vector3[6]
        {
            //Base of the roof
            BottomLeft, // 0
            new Vector3((BottomRight.x + BottomLeft.x) /2, height + 1.0f, (BottomRight.z + BottomLeft.z) /2), // 1            
            TopLeft, // 2
            new Vector3((TopRight.x + TopLeft.x) /2, height+ 1.0f, (TopRight.z + TopLeft.z) /2), // 3
            BottomRight, // 4 
            TopRight // 5
        };
        mesh.triangles = new int[24]
        {
            //First Rect
            0, 2, 1,
            2, 3, 1,
            // Second Rect
            1,3,4,
            3,5,4,
            //Side Rect
            2,5,3,
            1,4,0,

            //Bottom
            5,0,4,
            5,2,0

        };


        int[] m_triangles = mesh.triangles;
        Vector3[] m_vertices = mesh.vertices;

        Vector3[] verticesModif = new Vector3[m_triangles.Length];
        int[] trianglesModif = new int[m_triangles.Length];

        Color32 currentColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);
        Color32[] myColors = new Color32[m_triangles.Length];

        for (int i = 0; i < trianglesModif.Length; i++)
        {
            verticesModif[i] = m_vertices[m_triangles[i]];
            trianglesModif[i] = i;

            if (i == 12)
            {
                currentColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);
            }
            myColors[i] = currentColor;
        }

        mesh.vertices = verticesModif;
        mesh.triangles = trianglesModif;
        mesh.colors32 = myColors;

    }
    void RoundedRoof(Mesh mesh, Vector3 BottomLeft, Vector3 BottomRight, Vector3 TopLeft, Vector3 TopRight, float height)
    {
        float tier0Height = height;
        float tier1Height = height + 0.5f;
        // Left To right 
        int m_width = (int)((BottomRight.x + BottomLeft.x) / 2); // Along X axis
        int m_depth = (int)((TopLeft.z + BottomLeft.z) / 2); // Along Z axis

        mesh.vertices = new Vector3[16]
        {
            BottomLeft,// 0
            new Vector3(BottomLeft.x,tier0Height,BottomLeft.z + (m_width * 0.2f)),// 1
            new Vector3(BottomLeft.x + (m_depth * 0.2f),tier1Height,BottomLeft.z + (m_width * 0.2f)),// 2
            new Vector3(BottomRight.x,tier0Height,BottomRight.z - (m_width * 0.2f)),// 3
            new Vector3(BottomRight.x - (m_width * 0.2f),tier1Height,BottomRight.z - (m_width * 0.2f)),// 4
            
            BottomRight,// 5

            new Vector3(BottomLeft.x + (m_depth * 0.2f),tier0Height,BottomLeft.z),// 6
            new Vector3(TopLeft.x + (m_width * 0.2f),tier1Height, TopLeft.z + (m_width * 0.2f)),// 7
            new Vector3(TopLeft.x + (m_width * 0.2f),tier0Height, TopLeft.z),// 8 

            TopLeft, // 9

            new Vector3(TopLeft.x,tier0Height,TopLeft.z + (m_width * 0.2f)),// 10
            new Vector3(TopRight.x - (m_depth * 0.2f),tier1Height,TopRight.z - (m_width * 0.2f)),// 11
            new Vector3(TopRight.x,tier0Height,TopRight.z - (m_width * 0.2f)),// 12

            TopRight,// 13

            new Vector3(TopRight.x- (m_depth * 0.2f),tier0Height,TopRight.z),// 14
            new Vector3(BottomRight.x - (m_width * 0.2f),tier0Height,BottomRight.z)// 15
        };
        mesh.triangles = new int[60]
        {
            //Bottom
            0, 2, 1,
            1, 2, 3,
            3,2,4,
            3,4,5,

            //Side
            0,6,2,
            2,6,7,
            6,8,7,
            8,9,7,

            //// Top
            7,9,10,
            10,12,11,
            10,11,7,
            11,12,13,

            ////Other Side
            13,14,11,
            14,4,11,
            14,15,4,
            15,5,4,

            // Top Cover 
            4,2,7,
            7,11,4,

            // ( Reverse since it will display on the wrong side otherwise )
            //Under
            13,0,5,
            0,13,9
        };

        int[] m_triangles = mesh.triangles;
        Vector3[] m_vertices = mesh.vertices;

        Vector3[] verticesModif = new Vector3[m_triangles.Length];
        int[] trianglesModif = new int[m_triangles.Length];

        Color32 currentColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);
        Color32[] myColors = new Color32[m_triangles.Length];

        for (int i = 0; i < trianglesModif.Length; i++)
        {
            verticesModif[i] = m_vertices[m_triangles[i]];
            trianglesModif[i] = i;

            if (i == 48)
            {
                currentColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);
            }

            myColors[i] = currentColor;
        }

        mesh.vertices = verticesModif;
        mesh.triangles = trianglesModif;
        mesh.colors32 = myColors;
    }
    void slantedRoofSmall(Mesh mesh, Vector3 BottomLeft, Vector3 BottomRight, Vector3 TopLeft, Vector3 TopRight, float height)
    {
        mesh.vertices = new Vector3[6]
        {
            // Front Triangle
            BottomLeft, // 0
            new Vector3(BottomLeft.x,   height + 0.5f, BottomLeft.z),// 1 // Top Corner
            new Vector3(BottomRight.x,   height,    BottomRight.z ),// 2 // Top Corner        

            // Back Triangle
            TopLeft, // 3
            new Vector3(TopLeft.x,   height + + 0.5f, TopLeft.z),// 4 // Top Corner
            new Vector3(TopRight.x,   height,    TopRight.z ),// 5 // Top Corner  

        };

        mesh.triangles = new int[24]
        {
            //Front Triangle
            1,2,0,
            // Back triangle
            3,5,4,
            // Side Plane
            0,3,4,
            4,1,0,
            // Top plane
            2,1,5,
            5,1,4,
            // Bottom Plane
            0,2,5,
            5,3,0
        };

        int[] m_triangles = mesh.triangles;
        Vector3[] m_vertices = mesh.vertices;

        Vector3[] verticesModif = new Vector3[m_triangles.Length];
        int[] trianglesModif = new int[m_triangles.Length];

        Color32 currentColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);
        Color32[] myColors = new Color32[m_triangles.Length];

        for (int i = 0; i < trianglesModif.Length; i++)
        {
            verticesModif[i] = m_vertices[m_triangles[i]];
            trianglesModif[i] = i;

            if (i  == 12)
            {
                currentColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);
            }

            myColors[i] = currentColor;
        }

        mesh.vertices = verticesModif;
        mesh.triangles = trianglesModif;
        mesh.colors32 = myColors;
    }
    void pyramidRoof(Mesh mesh, Vector3 BottomLeft, Vector3 BottomRight, Vector3 TopLeft, Vector3 TopRight, float height)
    {
        Vector3 midPoint = new Vector3(((BottomLeft.x + TopRight.x) / 2), height + 1, ((BottomLeft.z + TopRight.z) / 2));

        mesh.vertices = new Vector3[5]
        {
            // Front Triangle
            BottomLeft, // 0
            BottomRight,// 1 // Top Corner
            TopLeft,// 2 // Top Corner        
            TopRight, // 3
            midPoint// 4
 

        };

        mesh.triangles = new int[18]
        {
            //Pyramid
            0,4,1,
            1,4,3,
            3,4,2,
            2,4,0,
            // Bottom Plane
            1,3,0,
            3,2,0
        };


        int[] m_triangles = mesh.triangles;
        Vector3[] m_vertices = mesh.vertices;

        Vector3[] verticesModif = new Vector3[m_triangles.Length];
        int[] trianglesModif = new int[m_triangles.Length];

        Color32 currentColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);
        Color32[] myColors = new Color32[m_triangles.Length];

        for (int i = 0; i < trianglesModif.Length; i++)
        {
            verticesModif[i] = m_vertices[m_triangles[i]];
            trianglesModif[i] = i;

            if (i % 3 == 0)
            {
                currentColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);
            }

            myColors[i] = currentColor;
        }

        mesh.vertices = verticesModif;
        mesh.triangles = trianglesModif;
        mesh.colors32 = myColors;
    }
    void flatRoof(Mesh mesh, Vector3 BottomLeft, Vector3 BottomRight, Vector3 TopLeft, Vector3 TopRight, float height)
    {
        mesh.vertices = new Vector3[8]
       {
            // Front Triangle
            BottomLeft, // 0
            BottomRight,// 1 
            TopLeft,// 2      
            TopRight, // 3
            new Vector3(BottomLeft.x,   height + 0.25f,   BottomLeft.z), // 4
            new Vector3(BottomRight.x,  height + 0.25f,  BottomRight.z),// 5 
            new Vector3(TopLeft.x,  height + 0.25f,  TopLeft.z),// 6      
            new Vector3(TopRight.x, height + 0.25f, TopRight.z), // 7



       };

        mesh.triangles = new int[36]
        {        
            //Sides
            0,4,1,
            4,5,1,

            2,6,0,
            6,4,0,

            3,6,2,
            3,7,6,

            1,5,3,
            5,7,3,

            //BottomPlane
            1,2,0,
            1,3,2,

            // Top plane
            4,7,5,
            4,6,7
        };


        int[] m_triangles = mesh.triangles;
        Vector3[] m_vertices = mesh.vertices;

        Vector3[] verticesModif = new Vector3[m_triangles.Length];
        int[] trianglesModif = new int[m_triangles.Length];

        Color32 currentColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);
        Color32[] myColors = new Color32[m_triangles.Length];

        for (int i = 0; i < trianglesModif.Length; i++)
        {
            verticesModif[i] = m_vertices[m_triangles[i]];
            trianglesModif[i] = i;

            if (i  == 24)
            {
                currentColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);
            }

            myColors[i] = currentColor;
        }

        mesh.vertices = verticesModif;
        mesh.triangles = trianglesModif;
        mesh.colors32 = myColors;
    }
    void twoTierRoof(Mesh mesh, Vector3 BottomLeft, Vector3 BottomRight, Vector3 TopLeft, Vector3 TopRight, float height)
    {
        // Heights
        float firstHeight = height + 0.5f;
        float secondHeight = firstHeight + 1.0f;
        // Midpoint of all the corners
        Vector3 midPoint = new Vector3(((BottomLeft.x + TopRight.x) / 2), height + 1, ((BottomLeft.z + TopRight.z) / 2));
        // 1st tier height
        Vector3 botLeftRaisedCorner = new Vector3(((BottomLeft.x + midPoint.x) / 2), firstHeight, ((BottomLeft.z + midPoint.z) / 2));
        Vector3 botRightRaisedCorner = new Vector3(((BottomRight.x + midPoint.x) / 2), firstHeight, ((BottomRight.z + midPoint.z) / 2));
        Vector3 topLeftRaisedCorner = new Vector3(((TopLeft.x + midPoint.x) / 2), firstHeight, ((TopLeft.z + midPoint.z) / 2));
        Vector3 topRightRaisedCorner = new Vector3(((TopRight.x + midPoint.x) / 2), firstHeight, ((TopRight.z + midPoint.z) / 2));

        // Meshes Variables
        mesh.vertices = new Vector3[9]
        {
            BottomLeft,// 0
            botLeftRaisedCorner,// 1
            botRightRaisedCorner,// 2        
            BottomRight,// 3
            TopLeft,// 4
            topLeftRaisedCorner,// 5
            topRightRaisedCorner,// 6
            TopRight,// 7
            new Vector3(midPoint.x,secondHeight,midPoint.z) // 8
        };
        mesh.triangles = new int[42]
        {
            //Pyramid
            0,1,2,
            2,3,0,
            6,5,4,
            4,7,6,
            4,5,1,
            1,0,4,
            3,2,6,
            6,7,3,
            1,8,2,
            5,8,1,
            5,6,8,
            2,8,6,
            7,4,0,
            0,3,7
        };

        // Mesh Colors
        int[] m_triangles = mesh.triangles;
        Vector3[] m_vertices = mesh.vertices;
        Vector3[] verticesModif = new Vector3[m_triangles.Length];
        int[] trianglesModif = new int[m_triangles.Length];
        Color32 currentColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);
        Color32[] myColors = new Color32[m_triangles.Length];

        for (int i = 0; i < trianglesModif.Length; i++)
        {
            verticesModif[i] = m_vertices[m_triangles[i]];
            trianglesModif[i] = i;

            if (i == 24)
            {
                currentColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);
            }
            myColors[i] = currentColor;
        }

        mesh.vertices = verticesModif;
        mesh.triangles = trianglesModif;
        mesh.colors32 = myColors;
    }
    void barnRoof(Mesh mesh, Vector3 BottomLeft, Vector3 BottomRight, Vector3 TopLeft, Vector3 TopRight, float height)
    {
        // Height Tiers
        float firstTier = height + 0.6f;
        float secondTier = height + 0.8f;

        Vector3 midPoint = new Vector3(0, 0, ((BottomLeft.z + TopRight.z) / 2));
        Vector3 halfMidLeft =   new Vector3(0, 0, ((BottomLeft.z + midPoint.z) / 2));
        Vector3 halfMidRight = new Vector3(0, 0, ((BottomRight.z + midPoint.z) / 2));

        mesh.vertices = new Vector3[12]
        {
            // Left Side
            BottomLeft, // 0
            new Vector3(TopLeft.x,firstTier,halfMidLeft.z), // 1
            TopLeft, // 2
            new Vector3(BottomLeft.x,firstTier,halfMidLeft.z), // 3
            new Vector3(TopLeft.x,secondTier,midPoint.z), // 4
            new Vector3(BottomLeft.x,secondTier,midPoint.z), // 5

            // Right Side
            new Vector3(TopLeft.x,firstTier,halfMidRight.z), // 6
            new Vector3(BottomLeft.x,firstTier,halfMidRight.z), // 7
            TopRight, // 8
            BottomRight, // 9

            // Sides
            new Vector3(BottomLeft.x,BottomLeft.y,midPoint.z), // 10
            new Vector3(TopLeft.x,BottomLeft.y,midPoint.z), // 11

        };
        mesh.triangles = new int[54]
        {
            // Top 
            0,2,1,
            1,3,0,
            3,1,4,
            4,5,3,
            5,4,6,
            6,7,5,
            7,6,8,
            8,9,7,

            // Sides
            0,3,10,
            3,5,10,
            10,5,7,
            10,7,9,
            11,1,2,
            11,4,1,
            6,4,11,
            8,6,11,

            // Bottom
            8,2,0,
            0,9,8
        };


        int[] m_triangles = mesh.triangles;
        Vector3[] m_vertices = mesh.vertices;

        Vector3[] verticesModif = new Vector3[m_triangles.Length];
        int[] trianglesModif = new int[m_triangles.Length];

        Color32 currentColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);
        Color32[] myColors = new Color32[m_triangles.Length];

        for (int i = 0; i < trianglesModif.Length; i++)
        {
            verticesModif[i] = m_vertices[m_triangles[i]];
            trianglesModif[i] = i;

            if(i == 24)
            {
                currentColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);
            }
            myColors[i] = currentColor;
        }

        mesh.vertices = verticesModif;
        mesh.triangles = trianglesModif;
        mesh.colors32 = myColors;

    }


    // Roof Parts
    void Chimney(GameObject building, Vector3 position)
    {
        List<Vector3> m_vertices = new List<Vector3>();
        float size = 0.25f;
        GameObject m_Chimney = new GameObject("Chimney");
        m_Chimney.transform.parent = building.transform;

        MeshRenderer meshRenderer = m_Chimney.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = Resources.Load("BuildingParts/Materials/Roof") as Material;

        meshRenderer.material.color = Color.gray;

        MeshFilter meshFilter = m_Chimney.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();


        // Cube 1
        m_vertices.Add(new Vector3(position.x + size, position.y, position.z - size)); // 0 
        m_vertices.Add(new Vector3(position.x - size, position.y, position.z - size)); // 1 
        m_vertices.Add(new Vector3(position.x - size, position.y + size * 2, position.z - size)); // 2 
        m_vertices.Add(new Vector3(position.x + size, position.y + size * 2, position.z - size)); // 3
        m_vertices.Add(new Vector3(position.x + size, position.y + size * 2, position.z + size)); // 4
        m_vertices.Add(new Vector3(position.x + size, position.y, position.z + size)); // 5
        m_vertices.Add(new Vector3(position.x - size, position.y + size * 2, position.z + size)); // 6
        m_vertices.Add(new Vector3(position.x - size, position.y, position.z + size)); // 7

        // Cube 2
        m_vertices.Add(new Vector3(position.x + size, position.y + size * 2, position.z - size)); // 8
        m_vertices.Add(new Vector3(position.x - size, position.y + size * 2, position.z - size)); // 9 
        m_vertices.Add(new Vector3(position.x - size, position.y + size * 4, position.z - size)); // 10 
        m_vertices.Add(new Vector3(position.x + size, position.y + size * 4, position.z - size)); // 11
        m_vertices.Add(new Vector3(position.x + size, position.y + size * 4, position.z + size)); // 12 // 
        m_vertices.Add(new Vector3(position.x + size, position.y + size * 2, position.z + size)); // 13
        m_vertices.Add(new Vector3(position.x - size, position.y + size * 4, position.z + size)); // 14
        m_vertices.Add(new Vector3(position.x - size, position.y + size * 2, position.z + size)); // 15

        mesh.vertices = m_vertices.ToArray();
        mesh.triangles = new int[54]
        {
            // Cube 1
            0,1,3,
            1,2,3,
            5,0,4,
            0,3,4,
            7,5,6,
            5,4,6,
            1,7,2,
            7,6,2,

            // Cube 2
            8,9,11,
            9,10,11,
            13,8,12,
            8,11,12,
            15,13,14,
            13,12,14,
            9,15,10,
            15,14,10,

            // Cube 3
            11,10,12,
            10,14,12
        };

        // Set the normals
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
    void DormerWindow(GameObject building, Vector3 position, Quaternion t_rotation)
    {
        List<Vector3> m_points = new List<Vector3>();
        float size = 0.25f;
        GameObject dormerWindow = new GameObject("Window");
        dormerWindow.transform.parent = building.transform;

        MeshRenderer meshRenderer = dormerWindow.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Particles/Standard Surface"));

        MeshFilter meshFilter = dormerWindow.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();


        // Front Face
        m_points.Add(new Vector3(position.x + size, position.y, position.z - size)); // 0 
        m_points.Add(new Vector3(position.x - size, position.y, position.z - size)); // 1 
        m_points.Add(new Vector3(position.x - size, position.y + (size * 2), position.z - size)); // 2 
        m_points.Add(new Vector3(position.x + size, position.y + (size * 2), position.z - size)); // 3
        m_points.Add(new Vector3((((position.x - size) + (position.x + size)) / 2), position.y + size * 3, position.z - size)); // 4 Mid Of Front face
        m_points.Add(new Vector3(position.x - size, position.y, (position.z + (size * 1.5f)))); // 5
        m_points.Add(new Vector3(position.x - size, position.y + (size * 2), (position.z + (size * 2.5f)))); // 6 
        m_points.Add(new Vector3(position.x + size, position.y, (position.z + (size * 1.5f)))); // 7
        m_points.Add(new Vector3(position.x + size, position.y + (size * 2), (position.z + (size * 2.5f)))); // 8 
        m_points.Add(new Vector3((((position.x - size) + (position.x + size)) / 2), position.y + size * 3, (position.z + (size * 8.0f)))); // 9 Mid Of Back face

        // Top Front Points 
        m_points.Add(new Vector3(position.x - size, position.y + size * 2, (position.z - (size * 1.5f)))); // 10 
        m_points.Add(new Vector3(position.x + size, position.y + size * 2, (position.z - (size * 1.5f)))); // 11
        m_points.Add(new Vector3((((position.x - size) + (position.x + size)) / 2), position.y + size * 3, (position.z - (size * 1.5f)))); // 12



        mesh.vertices = m_points.ToArray();
        mesh.triangles = new int[45]
        {
            // Cube 1
            0,1,2,
            2,3,0,
            2,4,3,

            // Sides
            1,5,6,
            6,2,1,
            0,3,7,
            3,8,7,

        // Top
            // Top
            10,6,9,
            9,12,10,
            8,11,12,
            12,9,8,
            // Under
            9,6,10,
            10,12,9,
            12,11,8,
            8,9,12,

        };

        int[] m_triangles = mesh.triangles;
        Vector3[] m_vertices = mesh.vertices;

        Vector3[] verticesModif = new Vector3[m_triangles.Length];
        int[] trianglesModif = new int[m_triangles.Length];

        Color32 currentColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);
        Color32[] myColors = new Color32[m_triangles.Length];

        for (int i = 0; i < trianglesModif.Length; i++)
        {
            verticesModif[i] = m_vertices[m_triangles[i]];
            trianglesModif[i] = i;

            if (i == 9)
            {
                currentColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);
            }
            else if (i == 21)
            {
                currentColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);
            }
            myColors[i] = currentColor;
        }

        mesh.vertices = verticesModif;
        mesh.triangles = trianglesModif;
        mesh.colors32 = myColors;

        meshFilter.mesh = mesh;

        dormerWindow.transform.localRotation = t_rotation;
    }

    //Garden
    void CreateMailbox(GameObject t_building)
    {
        Vector3 doorPos;
        GameObject firstFloor = t_building.transform.GetChild(0).gameObject;
        for (int i = 0; i < firstFloor.transform.childCount; i++)
        {
            if (firstFloor.transform.GetChild(i).name.Contains("Door"))
            {
                doorPos = firstFloor.transform.GetChild(i).transform.localPosition;
                GameObject Tree = Instantiate(mailbox, t_building.transform);
                Tree.transform.localPosition = new Vector3(doorPos.x - 0.2f, 0, doorPos.z + 0.4f);
                break;
            }
        }
    }
    void CreateGarden(GameObject building)
    {
        GameObject m_garden = new GameObject("Garden");
        m_garden.transform.parent = building.transform;
        Vector3 TopRightCorner = new Vector3(Corners[3].transform.localPosition.x + Utility.GetSize(floor).x, 0, Corners[3].transform.localPosition.z - Utility.GetSize(floor).x);
        Vector3 TopLeftCorner = new Vector3(Corners[2].transform.localPosition.x + Utility.GetSize(floor).x, 0, Corners[2].transform.localPosition.z + Utility.GetSize(floor).x);


        for (int z = 0; z < 3; z++)
        {
            GameObject rightSide = Instantiate(Fence, m_garden.transform);
            rightSide.transform.localPosition = new Vector3(TopLeftCorner.x + z * (Utility.GetSize(floor).x),
                                                        0,
                                                        TopLeftCorner.z - (Utility.GetSize(floor).z / 2));
            rightSide.transform.localRotation = Quaternion.Euler(0, 90, 0);


            GameObject leftSide = Instantiate(Fence, m_garden.transform);
            leftSide.transform.localPosition = new Vector3(TopRightCorner.x + z * (Utility.GetSize(floor).x),
                                                        0,
                                                        TopRightCorner.z + (Utility.GetSize(floor).z / 2));
            leftSide.transform.localRotation = Quaternion.Euler(0, 90, 0);
        }

        for (int i = 0; i < Size.y; i++)
        {
            GameObject end = Instantiate(Fence, m_garden.transform);
            // X : Takes the top right corner and then moves up by the 3 spaces and then minuses half a tile to offset it back to the same level as the end of the fence.
            // Y : Since the Building transform will take care of that position.
            // Z : 
            // i * -Utility.GetSize(floor).x + (Utility.GetSize(floor).x) *Other*
            end.transform.localPosition = new Vector3(TopRightCorner.x + (3 * ((Utility.GetSize(floor).x))) - Utility.GetSize(floor).x / 2,
                                                        0,
                                                        Utility.GetSize(floor).z + (TopRightCorner.z + i * Utility.GetSize(floor).z));
            end.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }

        Utility.combineMesh(m_garden);
        // Plants
        Vector3 topLeftGardenCorner = new Vector3(TopRightCorner.x + (3 * ((Utility.GetSize(floor).x))) - Utility.GetSize(floor).x / 2, 0, Utility.GetSize(floor).z / 2 + (TopRightCorner.z + Size.y * Utility.GetSize(floor).z));
        Vector3 bottomRightGardenCorner = new Vector3(Corners[3].transform.localPosition.x + Utility.GetSize(floor).x / 2, 0, Corners[3].transform.localPosition.z - Utility.GetSize(floor).x / 2);

        for (int i = 0; i < 5; i++)
        {
            float x = Random.Range(topLeftGardenCorner.x, bottomRightGardenCorner.x);
            float z = Random.Range(topLeftGardenCorner.z, bottomRightGardenCorner.z);

            GameObject bush = Instantiate(Resources.Load("Summer") as GameObject, new Vector3(x, 0, z), Quaternion.Euler(270, 0, 0));
            bush.transform.SetParent(m_garden.transform);
        }

        // Garden Collider
        Vector3 colliderPoint = new Vector3(((topLeftGardenCorner.x + bottomRightGardenCorner.x) / 2), 0, ((topLeftGardenCorner.z + bottomRightGardenCorner.z) / 2));

        AddGardenComponents(m_garden, colliderPoint, new Vector2Int(3, Size.y));

    }
    void CreatePath(GameObject t_building)
    {
        //Vector3 scaling = new Vector3((Size.x / 1.0f), pathTile.transform.localScale.y, pathTile.transform.localScale.z);
        //Vector3 doorPos;
        //GameObject firstFloor = t_building.transform.GetChild(0).gameObject;
        //for (int i = 0; i < firstFloor.transform.childCount; i++)
        //{
        //    if (firstFloor.transform.GetChild(i).name.Contains("Door"))
        //    {
        //        doorPos = firstFloor.transform.GetChild(i).transform.localPosition;
        //        GameObject Tree = Instantiate(pathTile, t_building.transform);
        //        Tree.transform.localPosition = new Vector3(doorPos.x - Size.x / 2.0f, 0, doorPos.z);
        //        Tree.transform.localScale = scaling;
        //        break;
        //    }
        //}
    }
    void CreateHouseLight(GameObject t_building, int floor)
    {

        GameObject houseLight = Instantiate(indoorLight, t_building.transform);
        houseLight.transform.localPosition = new Vector3(houseLight.transform.localPosition.x,
                                                         floor * Utility.GetSize(wall).y,
                                                         houseLight.transform.localPosition.z);
    }
    
    // Others
    void AddBuildingComponents(GameObject building, Vector3 pos, Quaternion rotation)
    {

        building.transform.position = pos;
        building.transform.rotation = rotation;

        Vector3 m_colliderCentre = new Vector3(-Utility.GetSize(floor).x / 2.0f, ((Floors * Utility.GetSize(wall).y) / 2) - 0.5f, -Utility.GetSize(floor).z / 2.0f);

        building.AddComponent<BoxCollider>();
        building.GetComponent<BoxCollider>().center = m_colliderCentre;

        building.GetComponent<BoxCollider>().size = new Vector3((Size.x * Utility.GetSize(floor).x) - 0.5f,
                                                                    Floors * Utility.GetSize(wall).y + 0.5f,
                                                                    (Size.y * Utility.GetSize(floor).z) - 0.5f
                                                               );
        building.layer = LayerMask.NameToLayer("BuildingLayer");

        // Rigidbody 
        building.AddComponent<Rigidbody>();
        building.GetComponent<Rigidbody>().useGravity = false;
        building.GetComponent<Rigidbody>().isKinematic = false;
        building.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        building.tag = "House";
        if (SceneManager.GetActiveScene().name != "Building")
        {
           building.AddComponent<ObjectCollisions>();
        }
    }
    void AddGardenComponents(GameObject garden,Vector3 pos,Vector2Int t_size)
    {
        garden.AddComponent<BoxCollider>();
        garden.GetComponent<BoxCollider>().center = new Vector3(pos.x,
                                                                pos.y + 0.25f,
                                                                pos.z);


        garden.GetComponent<BoxCollider>().size = new Vector3(  t_size.x * 0.85f,
                                                                0.5f,
                                                                t_size.y * 0.9f);

        //Rigidbody
        garden.AddComponent<Rigidbody>();
        garden.GetComponent<Rigidbody>().useGravity = false;
        garden.GetComponent<Rigidbody>().isKinematic = false;
        garden.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        garden.tag = "Garden";
    }
    void EmptyLists()
    {
        floorTiles.Clear();
        wallTiles.Clear();
        Corners.Clear();
    }
    GameObject WallTypeSelector()
    {
        if (SceneManager.GetActiveScene().name == "Building")
        {
            Door = Resources.Load("BuildingParts/City/Door") as GameObject;
            window = Resources.Load("BuildingParts/City/WindowBay") as GameObject;
            crossWindow = Resources.Load("BuildingParts/City/CrossWindow") as GameObject;
            extrudedWindow = Resources.Load("BuildingParts/City/ExtrudedWindow") as GameObject;
            windowBay = Resources.Load("BuildingParts/City/WindowBay") as GameObject;

            //Wall Type being used ( Changes based on a chance between two two values )
            GameObject wallType = wall;

            //Chance when it is going to be window to be different types
            int wallTypeChance;
            int chance = Random.Range(0, 100);
            if (chance < 75)
            {
                wallType = wall;
            }
            else
            {
                wallTypeChance = Random.Range(0, 4);
                switch (wallTypeChance)
                {
                    case 1:
                        wallType = window;
                        break;
                    case 2:
                        wallType = crossWindow;
                        break;
                    case 3:
                        wallType = crossWindow;
                        break;
                    case 4:
                        wallType = extrudedWindow;
                        break;
                }
            }

            wallType.GetComponent<MeshRenderer>().material = Resources.Load("BuildingParts/Materials/Bricks") as Material;


            return wallType;

        }
        else
        {
            if (GameObject.Find("RoadSelector").GetComponent<Dropdown>().options[GameObject.Find("RoadSelector").GetComponent<Dropdown>().value].text == "City")
            {
                Door = Resources.Load("BuildingParts/City/Door") as GameObject;
                window = Resources.Load("BuildingParts/City/WindowBay") as GameObject;
                crossWindow = Resources.Load("BuildingParts/City/CrossWindow") as GameObject;
                extrudedWindow = Resources.Load("BuildingParts/City/ExtrudedWindow") as GameObject;
                windowBay = Resources.Load("BuildingParts/City/WindowBay") as GameObject;
            }
            else if (GameObject.Find("RoadSelector").GetComponent<Dropdown>().options[GameObject.Find("RoadSelector").GetComponent<Dropdown>().value].text == "Country")
            {
                Door = Resources.Load("BuildingParts/Country/DoorCountry") as GameObject;
                window = Resources.Load("BuildingParts/Country/WindowBayCountry") as GameObject;
                crossWindow = Resources.Load("BuildingParts/Country/CrossWindowCountry") as GameObject;
                extrudedWindow = Resources.Load("BuildingParts/Country/ExtrudedWindowCountry") as GameObject;
                windowBay = Resources.Load("BuildingParts/Country/WindowBayCountry") as GameObject;
            }

            //Wall Type being used ( Changes based on a chance between two two values )
            GameObject wallType = wall;

            //Chance when it is going to be window to be different types
            int wallTypeChance;
            int chance = Random.Range(0, 100);
            if (chance < 75)
            {
                wallType = wall;
            }
            else
            {
                wallTypeChance = Random.Range(0, 4);
                switch (wallTypeChance)
                {
                    case 1:
                        wallType = window;
                        break;
                    case 2:
                        wallType = crossWindow;
                        break;
                    case 3:
                        wallType = crossWindow;
                        break;
                    case 4:
                        wallType = extrudedWindow;
                        break;
                }
            }
            if (GameObject.Find("RoadSelector").GetComponent<Dropdown>().options[GameObject.Find("RoadSelector").GetComponent<Dropdown>().value].text == "City")
            {
                wallType.GetComponent<MeshRenderer>().material = Resources.Load("BuildingParts/Materials/Bricks") as Material;
            }
            else if (GameObject.Find("RoadSelector").GetComponent<Dropdown>().options[GameObject.Find("RoadSelector").GetComponent<Dropdown>().value].text == "Country")
            {
                wallType.GetComponent<MeshRenderer>().material = Resources.Load("BuildingParts/Materials/Wood") as Material;
            }

            return wallType;
        }
    }
}