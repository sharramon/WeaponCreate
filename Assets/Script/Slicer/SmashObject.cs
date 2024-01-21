using EzySlice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SmashObject : MonoBehaviour
{
    public float minSliceVelocity = 1f;
    public VelocityEstimator velocityEstimator;
    public LayerMask sliceableLayer;
    public Material crossSectionMaterial;

    public float cutPosNoise = 0.1f;
    public int cutCount = 5;
    public float distanceToSmash = 0.1f;
    public float cutForce = 500f;

    private List<GameObject> objectsToSmash = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        int otherLayerMask = 1 << other.gameObject.layer;

        if (otherLayerMask == sliceableLayer.value)
        {
            if(other.gameObject.GetComponent<Sliceable>() != null)
            {
                if(other.gameObject.GetComponent<Sliceable>()._isSliceable == true)
                {
                    GameObject target = other.gameObject;
                    Smash(target);
                }
            }
        }
    }

    public void Smash(GameObject target)
    {
        Vector3 velocity = velocityEstimator.GetVelocityEstimate();

        Debug.Log($"Velocity is {velocity}");

        if (velocity.magnitude < minSliceVelocity)
        {
            return;
        }

        Debug.Log("Velocity Passed");

        Vector3 smashPos = transform.position + velocity.normalized * distanceToSmash;
        objectsToSmash.Add(target);

        for (int i = 0; i < cutCount; i++)
        {
            SubSmash(smashPos);
        }
    }

    public void SubSmash(Vector3 targetPos)
    {
        // Temporary lists inside the method
        List<GameObject> newObjects = new List<GameObject>();
        List<GameObject> objectsToRemove = new List<GameObject>();

        foreach (GameObject smashObject in objectsToSmash) {

            //get random plane normal
            //Vector3 planeNormal = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            Vector3 planeNormal = Random.onUnitSphere;

            //get random plane position centered around targetPos but with added noise
            Vector3 planePos = targetPos + new Vector3(Random.Range(-cutPosNoise, cutPosNoise), Random.Range(-cutPosNoise, cutPosNoise), Random.Range(-cutPosNoise, cutPosNoise));

            //WeaponLayerManager.Instance.CreateSliceQuad(planePos, planeNormal);

            SlicedHull hull = smashObject.Slice(planePos, planeNormal);

            if(hull != null)
            {
                GameObject upperHull = hull.CreateUpperHull(smashObject, crossSectionMaterial);
                upperHull.name = "Upper Hull";
                SetupSlicedCompent(upperHull, 1f, 3f);

                GameObject lowerHull = hull.CreateLowerHull(smashObject, crossSectionMaterial);
                lowerHull.name = "Lower Hull";
                SetupSlicedCompent(lowerHull, 0, 10f);

                // Add new objects to the newObjects list
                newObjects.Add(upperHull);
                newObjects.Add(lowerHull);

                // Mark the current object for removal
                objectsToRemove.Add(smashObject);
            }
            else
            {
                Debug.Log("No Hull");
            }
        }

        // Update the original list
        foreach (GameObject obj in objectsToRemove)
        {
            objectsToSmash.Remove(obj);
            Destroy(obj);
        }
        objectsToSmash.AddRange(newObjects);
    }

    public void SetupSlicedCompent(GameObject slicedObject, float explosionForce, float mass)
    {
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        rb.mass = mass;
        rb.AddExplosionForce(cutForce, this.transform.position, explosionForce);

        //WeaponLayerManager.Instance.ChangeLayer(slicedObject, 0.5f, "Slice");
    }
}
