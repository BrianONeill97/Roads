using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GrassCreator : MonoBehaviour
{
    // Grass Container
    GameObject GrassContainer;

    List<GameObject> GrassList = new List<GameObject>(); // List of all the Grass Objects.
    GameObject Grass; // Single Grass Object With the L-System Script on it.
    public int m_maxGrassInArea; // Max Grass in the Area
    int m_currentGrassCountForBox; // Current Amount
    public static bool m_allowCreation = false; // Checks if the Correct Ui option has been selected

    private void Awake()
    {
        Grass = Resources.Load("Grass") as GameObject; // Load Grass Object
    }

    private void Start()
    {
        GrassContainer = new GameObject("GrassContainer");
        GrassContainer.transform.SetParent(GameObject.Find("Track").transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_allowCreation)
        {
            if (GetComponent<SelectBox>().m_updating) // if the select square if updating 
            {
                m_maxGrassInArea = GetComponent<SelectBox>().m_area / 3000; // Divides to get the max trees in an area

                if (m_currentGrassCountForBox <= m_maxGrassInArea)
                {
                    scatterGrass(GetComponent<SelectBox>().m_end, GetComponent<SelectBox>().m_start);
                }
            }


            if (Input.GetMouseButtonUp(1))
            {
                m_currentGrassCountForBox = 0;
            }

            if (Input.GetMouseButtonUp(1))
            {
                m_maxGrassInArea = 0;
            }
        }
    }

    public void scatterGrass(Vector3 startPos, Vector3 endPos)
    {
        Vector3 m_posVector = new Vector3(Random.Range(startPos.x, endPos.x), 1.25f, Random.Range(startPos.z, endPos.z));
        GameObject treeTemp = Instantiate(Grass, GameObject.FindGameObjectWithTag("Track").transform);
        treeTemp.transform.localPosition = m_posVector;
        treeTemp.transform.localRotation = Quaternion.identity;
        GrassList.Add(treeTemp);
        m_currentGrassCountForBox++;
    }

    public void toggle()
    {
        if (m_allowCreation)
        {
            m_allowCreation = false;
        }
        else
        {
            m_allowCreation = true;
        }
    }
}
