using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TiledRoadCreator : MonoBehaviour
{
    Camera thisCamera; // Camera
    public GameObject player; // Player 

    GameObject straightRoad; // Normal Road
    GameObject cornerRoad; // Corner Road
    GameObject intersection; // Intersection
    GameObject TJunction; // T-Junction

    [HideInInspector]
    public GameObject grassTile; // normal empty tile
    GameObject ramp; // ramp tile
    GameObject bridge; // Bridge
    GameObject cornerBridge; // CornerBridge
    GameObject roadLamp; // Lamps
    GameObject streetSign; // tree object
    GameObject trafficLight; // Traffic Light


    [Header("Options")]
    public GameObject dropDown;
    public GameObject Canvas;

    [Header("Grid Options")]
    public int gridX = 10;
    public int gridZ = 10;

    [Header("Spawning")]
    [Range(0.0f,100.0f)]
    public int BuildingSpawnChance = 33;

    //Track to be saved
    private GameObject track;

    // Containers
    GameObject m_roads;


    GameObject currObj; // reference to previous object.
    List<GameObject> path = new List<GameObject>(); // List of all the road tiles in the path.

    [HideInInspector]
    public List<GameObject> plains = new List<GameObject>();

    LineRenderer lr; // line renderer

    private bool _isDraging = false; // Bool for if im dragging
    bool started = false; // turns off the start.


    //Variables for Placement
    Quaternion rot;

    Vector3 placementPosition; // position for placing the tiles.
    Vector3 startPoint; // startPoint of the creation.

    float selectorAngle; // angle used for choosing the roads.
    string prevDirection = ""; // previous direction moved in.

    // Ramping over river variables
    Vector3 colliderPos;
    bool firstHitWater = false;
    Quaternion rotation;
    float originalY;

    private void Awake()
    {
        thisCamera = Camera.main;
        grassTile = Resources.Load("Plain") as GameObject;
        ramp = Resources.Load("City/Ramp") as GameObject;
        bridge = Resources.Load("Bridge") as GameObject;
        cornerBridge = Resources.Load("CornerBridge") as GameObject;
        roadLamp = Resources.Load("StreetLamp") as GameObject;
        streetSign = Resources.Load("Sign") as GameObject;
        trafficLight = Resources.Load("TrafficLight") as GameObject;

        lr = gameObject.GetComponent<LineRenderer>();
        track = new GameObject("Track");
        track.gameObject.tag = "Track";

        createBlank();
    }

    private void Start()
    {
        originalY = plains[1].gameObject.transform.position.y; // sets the position back to the normal Y position
        m_roads = new GameObject("Roads");
        m_roads.transform.SetParent(track.transform);
    }

    void Update()
    {
        thisCamera.transform.position = new Vector3(placementPosition.x, thisCamera.transform.position.y, placementPosition.z);

        if (Camera.main == thisCamera)
        {
            //Mouse Events
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (dropDown.GetComponent<Dropdown>().options[dropDown.GetComponent<Dropdown>().value].text != "Select Road Type")
                {
                    //MOUSE PRESSES
                    //Create the roads with the mouse.
                    if (Input.GetMouseButtonDown(0))
                    {
                        // Checks if this is the first road tile
                        if (started == false)//After the first tile of the road has been placed
                        {
                            Collider[] hits = Physics.OverlapSphere(Utility._get3dMousePosition(), 0.0f);

                            if (hits.Length > 0)
                            {
                                startPoint = hits[0].gameObject.transform.position;
                                createTile(startPoint, straightRoad, Quaternion.identity, straightRoad.gameObject.tag);
                                placementPosition = startPoint;
                            }
                            started = true;
                        }

                        if(!Input.GetKey(KeyCode.LeftShift))
                        {
                            // shows the tool when held 
                            lr.enabled = true;
                            lr.positionCount = 2;
                            lr.SetPosition(0, new Vector3(startPoint.x, startPoint.y + 2, startPoint.z));
                        }

                        _isDraging = true;
                    }
                    //Changes the line rotation.
                    if (_isDraging)
                    {
                        //Gets the position of the mouse
                        Vector3 dir = Utility._get3dMousePosition() - startPoint;
                        selectorAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                        lr.SetPosition(1, new Vector3(Utility._get3dMousePosition().x, Utility._get3dMousePosition().y + 2, Utility._get3dMousePosition().z));
                    }
                    //Gets the angle of the line and then selects the road from that.
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (!Input.GetKey(KeyCode.LeftShift))
                        {
                            if (selectorAngle >= 65 && selectorAngle <= 115)
                            {
                                ChooseDirectionRoads(straightRoad, "right");
                                prevDirection = "right";
                            }
                            if (selectorAngle > -115 && selectorAngle < -65)
                            {
                                ChooseDirectionRoads(straightRoad, "left");
                                prevDirection = "left";
                            }
                            if (selectorAngle > 155 && selectorAngle > -155)
                            {
                                ChooseDirectionRoads(straightRoad, "down");
                                prevDirection = "down";
                            }
                            if (selectorAngle < 25 && selectorAngle > -25)
                            {
                                ChooseDirectionRoads(straightRoad, "up");
                                prevDirection = "up";
                            }

                            spawnBuilding();
                            createLamps();

                            _isDraging = false;
                            lr.enabled = false;
                        }
                        else
                        {
                            Collider[] hits = Physics.OverlapSphere(Utility._get3dMousePosition(), 0.0f);
                            if (hits.Length > 0)
                            {
                                Debug.Log(hits[0].gameObject.name);
                                placementPosition = hits[0].gameObject.transform.localPosition;
                                startPoint = placementPosition;
                                prevDirection = "";
                            }
                        }
                    }
                }
            }
        }

        changeTileType();
        createIntersection();
        tJunction();
        RiverOVerObject();
    }

    //Create the roads
    void ChooseDirectionRoads(GameObject obj, string direction)
    {
        switch(direction)
        {
            //Moving Right
            case "right":
                if(prevDirection == "up") // places a turning right corner if you where moving up
                {
                    placementPosition.z += (Utility.GetSize(cornerRoad).z); // moves placement for road
                    startPoint = new Vector3(placementPosition.x + Utility.GetSize(cornerRoad).x, placementPosition.y, placementPosition.z); // moves startpoint
                    createTile(placementPosition, cornerRoad, Quaternion.Euler(0.0f, 270.0f, 0.0f),cornerRoad.gameObject.tag);
                    prevDirection = "";
                }
                else if(prevDirection == "down") // if you where last moving down then this turns you right
                {
                    placementPosition.z -= (Utility.GetSize(cornerRoad).z);
                    startPoint = new Vector3(placementPosition.x + Utility.GetSize(cornerRoad).x, placementPosition.y, placementPosition.z);
                    createTile(placementPosition, cornerRoad, Quaternion.Euler(0.0f, 180.0f, 0.0f),cornerRoad.gameObject.tag);
                    prevDirection = "";
                }
                else // normally moves you right
                {
                    placementPosition.x += (Utility.GetSize(obj).x);
                    startPoint = new Vector3(placementPosition.x + Utility.GetSize(obj).x, placementPosition.y, placementPosition.z);
                    createTile(placementPosition, obj, Quaternion.identity,obj.gameObject.tag);
                }
                break;
            //Move you left 
            case "left":
                if(prevDirection == "up")
                {
                    placementPosition.z += (Utility.GetSize(cornerRoad).z);
                    startPoint = new Vector3(placementPosition.x - Utility.GetSize(cornerRoad).x, placementPosition.y, placementPosition.z);
                    createTile(placementPosition, cornerRoad, Quaternion.identity,cornerRoad.gameObject.tag);
                    prevDirection = "";
                }// if moving up this turns you left
                else if(prevDirection == "down")
                {
                    placementPosition.z -= (Utility.GetSize(cornerRoad).z);
                    startPoint = new Vector3(placementPosition.x - Utility.GetSize(cornerRoad).x, placementPosition.y, placementPosition.z);
                    createTile(placementPosition, cornerRoad, Quaternion.Euler(0.0f, 90.0f, 0.0f),cornerRoad.gameObject.tag);
                    prevDirection = "";
                }// if moving down this turns you left
                else
                {
                    placementPosition.x -= (Utility.GetSize(obj).x);
                    startPoint = new Vector3(placementPosition.x - Utility.GetSize(obj).x, placementPosition.y, placementPosition.z);
                    createTile(placementPosition, obj, Quaternion.identity,obj.gameObject.tag);
                }// otherwise just move left normally
                break;
            //Moves you up
            case "up":
                if(prevDirection == "right")
                {
                    placementPosition.x += (Utility.GetSize(cornerRoad).x);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z + Utility.GetSize(straightRoad).z);
                    createTile(placementPosition, cornerRoad, Quaternion.Euler(0.0f, 90.0f, 0.0f),cornerRoad.gameObject.tag);
                } // if moving right this turns you upwards
                else if(prevDirection == "left")
                {
                    placementPosition.x -= (Utility.GetSize(cornerRoad).x);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z + Utility.GetSize(cornerRoad).z);
                    createTile(placementPosition, cornerRoad, Quaternion.Euler(0.0f, 180.0f, 0.0f),cornerRoad.gameObject.tag);
                } // if moving left this turns you upwards
                else
                {
                    placementPosition.z += (Utility.GetSize(obj).z);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z + Utility.GetSize(obj).z);
                    createTile(placementPosition, obj, Quaternion.Euler(0.0f, 90.0f, 0.0f),obj.gameObject.tag);
                } // move up normally 
                break;
            //Moves you down
            case "down":
                if(prevDirection == "right")
                {
                    placementPosition.x += (Utility.GetSize(obj).x);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z - Utility.GetSize(obj).z);
                    createTile(placementPosition, cornerRoad, Quaternion.identity,cornerRoad.gameObject.tag);
                } // if moving right this turns you downwards
                else if (prevDirection == "left")
                {
                    placementPosition.x -= (Utility.GetSize(cornerRoad).x);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z - Utility.GetSize(straightRoad).z);
                    createTile(placementPosition, cornerRoad, Quaternion.Euler(0.0f, 270.0f, 0.0f),cornerRoad.gameObject.tag);
                } // if moving left this turns you downwards
                else
                {
                    placementPosition.z -= (Utility.GetSize(obj).z);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z - Utility.GetSize(obj).z);
                    createTile(placementPosition, obj, Quaternion.Euler(0.0f, 90.0f, 0.0f),obj.gameObject.tag);
                } // move down normally
                break;
        }
    }

    void createIntersection()
    {

        if (path.Count > 1)
        {
            for (int i = 0; i < path.Count; i++)
            {
                if (currObj != null)
                {
                    if (path[i] != null)
                    {
                        if (Mathf.Approximately(path[i].gameObject.transform.position.x, currObj.transform.position.x) && Mathf.Approximately(path[i].gameObject.transform.position.y, currObj.transform.position.y) && Mathf.Approximately(path[i].gameObject.transform.position.z, currObj.transform.position.z))
                        {
                            //Creates Intersection when tow road hit from the side
                            if (path[i].gameObject.gameObject.tag == "Road")
                            {
                                if (currObj.tag == "Road")
                                {
                                    if (path[i].gameObject.transform.rotation != currObj.transform.rotation) // iF the rotation of the road tile that im hitting is not the same then add intersection
                                    {
                                        replaceObject(path[i].gameObject, intersection, Quaternion.identity, i);
                                        // Create the traffic lights
                                        createTile( new Vector3(path[path.Count - 1].transform.position.x + Utility.GetSize(grassTile).x /2,
                                                                path[path.Count -1].transform.position.y + 1.3f,
                                                                path[path.Count-1].transform.position.z + Utility.GetSize(grassTile).x / 2),
                                                    trafficLight,
                                                    Quaternion.Euler(0.0f,0.0f,0.0f),
                                                    trafficLight.tag);

                                        createTile(new Vector3(path[path.Count - 1].transform.position.x - Utility.GetSize(grassTile).x / 2,
                                                                path[path.Count - 1].transform.position.y + 1.3f,
                                                                path[path.Count - 1].transform.position.z + Utility.GetSize(grassTile).x / 2),
                                                    trafficLight,
                                                    Quaternion.Euler(0.0f, 270.0f, 0.0f),
                                                    trafficLight.tag);

                                        return;

                                    }
                                }
                            }
                            else if (path[i].gameObject.gameObject.tag == "TJunction")                         // Creates intersection from an intersection
                            {
                                if (currObj.gameObject.tag == "Road")
                                {
                                    if (path[i].gameObject.transform.rotation == Quaternion.identity && currObj.transform.rotation == Quaternion.identity ||
                                        path[i].gameObject.transform.rotation == Quaternion.identity && currObj.transform.rotation.eulerAngles.y == 180.0f ||
                                        path[i].gameObject.transform.rotation.eulerAngles.y == 90.0f && currObj.transform.rotation.eulerAngles.y == 90.0f ||
                                        path[i].gameObject.transform.rotation.eulerAngles.y == 90.0f && currObj.transform.rotation.eulerAngles.y == 270.0f)
                                    {
                                        replaceObject(path[i].gameObject, intersection, Quaternion.identity, i);
                                        return;
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }

    }

    void tJunction()
    {
        if (path.Count > 1)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                if (path[i] != null)
                {
                    if (currObj.tag == "Road")
                    {
                        if (Mathf.Approximately(path[i].gameObject.transform.position.x, currObj.transform.position.x) && Mathf.Approximately(path[i].gameObject.transform.position.y, currObj.transform.position.y) && Mathf.Approximately(path[i].gameObject.transform.position.z, currObj.transform.position.z))
                        {
                            if (path[i].gameObject.tag != "TJunction" && path[i].gameObject.tag != "Road")
                            {
                                //Corner Checks
                                if (currObj.tag == "Road") // Replaces with a T-Junction
                                {
                                    if (path[i].gameObject.transform.rotation.eulerAngles.y == 90.0f && currObj.transform.rotation == Quaternion.identity || path[i].gameObject.transform.rotation.eulerAngles.y == 180.0f && currObj.transform.rotation == Quaternion.identity)
                                    {
                                        replaceObject(path[i].gameObject, TJunction, Quaternion.Euler(0.0f, 90.0f, 0.0f), i);
                                        createTile(new Vector3( path[path.Count - 1].transform.position.x - Utility.GetSize(grassTile).x / 2,
                                                                path[path.Count - 1].transform.position.y + 1.3f,
                                                                path[path.Count - 1].transform.position.z + Utility.GetSize(grassTile).x / 2),
                                                    trafficLight,
                                                    Quaternion.Euler(0.0f, 270.0f, 0.0f),
                                                    trafficLight.tag);
                                        return;
                                    }

                                    if (path[i].gameObject.transform.rotation.eulerAngles.y == 90.0f && currObj.transform.rotation.eulerAngles.y == 90.0f || path[i].gameObject.transform.rotation == Quaternion.identity && currObj.transform.rotation.eulerAngles.y == 90.0f)
                                    {
                                        replaceObject(path[i].gameObject, TJunction, Quaternion.identity, i);
                                        createTile(new Vector3(path[path.Count - 1].transform.position.x - Utility.GetSize(grassTile).x / 2,
                                                                path[path.Count - 1].transform.position.y + 1.3f,
                                                                path[path.Count - 1].transform.position.z + Utility.GetSize(grassTile).x / 2),
                                                    trafficLight,
                                                    Quaternion.Euler(0.0f, 270.0f, 0.0f),
                                                    trafficLight.tag);
                                        return;
                                    }

                                    if (path[i].gameObject.transform.rotation == Quaternion.identity && currObj.transform.rotation == Quaternion.identity || path[i].gameObject.transform.rotation.eulerAngles.y == 270.0f && currObj.transform.rotation == Quaternion.identity)
                                    {
                                        replaceObject(path[i].gameObject, TJunction, Quaternion.Euler(0.0f, 270.0f, 0.0f), i);
                                        createTile(new Vector3(path[path.Count - 1].transform.position.x - Utility.GetSize(grassTile).x / 2,
                                                                path[path.Count - 1].transform.position.y + 1.3f,
                                                                path[path.Count - 1].transform.position.z + Utility.GetSize(grassTile).x / 2),
                                                    trafficLight,
                                                    Quaternion.Euler(0.0f, 270.0f, 0.0f),
                                                    trafficLight.tag);
                                        return;
                                    }

                                    if (path[i].gameObject.transform.rotation.eulerAngles.y == 180.0f && currObj.transform.rotation.eulerAngles.y == 90.0f || path[i].gameObject.transform.rotation.eulerAngles.y == 270.0f && currObj.transform.rotation.eulerAngles.y == 90.0f)
                                    {
                                        replaceObject(path[i].gameObject, TJunction, Quaternion.Euler(0.0f, 180.0f, 0.0f), i);
                                        createTile(new Vector3(path[path.Count - 1].transform.position.x - Utility.GetSize(grassTile).x / 2,
                                                                path[path.Count - 1].transform.position.y + 1.3f,
                                                                path[path.Count - 1].transform.position.z + Utility.GetSize(grassTile).x / 2),
                                                    trafficLight,
                                                    Quaternion.Euler(0.0f, 270.0f, 0.0f),
                                                    trafficLight.tag);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    else if (currObj.tag == "Corner")
                    {
                        if (Mathf.Approximately(path[i].gameObject.transform.position.x, currObj.transform.position.x) && Mathf.Approximately(path[i].gameObject.transform.position.y, currObj.transform.position.y) && Mathf.Approximately(path[i].gameObject.transform.position.z, currObj.transform.position.z))
                        {
                            if (path[i].gameObject.tag != "TJunction")
                            {
                                // -----> down
                                if (path[i].gameObject.transform.rotation == Quaternion.identity && currObj.transform.rotation.eulerAngles.y == 0.0f)
                                {
                                    replaceObject(path[i].gameObject, TJunction, Quaternion.Euler(0.0f, 270.0f, 0.0f), i);
                                    return;
                                }
                                // <------ down
                                if (path[i].gameObject.transform.rotation == Quaternion.identity && currObj.transform.rotation.eulerAngles.y == 270.0f)
                                {
                                    replaceObject(path[i].gameObject, TJunction, Quaternion.Euler(0.0f, 270.0f, 0.0f), i);
                                    return;
                                }
                                //--> Up 
                                if (path[i].gameObject.transform.rotation == Quaternion.identity && currObj.transform.rotation.eulerAngles.y == 90.0f)
                                {
                                    replaceObject(path[i].gameObject, TJunction, Quaternion.Euler(0.0f, 90.0f, 0.0f), i);
                                    return;
                                }
                                // <------ Up
                                if (path[i].gameObject.transform.rotation == Quaternion.identity && currObj.transform.rotation.eulerAngles.y == 180)
                                {
                                    replaceObject(path[i].gameObject, TJunction, Quaternion.Euler(0.0f, 90.0f, 0.0f), i);
                                    return;
                                }

                                //^ Left
                                if (path[i].gameObject.transform.rotation.eulerAngles.y == 90.0f && currObj.transform.rotation.eulerAngles.y == 0.0f)
                                {
                                    replaceObject(path[i].gameObject, TJunction, Quaternion.Euler(0.0f, 0.0f, 0.0f), i);
                                    return;
                                }
                                //^ Right
                                if (path[i].gameObject.transform.rotation.eulerAngles.y == 90.0f && currObj.transform.rotation.eulerAngles.y == 270.0f)
                                {
                                    replaceObject(path[i].gameObject, TJunction, Quaternion.Euler(0.0f, 180.0f, 0.0f), i);
                                    return;
                                }
                                //V Left
                                if (path[i].gameObject.transform.rotation.eulerAngles.y == 90.0f && currObj.transform.rotation.eulerAngles.y == 90.0f)
                                {
                                    replaceObject(path[i].gameObject, TJunction, Quaternion.Euler(0.0f, 0.0f, 0.0f), i);
                                    return;
                                }
                                //V Right
                                if (path[i].gameObject.transform.rotation.eulerAngles.y == 90.0f && currObj.transform.rotation.eulerAngles.y == 180.0f)
                                {
                                    replaceObject(path[i].gameObject, TJunction, Quaternion.Euler(0.0f, 180.0f, 0.0f), i);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void RiverOVerObject()
    {
        if (path.Count != 0)
        {
            Vector3 secondRampLocation = Vector3.zero;

            if (!firstHitWater)
            {
                if (prevDirection == "right")
                {
                    colliderPos = new Vector3(placementPosition.x + Utility.GetSize(grassTile).x, originalY, placementPosition.z);
                    rotation = Quaternion.Euler(0, 0.0f, 0);
                }
                else if (prevDirection == "left")
                {
                    colliderPos = new Vector3(placementPosition.x - Utility.GetSize(grassTile).x, originalY, placementPosition.z);
                    rotation = Quaternion.Euler(0, 180.0f, 0);
                }
                else if (prevDirection == "up")
                {
                    colliderPos = new Vector3(placementPosition.x, originalY, placementPosition.z + Utility.GetSize(grassTile).z);
                    rotation = Quaternion.Euler(0, 270.0f, 0);
                }
                else if (prevDirection == "down")
                {
                    colliderPos = new Vector3(placementPosition.x, originalY, placementPosition.z - Utility.GetSize(grassTile).z);
                    rotation = Quaternion.Euler(0, 90, 0);
                }
            }
            else
            {
                colliderPos = new Vector3(placementPosition.x, originalY, placementPosition.z);

                if (prevDirection == "right")
                {
                    rotation = Quaternion.Euler(0, 180.0f, 0);
                }
                else if (prevDirection == "left")
                {
                    rotation = Quaternion.Euler(0, 0.0f, 0);
                }
                else if (prevDirection == "up")
                {
                    rotation = Quaternion.Euler(0, 90.0f, 0);
                }
                else if (prevDirection == "down")
                {
                    rotation = Quaternion.Euler(0, 270, 0);
                }
            }
            // Sets up the collider at the position at the point that is updated above
            Collider[] hit = Physics.OverlapSphere(colliderPos, 0.5f);

            // Run through the objects that are hit by the sphere 
            for (int i = 0; i < hit.Length; i++)
            {
                // Only if its a piece of water that ive hit
                if (hit[i].gameObject.tag == "Wave")
                {
                    if (!firstHitWater)
                    {
                        replaceObject(path[path.Count - 1].gameObject, ramp, rotation, path.Count - 1);
                        path[path.Count - 1].gameObject.transform.localPosition = new Vector3(path[path.Count - 1].gameObject.transform.localPosition.x,
                                                                                                path[path.Count - 1].gameObject.transform.localPosition.y - Utility.GetSize(ramp).y / 5.5f,
                                                                                                path[path.Count - 1].gameObject.transform.localPosition.z
                                                                                             );
                        placementPosition.y += 1.1f;
                        firstHitWater = true;

                    }

                }
                if (hit.Length == 1)
                {
                    // Only if the object i have now hit is a normal grass tile
                    if (firstHitWater)
                    {
                        if (hit[i].gameObject.tag == "Plain")
                        {
                            replaceObject(path[path.Count - 1].gameObject, ramp, rotation, path.Count - 1);
                            path[path.Count - 1].gameObject.transform.localPosition = new Vector3(path[path.Count - 1].gameObject.transform.localPosition.x,
                                                                                                path[path.Count - 1].gameObject.transform.localPosition.y - Utility.GetSize(ramp).y / 1.225f,
                                                                                                path[path.Count - 1].gameObject.transform.localPosition.z
                                                                                             );
                            secondRampLocation = path[path.Count - 1].gameObject.transform.localPosition;
                            firstHitWater = false;
                            placementPosition.y -= 1.1f;

                        }
                    }
                }
            }

            if (path[path.Count - 1].transform.localPosition.y > originalY)
            {
                if (path[path.Count - 1].CompareTag("Road"))
                {
                    replaceObject(path[path.Count - 1].gameObject, bridge, path[path.Count - 1].gameObject.transform.localRotation, path.Count - 1);
                    path[path.Count - 1].transform.localPosition = new Vector3(path[path.Count - 1].transform.localPosition.x,
                                                                                path[path.Count - 1].transform.localPosition.y - 0.15f,
                                                                                path[path.Count - 1].transform.localPosition.z);
                }
                //else if(path[path.Count - 1].CompareTag("Corner"))
                //{
                //    replaceObject(path[path.Count - 1].gameObject, cornerBridge, path[path.Count - 1].gameObject.transform.localRotation, path.Count - 1);

                //    if(prevDirection == "right")
                //    {
                //        path[path.Count - 1].transform.localPosition = new Vector3(path[path.Count - 1].transform.localPosition.x - 0.35f,
                //                                                path[path.Count - 1].transform.localPosition.y - 0.15f,
                //                                                path[path.Count - 1].transform.localPosition.z - 0.35f);
                //    }
                //}
            }
        }
    }

    // Used to create a tile
    void createTile(Vector3 location, GameObject obj,Quaternion q,string tileTag)
    {
        //hit[0] is the object thats been hit 
        // tileTag is the object that is being created
        Collider[] hit = Physics.OverlapSphere(location, 0);
        if (hit.Length > 0 )
        {
            //checks if the object hit was an intersection and if it was dont change it as you shouldnt be able too
            if(hit[0].gameObject.tag == "Intersection")
            {
                tileTag = "";
                return;
            }

            // Checks if the object thats been hit is a corner
            if (hit[0].gameObject.tag == "Corner")
            {
                rot = q;
                GameObject objectC = Instantiate(obj, m_roads.transform);
                objectC.transform.localPosition = location;
                objectC.transform.localRotation = q;
                if (tileTag != "Lamp")
                {
                    path.Add(objectC);
                    currObj = objectC;
                    tileTag = currObj.gameObject.tag;
                }
                return;
            }

            // checks the current tile 
            if (tileTag == "Road")
            {
                // checks the object i just hit, Check if its rotation is the same and if it is then dont make another one
                if (hit[0].gameObject.tag != "Plain")
                {
                    if (hit[0].gameObject.tag == "Wave")
                    {
                        rot = q;
                        GameObject objectC = Instantiate(obj, m_roads.transform);
                        objectC.transform.localPosition = location;
                        objectC.transform.localRotation = q;
                    }
                    else if(hit[0].transform.localRotation == q)
                    {
                        return;
                    }
                    //if (hit[0].transform.localRotation == q)
                    //{
                    //    return;
                    //}
                    else // otherwise create a new one( this is where the intersection is made )
                    {
                        rot = q;
                        GameObject objectC = Instantiate(obj, m_roads.transform);
                        objectC.transform.localPosition = location;
                        objectC.transform.localRotation = q;
                        if (tileTag != "Lamp")
                        {
                            path.Add(objectC);
                            currObj = objectC;
                            tileTag = currObj.gameObject.tag;
                        }
                    }             
                }
                else
                {
                    rot = q;
                    GameObject objectC = Instantiate(obj, m_roads.transform);
                    objectC.transform.localPosition = location;
                    objectC.transform.localRotation = q;
                    if (tileTag != "Lamp")
                    {
                        path.Add(objectC);
                        currObj = objectC;
                        tileTag = currObj.gameObject.tag;
                    }
                }
            }
            else // if the current tile is not a road then just create it 
            {
                rot = q;
                GameObject objectC = Instantiate(obj, m_roads.transform);
                objectC.transform.localPosition = location;
                objectC.transform.localRotation = q;
                if (tileTag != "Lamp")
                {
                    path.Add(objectC);
                    currObj = objectC;
                    tileTag = currObj.gameObject.tag;
                }
            }
        }
        else // if nothing is hit hten just create normally 
        {
            rot = q;
            GameObject objectC = Instantiate(obj, m_roads.transform);
            objectC.transform.localPosition = location;
            objectC.transform.localRotation = q;
            if(tileTag != "Lamp")
            {
                path.Add(objectC);
                currObj = objectC;
                tileTag = currObj.gameObject.tag;
            }
        }
    } // Creates a tile 

    //Replaces the object with a new object
    void replaceObject(GameObject obectbeingReplaced, GameObject objectReplacing, Quaternion rot, int placeInList)
    {
        Quaternion rotation = rot;
        Destroy(obectbeingReplaced);
        path.RemoveAt(placeInList);
        Destroy(currObj);
        createTile(placementPosition, objectReplacing, rot,objectReplacing.gameObject.tag);
        path.Insert(placeInList, currObj);
    } // REplace the game object in the list with a new one

    void createLamps()
    {
        float offsetYForLamp = 0.2f;

        if (path.Count > 0)
        {
            if ((path.Count % 2) != 0)
            {
                if (path[path.Count - 1].gameObject.tag == "Road")
                {
                    // Num 0,1 is lamps
                    // Num 2,3 is signs
                    int num = Random.Range(0, 4);
                    if (num == 0)
                    {
                        if (prevDirection == "left" || prevDirection == "right")
                        {
                            createTile(new Vector3(path[path.Count - 1].transform.position.x, path[path.Count - 1].transform.position.y + offsetYForLamp, path[path.Count - 1].transform.position.z - Utility.GetSize(path[path.Count - 1].gameObject).z / 3.5f), roadLamp, path[path.Count - 1].transform.rotation * roadLamp.transform.rotation, roadLamp.gameObject.tag);
                            return;
                        }
                        else if (prevDirection == "up" || prevDirection == "down")
                        {
                            createTile(new Vector3(path[path.Count - 1].transform.position.x - Utility.GetSize(path[path.Count - 1].gameObject).x / 3.5f, path[path.Count - 1].transform.position.y + offsetYForLamp, path[path.Count - 1].transform.position.z), roadLamp, path[path.Count - 1].transform.rotation * roadLamp.transform.rotation, roadLamp.gameObject.tag);
                            return;
                        }
                    }
                    else if (num == 1)
                    {
                        if (prevDirection == "left" || prevDirection == "right")
                        {
                            createTile(new Vector3(path[path.Count - 1].transform.position.x, path[path.Count - 1].transform.position.y + offsetYForLamp, path[path.Count - 1].transform.position.z + Utility.GetSize(path[path.Count - 1].gameObject).z / 3.5f), roadLamp, Quaternion.Euler(0, 270, 0), roadLamp.gameObject.tag);
                            return;
                        }
                        else if (prevDirection == "up" || prevDirection == "down")
                        {
                            createTile(new Vector3(path[path.Count - 1].transform.position.x + Utility.GetSize(path[path.Count - 1].gameObject).x / 3.5f, path[path.Count - 1].transform.position.y + offsetYForLamp, path[path.Count - 1].transform.position.z), roadLamp, Quaternion.Euler(0,0,0), roadLamp.gameObject.tag);
                            return;
                        }
                    }
                    else if(num == 2)
                    {
                        if (prevDirection == "left" || prevDirection == "right")
                        {
                            createTile(new Vector3( path[path.Count - 1].transform.position.x,
                                                    path[path.Count - 1].transform.position.y + offsetYForLamp,
                                                    path[path.Count - 1].transform.position.z - Utility.GetSize(path[path.Count - 1].gameObject).z / 3.5f), roadLamp, path[path.Count - 1].transform.rotation * roadLamp.transform.rotation, roadLamp.gameObject.tag);
                            return;
                        }
                        else if (prevDirection == "up" || prevDirection == "down")
                        {
                            createTile(new Vector3( path[path.Count - 1].transform.position.x - Utility.GetSize(path[path.Count - 1].gameObject).x / 3.5f,
                                                    path[path.Count - 1].transform.position.y + offsetYForLamp, 
                                                    path[path.Count - 1].transform.position.z), roadLamp, path[path.Count - 1].transform.rotation * roadLamp.transform.rotation, roadLamp.gameObject.tag);
                            return;
                        }
                    }
                    else if(num == 3)
                    {
                        if (prevDirection == "left" || prevDirection == "right")
                        {
                            createTile(new Vector3( path[path.Count - 1].transform.position.x,
                                                    path[path.Count - 1].transform.position.y + 1,
                                                    path[path.Count - 1].transform.position.z + Utility.GetSize(path[path.Count - 1].gameObject).z / 3.5f), streetSign, Quaternion.Euler(0, 90, 0), streetSign.gameObject.tag);
                            return;
                        }
                        else if (prevDirection == "up" || prevDirection == "down")
                        {
                            createTile(new Vector3( path[path.Count - 1].transform.position.x + Utility.GetSize(path[path.Count - 1].gameObject).x / 3.5f,
                                                    path[path.Count - 1].transform.position.y + 1,
                                                    path[path.Count - 1].transform.position.z), streetSign, Quaternion.Euler(0, 0, 0), streetSign.gameObject.tag);
                            return;
                        }
                    }
                }
            }
            else
            {
                return;
            }
        }
    }

    public void createBlank()
    {
        GameObject m_ground = new GameObject("Ground");
        m_ground.transform.SetParent(GameObject.Find("Track").transform);
        for(int x = 0; x < gridX; x++)
        {
            for(int z = 0; z < gridZ; z ++)
            {
                Vector3 tempVec = new Vector3(placementPosition.x + ((gridX / 2) * Utility.GetSize(grassTile).x) - (x * Utility.GetSize(grassTile).x), 1, placementPosition.z + ((gridZ / 2) * Utility.GetSize(grassTile).z) - (z * Utility.GetSize(grassTile).z));
                GameObject temp = Instantiate(grassTile, m_ground.transform);
                temp.transform.localPosition = tempVec;
                temp.transform.localRotation = Quaternion.identity;
                plains.Add(temp);
            }
        }
    }

    void spawnBuilding()
    {
        if (path.Count > 0)
        {
            int num = Random.Range(0, 100);

            if (num < BuildingSpawnChance)
            {
                if (path[path.Count - 1].gameObject.tag == "Road")
                {
                    if (path[path.Count -1].gameObject.transform.localPosition.y < 2)
                    {
                        if (num < BuildingSpawnChance / 2)
                        {
                            if (prevDirection == "left" || prevDirection == "right")
                            {
                                //Left to right under the road
                                GetComponent<TEst>().createBuilding(new Vector3(
                                                                            path[path.Count - 1].transform.position.x,
                                                                            path[path.Count - 1].transform.position.y + Utility.GetSize(grassTile).y / 2,
                                                                            path[path.Count - 1].transform.position.z - (Utility.GetSize(path[path.Count - 1].gameObject).z / 2.0f) - GetComponent<TEst>().Size.y / 1.0f),
                                                                            Quaternion.Euler(0, 90, 0));
                                return;
                            }
                            else if (prevDirection == "up" || prevDirection == "down")
                            {
                                GetComponent<TEst>().createBuilding(new Vector3(
                                                                             path[path.Count - 1].transform.position.x - Utility.GetSize(path[path.Count - 1].gameObject).x / 2.0f - GetComponent<TEst>().Size.x / 1.0f,
                                                                             path[path.Count - 1].transform.position.y + Utility.GetSize(grassTile).y / 2,
                                                                             path[path.Count - 1].transform.position.z),
                                                                             Quaternion.Euler(0, 180, 0));
                                return;
                            }
                        }
                        if (num < BuildingSpawnChance && num > BuildingSpawnChance / 2)
                        {
                            if (prevDirection == "left" || prevDirection == "right")
                            {
                                GetComponent<TEst>().createBuilding(new Vector3(
                                                                            path[path.Count - 1].transform.position.x,
                                                                            path[path.Count - 1].transform.position.y + Utility.GetSize(grassTile).y / 2.0f,
                                                                            path[path.Count - 1].transform.position.z + (Utility.GetSize(path[path.Count - 1].gameObject).z / 2.0f) + GetComponent<TEst>().Size.y / 1.0f),
                                                                            Quaternion.Euler(0, 270, 0));
                                return;
                            }
                            else if (prevDirection == "up" || prevDirection == "down")
                            {
                                GetComponent<TEst>().createBuilding(new Vector3(
                                                                            path[path.Count - 1].transform.position.x + Utility.GetSize(path[path.Count - 1].gameObject).x / 2.0f + GetComponent<TEst>().Size.x / 1.0f,
                                                                            path[path.Count - 1].transform.position.y + Utility.GetSize(grassTile).y / 2.0f,
                                                                            path[path.Count - 1].transform.position.z),
                                                                            Quaternion.Euler(0, 0, 0));
                                return;
                            }
                        }
                    }
                }
            }
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Change Types
    void changeTileType()
    {
        if (GameObject.Find("RoadSelector").GetComponent<Dropdown>().options[dropDown.GetComponent<Dropdown>().value].text == "Select Road Type")
        {
            GameObject.Find("Start Text").GetComponent<Text>().enabled = true;
        }

        if (GameObject.Find("RoadSelector").GetComponent<Dropdown>().options[dropDown.GetComponent<Dropdown>().value].text == "City")
        {
            GameObject.Find("Start Text").GetComponent<Text>().enabled = false;
            straightRoad = Resources.Load("City/Road") as GameObject;
            cornerRoad = Resources.Load("City/corner") as GameObject;
            intersection = Resources.Load("City/Intersection") as GameObject;
            TJunction = Resources.Load("City/T-Junction") as GameObject;

        }
        if (GameObject.Find("RoadSelector").GetComponent<Dropdown>().options[dropDown.GetComponent<Dropdown>().value].text == "Country")
        {
            GameObject.Find("Start Text").GetComponent<Text>().enabled = false;
            straightRoad = Resources.Load("Country/Road") as GameObject;
            cornerRoad = Resources.Load("Country/corner") as GameObject;
            intersection = Resources.Load("Country/Intersection") as GameObject;
            TJunction = Resources.Load("Country/T-Junction") as GameObject;
        }
    }

}