using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBox : MonoBehaviour
{
    public Camera thisCamera;
    [SerializeField]
    private RectTransform selectSquareImage;

    [HideInInspector]
    public Vector3 m_start;
    public Vector3 m_end;

    bool m_updating = false;
    // Start is called before the first frame update
    void Start()
    {
        selectSquareImage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            selectSquareImage.gameObject.SetActive(true);

            m_start = _get3dMousePosition();
        }

        if (Input.GetMouseButtonUp(1))
        {
            selectSquareImage.gameObject.SetActive(false);
            m_start = Vector3.zero;
            m_end = Vector3.zero;
            m_updating = false;
        }

        if (Input.GetMouseButton(1))
        {
            if (!selectSquareImage.gameObject.activeInHierarchy)
            {
                selectSquareImage.gameObject.SetActive(true);
            }
            m_end = _get3dMousePosition();

            Vector3 m_centre = (m_start + m_end) / 2;
            selectSquareImage.position = m_centre;



            float m_sizeX = Mathf.Abs(m_start.x - m_end.x);
            float m_sizeY = Mathf.Abs(m_start.z - m_end.z);

            selectSquareImage.sizeDelta = new Vector2(m_sizeX * 6, m_sizeY * 6);

            m_updating = true;
        }
    }

    //Gets the mouse position in the game
    public Vector3 _get3dMousePosition()
    {
        Vector3 newPos = new Vector3();
        newPos = Input.mousePosition;
        newPos.z = thisCamera.farClipPlane - 5;
        newPos = thisCamera.ScreenToWorldPoint(newPos);
        return newPos;
    } // Gets the mouse position 

    //Gets magnitude
    float GetMag(Vector3 start, Vector3 end)
    {
        float mag = Mathf.Sqrt(((start.x * end.x) + (start.y * start.y) + (start.z * end.z)));
        return mag;
    } // Gets the magnitude of the line Vector
}
