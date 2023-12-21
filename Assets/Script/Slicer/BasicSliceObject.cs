using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using UnityEngine.InputSystem;

public class BasicSliceObject : MonoBehaviour
{
    public Transform planeDebug;
    public GameObject target;

    public Material crossSectionMaterial;
    public float cutForce = 2000f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Slice(target);
        }
    }

    public void Slice(GameObject target)
    {
        Debug.Log("Sliced");

        CreateSliceQuad(planeDebug.position, planeDebug.up);

        SlicedHull hull = target.Slice(planeDebug.position, planeDebug.up);

        if (hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
            SetupSlicedCompent(upperHull, 1f, 3f);

            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
            SetupSlicedCompent(lowerHull, 0, 10f);
        }

        Destroy(target);
    }

    public void SetupSlicedCompent(GameObject slicedObject, float explosionForce, float mass)
    {
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        rb.mass = mass;
        rb.AddExplosionForce(cutForce, slicedObject.transform.position, explosionForce);
    }

    void CreateSliceQuad(Vector3 quadCenter, Vector3 planeNormal)
    {
        float quadSize = 1f; // Set this as needed

        GameObject quad = new GameObject("SlicingQuad");
        MeshFilter meshFilter = quad.AddComponent<MeshFilter>();
        quad.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        // Vertices and triangles as before
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-quadSize, 0, -quadSize),
            new Vector3(quadSize, 0, -quadSize),
            new Vector3(quadSize, 0, quadSize),
            new Vector3(-quadSize, 0, quadSize)
        };

        // Triangles for both sides
        int[] tris = new int[12] // 6 indices per side
        {
            // Front Side
            0, 2, 1, 2, 0, 3,
            // Back Side
            0, 1, 2, 0, 2, 3
        };

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        // Align quad normal with planeNormal and position it
        quad.transform.up = planeNormal;
        quad.transform.position = quadCenter;
    }
}
