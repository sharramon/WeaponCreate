using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Oculus;
using FullMetal;

public class ArrowHand : MonoBehaviour
{
    [SerializeField] private Longbow bow;

    [SerializeField] private bool _isLeft = false;
    [SerializeField] private OVRInput.Button nockButton;
    [SerializeField] private float m_allowedDistanceForNock = 1f;

    private bool nocked = false;

    private GameObject currentArrow;
    public GameObject arrowPrefab;

    public Transform arrowNockTransform;
    public Transform arrowNockBowTransform;

    public float nockDistance = 0.1f;
    public float lerpCompleteDistance = 0.08f;
    public float rotationLerpThreshold = 0.15f;
    public float positionLerpThreshold = 0.15f;

    private bool allowArrowSpawn = true;

    private bool inNockRange = false;
    private bool arrowLerpComplete = false;

    public int maxArrowCount = 10;
    private Queue<GameObject> arrowQueue = new Queue<GameObject>();

    public SoundPlayOneshot arrowSpawnSound;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (OVRInput.GetDown(nockButton))
        {
            Debug.Log("Nock button pressed");
            CheckDistanceToNock();
            //arrowSpawnSound.Play();
        }
        
        else if (OVRInput.GetUp(nockButton))
        {
            ReleaseNock();
        }
        

