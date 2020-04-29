using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeCreator : MonoBehaviour
{
    // Grass Container
    GameObject Forest;

    List<GameObject> Trees = new List<GameObject>(); // List of all the road tiles in the path.
    int m_maxTreesInArea;
    int m_currentTreeCountForBox;
    public static bool m_allowCreation;


    List<Vector3> realPoints;
    GameObject tree;


    private void Awake()
    {
        tree = Resources.Load("Tree") as GameObject;
    }

    private void Start()
    {
        m_allowCreation = false;
        Forest = new GameObject("Forest");
        Forest.transform.SetParent(GameObject.Find("Track").transform);
    }

    // Update is called once per frame
    void Update()
    {
        // if the box has been ticketed 
        if (m_allowCreation)
        {
            if (Input.GetMouseButtonUp(1))
            {
                realPoints = PointsOfTrees(10, 10);
                for(int i = 0; i < realPoints.Count;i++)
                {
                    scatterTREESTest(realPoints[i]);
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                m_maxTreesInArea = 0;
            }

            if(Input.GetMouseButtonDown(1))
            {
                m_currentTreeCountForBox = 0;
            }
        }
    }

    public void scatterTREES(Vector3 startPos, Vector3 endPos)
    {
        Vector3 m_posVector = new Vector3(Random.Range(startPos.x, endPos.x), 1.3f, Random.Range(startPos.z, endPos.z));
        GameObject treeTemp = Instantiate(tree, GameObject.FindGameObjectWithTag("Track").transform);
        treeTemp.transform.localPosition = m_posVector;
        treeTemp.transform.localRotation = Quaternion.identity;


        Trees.Add(treeTemp);
        m_currentTreeCountForBox++;
    }

    public void scatterTREESTest(Vector3 pos)
    {
        Vector3 m_posVector = pos;
        GameObject treeTemp = Instantiate(tree, GameObject.FindGameObjectWithTag("Track").transform);
        treeTemp.transform.localPosition = m_posVector;
        treeTemp.transform.localRotation = Quaternion.identity;

        Trees.Add(treeTemp);
    }


    public void toggle()
    {
        if(m_allowCreation)
        {
            m_allowCreation = false;
        }
        else
        {
            m_allowCreation = true;
        }
    }

    //Placement 
    List<Vector3> PointsOfTrees(float radiusBetween,int numberOfChecks)
    {
        List<Vector3> actualPoints = new List<Vector3>();
        List<Vector3> tempPoints = new List<Vector3>();

        tempPoints.Add(new Vector3(GetComponent<SelectBox>().m_centre.x,0, GetComponent<SelectBox>().m_centre.z));
        while(m_currentTreeCountForBox < GetComponent<SelectBox>().m_area / 3000)
        {
            int spawnIndex = Random.Range(0, tempPoints.Count);
            Vector3 spawnCentre = tempPoints[spawnIndex];
            bool accepted = false;
            for(int i = 0; i < numberOfChecks; i++)
            {
                float angle = Random.value * Mathf.PI * 2;
                Vector3 dir = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle),Mathf.Tan(angle));
                Vector3 candidate = spawnCentre + dir * Random.Range(radiusBetween, 2 * radiusBetween);

                candidate = new Vector3(candidate.x, 1.3f, candidate.z);

                if(isOk(candidate,radiusBetween,actualPoints))
                {
                    m_currentTreeCountForBox++;
                    actualPoints.Add(candidate);
                    tempPoints.Add(candidate);
                    accepted = true;
                    break;
                }
            }

            if (!accepted)
            {
                tempPoints.RemoveAt(spawnIndex);
            }
        }
        return actualPoints;
    }

    bool isOk(Vector3 t_pos,float radius,List<Vector3> realPoints)
    {
        Vector3 startCorner = GetComponent<SelectBox>().m_start;
        Vector3 endCorner = GetComponent<SelectBox>().m_end;

        float leftSide = 0;
        if(startCorner.x < endCorner.x)
        {
            leftSide = startCorner.x;
        }
        else
        {
            leftSide = endCorner.x;
        }

        float rightSide = 0;
        if (startCorner.x > endCorner.x)
        {
            rightSide = startCorner.x;
        }
        else
        {
            rightSide = endCorner.x;
        }

        float BottomSide = 0;
        if (startCorner.z < endCorner.z)
        {
            BottomSide = startCorner.z;
        }
        else
        {
            BottomSide = endCorner.z;
        }

        float TopSide = 0;
        if (startCorner.z > endCorner.z)
        {
            TopSide = startCorner.z;
        }
        else
        {
            TopSide = endCorner.z;
        }

        if(t_pos.x < rightSide && t_pos.x > leftSide && t_pos.z < TopSide && t_pos.z > BottomSide)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
