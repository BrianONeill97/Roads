using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadCreator : MonoBehaviour
{

    public Vector3 startP;
    public Vector3 endP;
    private Vector3 worldPoint;
    private Vector3 currentP;

    private GameObject part;

    public Camera cam;

    public GameObject obj;

    private List<Vector3> Pos = new List<Vector3>();


    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0,0,15));

            if (!Pos.Contains(worldPoint))
            {           
                    Pos.Add(worldPoint);
            }

            if (Pos.Count > 2)
            {
                for (int i = 0; i < Pos.Count; i++)
                {
                    Debug.Log(Pos[i]);
                    //Spawn(Pos[i]);
                }
            }
        }
    }

    public void Spawn(Vector3 pos)
    {
            part = Instantiate(obj, transform);
            part.transform.parent = gameObject.transform;
            part.transform.position = pos;
            part.transform.rotation = Quaternion.identity;
    }
}
