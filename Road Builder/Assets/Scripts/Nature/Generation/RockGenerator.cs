using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockGenerator : MonoBehaviour
{
    int rockSeed; // Seed for random gen
    private List<Vector3> m_vertices = new List<Vector3>(); // Vertices of the mesh
    private List<Vector3> usedVetices = new List<Vector3>();  

    private Vector3 center; // Center of the rock

    // Start is called before the first frame update
    void Awake()
    {
        // Random Variables for the generation
        transform.localScale = new Vector3(Random.Range(1, 3), Random.Range(1, 3), Random.Range(1, 3));
        rockSeed = Random.Range(1, 200000);
        Random.InitState(rockSeed);
        float m_offset = Random.Range(1, 200000); // ANotehr value to help up the noise value

        Mesh mesh = GetComponent<MeshFilter>().mesh;

        // Add all the vertices from the mesh
        for (int s = 0; s < mesh.vertices.Length; s++)
        {
            m_vertices.Add(mesh.vertices[s]);
        }

        // Gets the center of the object
        center = GetComponent<Renderer>().bounds.center;

        // Run throught the meshes vertices
        for (int v = 0; v < m_vertices.Count; v++)
        {
            // A bool to check to see ifits been used
            bool used = false;

            // checks them all to see which ones have changed 
            for (int k = 0; k < usedVetices.Count; k++)
            {
                if (usedVetices[k] == m_vertices[v])
                {
                    used = true;
                }
            }
            if (!used) // if its not been changed 
            {
                Vector3 curVector = m_vertices[v]; // Gets the current vertice
                usedVetices.Add(curVector); // Adds it 
                int m_smoothValue = Random.Range(2, 10); // Smoothing so its not so jagged.
                Vector3 changedVector = (curVector - ((curVector - center) * (Mathf.PerlinNoise(((float)v / m_offset) / m_smoothValue, 
                                                                                                 (float)v / m_offset) / m_smoothValue))); // change the vertices point using perlin noise
                // change the vector
                for (int s = 0; s < m_vertices.Count; s++)
                {
                    if (m_vertices[s] == curVector)
                    {
                        m_vertices[s] = changedVector;
                    }
                }
            }
        }

        // Sets the vertices.
        mesh.SetVertices(m_vertices);
        mesh.RecalculateBounds();
    }
}
