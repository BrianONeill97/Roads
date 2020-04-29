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

    GameObject WaterTiles;


    private void Awake()
    {
        wave = Resources.Load("Water Tile") as GameObject;
        WaterTiles = new GameObject("Water");
        WaterTiles.transform.SetParent(GameObject.Find("Track").transform);
        WaterTiles.transform.SetAsFirstSibling();
    }

    private void Start()
    {

        temp = GetComponent<TiledRoadCreator>().plains[Random.Range(0, GetComponent<TiledRoadCreator>().plains.Count)].gameObject.transform.localPosition;

        edgeX = spacingCheckX * GetComponent<TiledRoadCreator>().gridX;
        edgeZ = spacingCheckZ * GetComponent<TiledRoadCreator>().gridZ;

        Create(temp, Utility.GetSize(GetComponent<TiledRoadCreator>().grassTile));
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
                    waterTile.transform.parent = WaterTiles.transform;
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
                    waterTile.transform.parent = WaterTiles.transform;
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
                    waterTile.transform.parent = WaterTiles.transform;
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
                if (water[i].gameObject.transform.localPosition.x > edgeX || water[i].gameObject.transform.localPosition.x <= -edgeX ||
                    water[i].gameObject.transform.localPosition.z > edgeZ || water[i].gameObject.transform.localPosition.z < -edgeZ)
                {
                    Destroy(water[i].gameObject);
                    water.RemoveAt(i);
                }
            }
        }
    }
}
