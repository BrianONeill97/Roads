using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeStats : MonoBehaviour
{
    MeshRenderer m_mr;
    public int m_age;
    float scaling;
    // Start is called before the first frame update
    void Start()
    {
        m_mr = gameObject.GetComponent<MeshRenderer>();
        m_age = Random.Range(   int.Parse(GameObject.Find("TreeMinAge").GetComponent<InputField>().text),
                                int.Parse(GameObject.Find("TreeMaxAge").GetComponent<InputField>().text));
        scale();
        changeColor();
    }

    void scale()
    {
        if (m_age > 50)
        {
            scaling = m_age / 2;
            gameObject.transform.localScale += new Vector3((scaling / 2) / 100, scaling / 100, (scaling / 2) / 100);
            gameObject.transform.localPosition -= new Vector3(+(scaling / 2) / 100, (scaling / 100) +0.2f, +(scaling / 2) / 100);
        }
        else
        { 
            scaling = m_age / 2;
            gameObject.transform.localScale -= new Vector3(scaling / 100, m_age / 100, scaling / 100);
            gameObject.transform.localPosition += new Vector3(-scaling / 100, (m_age / 100) - 0.2f, -scaling / 100);

        }
    }

    void changeColor()
    {
        for(int i = 0; i < m_mr.materials.Length;i++)
        {
            if (m_mr.materials[i].name.Contains("Leaves"))
            {
                m_mr.materials[i].color = new Color(Random.Range(0.24f,0.56f),
                                                    Random.Range(0.65f, 0.68f),
                                                    Random.Range(0.36f, 0.37f));
            }
        }
    }


}
