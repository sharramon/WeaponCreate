using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLayerManager : Singleton<WeaponLayerManager>
{
    public void ChangeLayer(GameObject gameObject, float time, string layerName)
    {
        StartCoroutine(ChangeLayerNameAfterTime(gameObject, 0.5f, layerName));
    }

    private IEnumerator ChangeLayerNameAfterTime(GameObject gameObject, float time, string LayerName)
    {
        yield return new WaitForSeconds(time);
        gameObject.layer = LayerMask.NameToLayer("Slice");
    }

    public void CreateSliceQuad(Vector3 quadCenter, Vector3 planeNormal)
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
