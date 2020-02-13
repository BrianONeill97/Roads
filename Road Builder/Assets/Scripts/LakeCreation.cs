using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LakeCreation : MonoBehaviour
{
    GameObject wave;
    public int WaterTileCount = 0;
    public int i = 0;
    Vector3 temp;

    string prevDirection = "";

    List<GameObject> water = new List<GameObject>();

    private const float spacingCheckX = 1.5f;
    private const float spacingCheckZ = 1.5f;

    private float edgeX = 0f;
    private float edgeZ = 0f;


    private void Awake()
    {
        wave = Resources.Load("Water Tile") as GameObject;
    }

    private void Start()
    {

        temp = GetComponent<TiledRoadCreator>().plains[Random.Range(0, GetComponent<TiledRoadCreator>().plains.Count)].gameObject.transform.localPosition;

        edgeX = spacingCheckX * GetComponent<TiledRoadCreator>().gridX;
        edgeZ = spacingCheckZ * GetComponent<TiledRoadCreator>().gridZ;

        Create(temp, GetComponent<TiledRoadCreator>().GetSize(GetComponent<TiledRoadCreator>().grassTile));
    }

    public void Create(Vector3 temp, Vector3 offSet)
    {
        Vector3 newPos = temp;

        //Make 4 collider hits around on each side and if one is null then stop

        if (i < WaterTileCount)
        {
            int Chance = Random.Range(0,3);
            if (Chance == 0 && prevDirection != "down")
            {
                Collider[] hitOne = Physics.OverlapSphere(newPos, 1.5f);
                if(hitOne.Length != null)
                {
                    GameObject waterTile = Instantiate(wave, newPos, Quaternion.identity);
                    newPos = new Vector3(temp.x, temp.y, temp.z + offSet.z);
                    prevDirection = "up";
                    water.Add(waterTile);
                }
                else
                {
                    return;
                }
            }
            else if (Chance == 1 && prevDirection != "right")
            {
                Collider[] hitOne = Physics.OverlapSphere(newPos, 0);
                if (hitOne.Length != null)
                {
                    GameObject waterTile = Instantiate(wave, newPos, Quaternion.identity);
                    newPos = new Vector3(temp.x - offSet.x, temp.y, temp.z);
                    prevDirection = "left";
                    water.Add(waterTile);
                }
                else
                {
                    return;
                }
            }
            else if (Chance == 2 && prevDirection != "left")
            {
                Collider[] hitOne = Physics.OverlapSphere(newPos, 0);
                if (hitOne.Length != null)
                {
                    GameObject waterTile = Instantiate(wave, newPos, Quaternion.identity);
                    newPos = new Vector3(temp.x + offSet.x, temp.y, temp.z);
                    prevDirection = "right";
                    water.Add(waterTile);
                }
                else
                {
                    return;
                }
            }
            i++;
            Create(newPos, offSet);
        }

        for (int i = 0; i < water.Count; i++)
        {
            if(water[i].gameObject != null)
            {
                if (water[i].gameObject.transform.localPosition.x > edgeX || water[i].gameObject.transform.localPosition.x < -edgeX ||
                    water[i].gameObject.transform.localPosition.z > edgeZ || water[i].gameObject.transform.localPosition.z < -edgeZ)
                {
                    Destroy(water[i].gameObject);
                    water.RemoveAt(i);
                    Debug.Log("Removing Tile Outside bounds");
                }
            }
        }
    }

    public void deleteAndReCreate()
    {
        temp = new Vector3();
        prevDirection = "";
        i = 0;
        for (int i = 0; i < water.Count; i++)
        {
            Destroy(water[i].gameObject);
            water.RemoveAt(i);
        }

        if (GetComponent<TiledRoadCreator>().plains.Count > 0)
        {
            temp = GetComponent<TiledRoadCreator>().plains[Random.Range(0, GetComponent<TiledRoadCreator>().plains.Count)].gameObject.transform.localPosition;
            Create(temp, GetComponent<TiledRoadCreator>().GetSize(GetComponent<TiledRoadCreator>().grassTile));
        }
    }
}
