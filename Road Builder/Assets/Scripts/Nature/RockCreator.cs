using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockCreator : MonoBehaviour
{
    // Overall Rock Container
    GameObject RocksContainer;
    List<GameObject> Rocks = new List<GameObject>(); // List of all the road tiles in the path.
    GameObject rock;
    public int m_maxRocksInArea;
    int m_currentRockCountForBox;
    public static bool m_allowCreation = false;


    private void Awake()
    {
        rock = Resources.Load("Rock") as GameObject;
    }

    private void Start()
    {
        RocksContainer = new GameObject("RocksContainer");
        RocksContainer.transform.SetParent(GameObject.Find("Track").transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_allowCreation)
        {
            if (GetComponent<SelectBox>().m_updating)
            {
                m_maxRocksInArea = GetComponent<SelectBox>().m_area / 3000;

                if (m_currentRockCountForBox <= m_maxRocksInArea)
                {
                    scatterTREES(GetComponent<SelectBox>().m_end, GetComponent<SelectBox>().m_start);
                }
            }


            if (Input.GetMouseButtonUp(1))
            {
                m_currentRockCountForBox = 0;
            }

            if (Input.GetMouseButtonUp(1))
            {
                m_maxRocksInArea = 0;
            }
        }
    }

    public void scatterTREES(Vector3 startPos, Vector3 endPos)
    {
        Vector3 m_posVector = new Vector3(Random.Range(startPos.x, endPos.x), 1.3f, Random.Range(startPos.z, endPos.z));
        GameObject treeTemp = Instantiate(rock, RocksContainer.transform);
        treeTemp.transform.localPosition = m_posVector;
        treeTemp.transform.localRotation = Quaternion.identity;
        Rocks.Add(treeTemp);
        m_currentRockCountForBox++;
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
