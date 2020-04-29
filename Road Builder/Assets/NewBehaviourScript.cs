using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Texture m_woodTexture;

    Renderer m_Renderer;
    private Material myNewMaterial;


    // Start is called before the first frame update
    void Start()
    {
        myNewMaterial = new Material(Shader.Find("Standard"));
        m_Renderer = GetComponent<Renderer>();
        m_Renderer.material = myNewMaterial;
        m_Renderer.material.SetTexture("_MainTex", m_woodTexture);
        m_Renderer.material.color = Color.blue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
