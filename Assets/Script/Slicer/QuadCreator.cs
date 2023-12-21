using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadCreator : MonoBehaviour
{
    public Transform startSlicePoint;
    public Transform endSlicePoint;
    public VelocityEstimator velocityEstimator; // Ensure this component is attached and properly set up
    public float quadSize = 1f;

    void Start()
    {
        Vector3 velocity = velocityEstimator.GetVelocityEstimate();
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity);
        planeNormal.Normalize();

        Vector3 quadCenter = (startSlicePoint.position + endSlicePoint.position) / 2;

        GameObject quad = new GameObject("CustomQuad");
        MeshFilter meshFilter = quad.AddComponent<MeshFilter>();
        quad.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        // Define vertices and triangles as in the previous example
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-quadSize, -quadSize, 0),
            new Vector3(quadSize, -quadSize, 0),
            new Vector3(quadSize, quadSize, 0),
            new Vector3(-quadSize, quadSize, 0)
        };

        // Triangles
        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 0, 3
        };

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        // Align quad normal with planeNormal and position it
        quad.transform.up = planeNormal;
        quad.transform.position = quadCenter;
    }
}