        if(nocked)
        {
            //HandAttachedUpdate();
        }
    }

    public void SetBow(Longbow _bow)
    {
        bow = _bow;

        if (_bow == null)
            return;

        arrowNockBowTransform = _bow.nockTransform;
    }

    public Longbow GetBow()
    {
        return bow;
    }

    private void CheckDistanceToNock()
    {
        if(bow == null)
        {
            Debug.Log($"No bow in {this.transform.name}");
            return;
        }


        float distanceToNock = Vector3.Distance(this.transform.position, bow.transform.position);
        Debug.Log($"Distance to nock is {distanceToNock}");
        if (distanceToNock < m_allowedDistanceForNock)
        {
            currentArrow = InstantiateArrow();
            bow.StartNock();
        }
    }

    private void ReleaseNock()
    {
        FireArrow();
    }

    private GameObject InstantiateArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, arrowNockBowTransform.position, arrowNockBowTransform.rotation) as GameObject;
        arrow.name = "Bow Arrow";
        arrow.transform.parent = arrowNockBowTransform;
        Util.ResetTransform(arrow.transform);

        arrowQueue.Enqueue(arrow);

        while (arrowQueue.Count > maxArrowCount)
        {
            GameObject oldArrow = arrowQueue.Dequeue();

            if (oldArrow)
            {
                Destroy(oldArrow);
            }
        }

        return arrow;
    }

    private void HandAttachedUpdate()
    {
        if (bow == null)
            return;

        float distanceToNockPosition = Vector3.Distance(transform.parent.position, bow.nockTransform.position);

        
        //// If there's an arrow spawned in the hand and it's not nocked yet
        //if (!nocked && currentArrow != null)
        //{
        //    // If we're close enough to nock position that we want to start arrow rotation lerp, do so
        //    if (distanceToNockPosition < rotationLerpThreshold)
        //    {
        //        float lerp = Util.RemapNumber(distanceToNockPosition, rotationLerpThreshold, lerpCompleteDistance, 0, 1);

        //        arrowNockTransform.rotation = Quaternion.Lerp(arrowNockTransform.parent.rotation, bow.nockRestTransform.rotation, lerp);
        //    }
        //    else // Not close enough for rotation lerp, reset rotation
        //    {
        //        arrowNockTransform.localRotation = Quaternion.identity;
        //    }

        //    // If we're close enough to the nock position that we want to start arrow position lerp, do so
        //    if (distanceToNockPosition < positionLerpThreshold)
        //    {
        //        float posLerp = Util.RemapNumber(distanceToNockPosition, positionLerpThreshold, lerpCompleteDistance, 0, 1);

        //        posLerp = Mathf.Clamp(posLerp, 0f, 1f);

        //        arrowNockTransform.position = Vector3.Lerp(arrowNockTransform.parent.position, bow.nockRestTransform.position, posLerp);
        //    }
        //    else // Not close enough for position lerp, reset position
        //    {
        //        arrowNockTransform.position = arrowNockTransform.parent.position;
        //    }

        //    //lerp visually complete, set nocked to true
        //    if (distanceToNockPosition < lerpCompleteDistance)
        //    {
        //        if (!arrowLerpComplete)
        //        {
        //            arrowLerpComplete = true;
        //        }
        //    }
        //    else
        //    {
        //        if (arrowLerpComplete)
        //        {
        //            arrowLerpComplete = false;
        //        }
        //    }

        //    // Allow nocking the arrow when controller is close enough
        //    if (distanceToNockPosition < nockDistance)
        //    {
        //        if (!inNockRange)
        //        {
        //            inNockRange = true;
        //            bow.ArrowInPosition();
        //        }
        //    }
        //    else
        //    {
        //        if (inNockRange)
        //        {
        //            inNockRange = false;
        //        }
        //    }

        //    // If we're close enough to the nock position and the arrow is visually lerp complete, nock the arrow
        //    // I might need to add the 'grab' logic with hand closed logic
        //    if ((distanceToNockPosition < nockDistance) && !nocked)
        //    {
        //        if (currentArrow == null)
        //        {
        //            currentArrow = InstantiateArrow();
        //        }

        //        nocked = true;
        //        bow.StartNock();
        //        currentArrow.transform.parent = bow.nockTransform;
        //        Util.ResetTransform(currentArrow.transform);
        //        Util.ResetTransform(arrowNockTransform);
        //    }
        //}

        // If arrow is nocked, and we release the trigger
        // Have to add 'release' logic from hand check
        if (nocked)
        {
            if (bow.pulled) // If bow is pulled back far enough, fire arrow, otherwise reset arrow in arrowhand
            {
                FireArrow();
            }
            else
            {
                /*
                arrowNockTransform.rotation = currentArrow.transform.rotation;
                currentArrow.transform.parent = arrowNockTransform;
                Util.ResetTransform(currentArrow.transform);
                nocked = false;
                bow.ReleaseNock();
                */
            }

            //bow.StartRotationLerp(); // Arrow is releasing from the bow, tell the bow to lerp back to controller rotation
        }
    }
        

    private void FireArrow()
    {
        if (currentArrow == null)
            return;

        currentArrow.transform.parent = null;

        Arrow arrow = currentArrow.GetComponent<Arrow>();

        arrow.shaftRB.isKinematic = false;
        arrow.shaftRB.useGravity = true;
        arrow.shaftRB.transform.GetComponent<BoxCollider>().enabled = true;
        arrow.shaftRB.interpolation = RigidbodyInterpolation.Extrapolate;

        arrow.arrowHeadRB.isKinematic = false;
        arrow.arrowHeadRB.useGravity = true;
        arrow.arrowHeadRB.transform.GetComponent<BoxCollider>().enabled = true;


        arrow.arrowHeadRB.AddForce(currentArrow.transform.forward * bow.GetArrowVelocity(), ForceMode.VelocityChange);
        //arrow.arrowHeadRB.AddForce(arrow.arrowHeadRB.transform.forward * bow.GetArrowVelocity(), ForceMode.VelocityChange);
        //arrow.shaftRB.AddForce(currentArrow.transform.forward * bow.GetArrowVelocity(), ForceMode.VelocityChange);
        //arrow.shaftRB.velocity = currentArrow.transform.forward * bow.GetArrowVelocity() * 10;
        arrow.arrowHeadRB.AddTorque(currentArrow.transform.forward * 100);

        nocked = false;

        currentArrow.GetComponent<Arrow>().ArrowReleased();
        bow.ArrowReleased();

        allowArrowSpawn = false;
        Invoke("EnableArrowSpawn", 0.5f);

        currentArrow = null;
    }
}
