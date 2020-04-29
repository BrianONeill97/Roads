using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    [Header("Camera's")]
    public Camera m_playerCam;
    public Camera m_mainCamera;

    [Header("Light's")]
    public Light directionalLight;

    [Header("Canvas")]
    public Canvas m_roadCreateCanvas;

    [Header("Objects")]
    public GameObject m_player;
    public GameObject m_tileCreator;

    // List of settings
    GameObject OverallSettingsSwitches;

    [HideInInspector]
    public GameObject treeSettingObject;
    [HideInInspector]
    public GameObject rockSettingObject;
    [HideInInspector]
    public GameObject grassSettingObject;

    //Selecting
    Ray m_ray;
    RaycastHit m_rayHit;

    //List for settings
    List<GameObject> m_settings = new List<GameObject>();
    Vector2Int m_canvasRange;


    private void Awake()
    {
        m_canvasRange = new Vector2Int(0, 3);

        m_playerCam.enabled = false;
        m_player.GetComponent<CharCont>().enabled = false;
        m_player.GetComponent<RotateToMouse>().enabled = false;
        Cursor.lockState = CursorLockMode.None;

    }

    private void Start()
    {
        OverallSettingsSwitches = GameObject.Find("SettingsSwitchButtons");

        treeSettingObject = GameObject.Find("TreeSettings");
        rockSettingObject = GameObject.Find("RockSettings");
        grassSettingObject = GameObject.Find("GrassSettings");

        treeSettingObject.SetActive(false);
        rockSettingObject.SetActive(false);
        grassSettingObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CreatorState();
        }
        UpdatePlayerCanvas();
    }

    //Player Canvas updates
    void UpdatePlayerCanvas()
    {
        if (m_playerCam.isActiveAndEnabled == true)
        {
            UpdateMouseOverObject();
        }
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
            if(t_objectSelected.layer != LayerMask.NameToLayer("Road"))
            {
                Destroy(t_objectSelected);
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
    public void LightToggle()
    {
        if (directionalLight.gameObject.activeSelf == true)
        {
            directionalLight.gameObject.SetActive(false);
        }
        else
        {
            directionalLight.gameObject.SetActive(true);
        }
    }
    //Camera States
    void PlayerState()
    {
        m_mainCamera.enabled = false;
        m_playerCam.enabled = true;
        m_roadCreateCanvas.enabled = false;

        m_player.SetActive(true);
        m_tileCreator.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        m_player.GetComponent<RotateToMouse>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        m_player.GetComponent<CharCont>().enabled = true;
        m_player.GetComponent<RotateToMouse>().enabled = true;
    }

    void CreatorState()
    {
        m_playerCam.enabled = false;
        m_mainCamera.enabled = true;
        m_roadCreateCanvas.enabled = true;
        m_player.SetActive(false);
        m_tileCreator.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        m_player.GetComponent<RotateToMouse>().enabled = false;
        m_player.GetComponent<CharCont>().enabled = false;
        m_player.GetComponent<RotateToMouse>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ToggleTreeSettings()
    {
        if (treeSettingObject.activeSelf == true)
        {
            OverallSettingsSwitches.SetActive(true);
            treeSettingObject.SetActive(false);
            return;
        }
        else
        {
            OverallSettingsSwitches.SetActive(false);
            treeSettingObject.SetActive(true);
            return;
        }

    }

    public void ToggleRockSettings()
    {
        if (rockSettingObject.activeSelf == true)
        {
            OverallSettingsSwitches.SetActive(true);
            rockSettingObject.SetActive(false);
            return;
        }
        else
        {
            OverallSettingsSwitches.SetActive(false);
            rockSettingObject.SetActive(true);
            return;
        }

    }

    public void ToggleGrassSettings()
    {
        if (grassSettingObject.activeSelf == true)
        {
            OverallSettingsSwitches.SetActive(true);
            grassSettingObject.SetActive(false);
            return;
        }
        else
        {
            OverallSettingsSwitches.SetActive(false);
            grassSettingObject.SetActive(true);
            return;
        }

    }


}
