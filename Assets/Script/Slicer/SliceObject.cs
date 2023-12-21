using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using UnityEngine.InputSystem;

public class SliceObject : MonoBehaviour
{
    public Transform startSlicePoint;
    public Transform endSlicePoint;

    public float minSliceVelocity = 1f;
    public VelocityEstimator velocityEstimator;
    public LayerMask sliceableLayer;

    public Material crossSectionMaterial;
    public float cutForce = 2000f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer))
        {
            GameObject target = hit.transform.gameObject;
            Slice(target);
        }
    }

    public void Slice(GameObject target)
    {
        Debug.Log("Sliced");

        Vector3 velocity = velocityEstimator.GetVelocityEstimate();

        if (velocity.magnitude < minSliceVelocity)
        {
            return;
        }

        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity);
        planeNormal.Normalize();

        // Call the method to create the quad
        //CreateSliceQuad(endSlicePoint.position, planeNormal);

        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

        if(hull != null)
        {
            Debug.Log("Creating Upper and Lower");

            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
            upperHull.name = "Upper Hull";
            SetupSlicedCompent(upperHull, 1f, 3f);

            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
            lowerHull.name = "Lower Hull";
            SetupSlicedCompent(lowerHull, 0, 10f);

            Destroy(target);
        }
        else
        {
            Debug.Log("No Hull");
        }
    }

    public void SetupSlicedCompent(GameObject slicedObject, float explosionForce, float mass)
    {
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        rb.mass = mass;
        rb.AddExplosionForce(cutForce, slicedObject.transform.position, explosionForce);

        WeaponLayerManager.Instance.ChangeLayer(slicedObject, 0.5f, "Slice");
    }

    void CreateSliceQuad(Vector3 quadCenter, Vector3 planeNormal)
    {
        float quadSize = 1f; // Set this as needed

        GameObject quad = new GameObject("SlicingQuad");
        MeshFilter meshFilter = quad.AddComponent<MeshFilter>();
        quad.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        Material material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        material.color = Color.white;

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

        quad.GetComponent<MeshRenderer>().material = material;
    }
}
