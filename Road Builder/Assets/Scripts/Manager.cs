using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Playe Canvas Indexs:
// 0 = Tree Chance
// 1 = Rock Chance
// 2 = Grass Chance

public class Manager : MonoBehaviour
{
    [Header("Camera's")]
    //Camera
    public Camera m_playerCam;
    public Camera m_mainCamera;

    [Header("Canvas")]
    //Canvas
    public Canvas m_playerCanvas;
    public Canvas m_roadCreateCanvas;

    [Header("Objects")]
    //Player 
    public GameObject m_player;
    public GameObject m_tileCreator;

    // Previous Chances
    List<InputField> m_chances = new List<InputField>();
    int m_previousTreeChance = 0;
    int m_previousRockChance = 0;
    int m_previousGrassChance = 0;

    //Selecting
    Ray m_ray;
    RaycastHit m_rayHit;

    //List for settings
    List<GameObject> m_settings = new List<GameObject>();
    Vector2Int m_canvasRange;

    // Rectangle Selector

    private void Awake()
    {
        m_canvasRange = new Vector2Int(0, 3);

        m_playerCam.enabled = false;
        for (int i = 0; i < 3; i++)
        {
            m_chances.Add(m_playerCanvas.transform.GetChild(i).transform.GetChild(0).GetComponent<InputField>());
        }

        for (int i = 0; i < m_playerCanvas.transform.childCount; i++)
        {
            // i < 3 == Basic World Settings
            // i  > 2 && i < 4 == ROCKS SETTINGS
            m_settings.Add(m_playerCanvas.transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CreatorState();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            lockMovement();
        }

        UpdatePlayerCanvas();

    }

    void lockMovement()
    {
        if (m_player.GetComponent<CharCont>().enabled)
        {
            m_player.GetComponent<CharCont>().enabled = false;
            m_player.GetComponent<RotateToMouse>().enabled = false;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            m_player.GetComponent<CharCont>().enabled = true;
            m_player.GetComponent<RotateToMouse>().enabled = true;
        }
    }

    //Player Canvas updates
    void UpdatePlayerCanvas()
    {
        if (m_playerCam.isActiveAndEnabled == true)
        {
            UpdateTreeCount();
            UpdateRockCount();
            UpdateGrassCount();
            UpdateMouseOverObject();
        }
    }

    //Live Updates to counts
    void UpdateTreeCount()
    {
            if (m_chances[0].text != m_previousTreeChance.ToString())
            {
                m_tileCreator.GetComponent<TiledRoadCreator>().deleteFromWorld("TREES");

                m_tileCreator.GetComponent<TiledRoadCreator>().scatter(m_tileCreator.GetComponent<TiledRoadCreator>().plains[0].transform.position,
                                                                       "TREES",
                                                                        int.Parse(m_chances[0].text));
            }
            m_previousTreeChance = int.Parse(m_chances[0].text);
    }
    void UpdateRockCount()
    {
            if (m_chances[1].text != m_previousRockChance.ToString())
            {
                m_tileCreator.GetComponent<TiledRoadCreator>().deleteFromWorld("ROCKS");

                m_tileCreator.GetComponent<TiledRoadCreator>().scatter(m_tileCreator.GetComponent<TiledRoadCreator>().plains[0].transform.position,
                                                                       "ROCKS",
                                                                        int.Parse(m_chances[1].text));
            }
            m_previousRockChance = int.Parse(m_chances[1].text);
    }
    void UpdateGrassCount()
    {
            if (m_chances[2].text != m_previousGrassChance.ToString())
            {
                m_tileCreator.GetComponent<TiledRoadCreator>().deleteFromWorld("GRASS");

                m_tileCreator.GetComponent<TiledRoadCreator>().scatter(m_tileCreator.GetComponent<TiledRoadCreator>().plains[0].transform.position,
                                                                       "GRASS",
                                                                        int.Parse(m_chances[2].text));
            }
        m_previousGrassChance = int.Parse(m_chances[2].text);
    }

    //Selection 
    void UpdateMouseOverObject()
    {
        m_ray = m_playerCam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(m_ray,out m_rayHit))
        {
            UpdateSelected(m_rayHit.transform.gameObject);
        }
    }
    void UpdateSelected(GameObject t_objectSelected)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(t_objectSelected.name.Contains("Plain"))
            {
                m_canvasRange.x = 0;
                m_canvasRange.y = 3;
            }

            if (t_objectSelected.name.Contains("Rock"))
            {
                m_canvasRange.x = 3;
                m_canvasRange.y = 4;
            }

            if (t_objectSelected.name.Contains("tree"))
            {
                m_canvasRange.x = 4;
                m_canvasRange.y = 5;
            }

            if (t_objectSelected.name.Contains("Grass"))
            {
                m_canvasRange.x = 5;
                m_canvasRange.y = 6;
            }
            toggleSettings();
        }
    }
    void toggleSettings()
    {
        for (int i = 0; i < m_settings.Count; i++)
        {
            if (i >= m_canvasRange.x && i < m_canvasRange.y)
            {
                m_settings[i].SetActive(true);
            }
            else
            {
                if(m_settings[i].activeInHierarchy)
                {
                    m_settings[i].SetActive(false);
                }
            }
        }
    }

// Other Functions 
    //Button Functions
    public void switchCamera()
    {
        if (Camera.main == m_playerCam)
        {
            CreatorState();
        }
        else
        {
            PlayerState();
        }
    }
    //Camera States
    void PlayerState()
    {
        m_mainCamera.enabled = false;
        m_playerCam.enabled = true;
        m_roadCreateCanvas.enabled = false;
        m_playerCanvas.gameObject.SetActive(true);

        m_playerCanvas.enabled = true;
        m_player.SetActive(true);
        m_tileCreator.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        m_chances[0].text = m_previousTreeChance.ToString();
        m_chances[1].text = m_previousRockChance.ToString();
        m_chances[2].text = m_previousGrassChance.ToString();

    }
    void CreatorState()
    {
        m_playerCam.enabled = false;
        m_mainCamera.enabled = true;
        m_roadCreateCanvas.enabled = true;
        m_playerCanvas.gameObject.SetActive(false);
        m_playerCanvas.enabled = false;
        m_player.SetActive(false);
        m_tileCreator.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }
}
