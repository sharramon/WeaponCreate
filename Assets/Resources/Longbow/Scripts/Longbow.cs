using PhantoUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Longbow : MonoBehaviour
{
    //
    private ArrowHand arrowHand;

    public Transform pivotTransform;
    public Transform handleTransform;

    public Transform nockTransform;
    public Transform nockRestTransform;

    public bool nocked = false;
    public bool pulled;

    private float drawTension;
    private const float minPull = 0.01f;
    private const float maxPull = 0.2f;
    private float nockDistanceTravelled = 0f;
    private float hapticDistanceThreshold = 0.01f;
    private float lastTickDistance;
    private const float bowPullPulseStrengthLow = 100;
    private const float bowPullPulseStrengthHigh = 500;

    //arrow speed
    public float arrowMinVelocity = 3f;
    public float arrowMaxVelocity = 30f;
    private float arrowVelocity = 3f;

    //strain sound
    private float minStrainTickTime = 0.1f;
    private float maxStrainTickTime = 0.5f;
    private float nextStrainTick = 0;

    //bow lerp
    private bool lerpBackToZeroRotation;
    private float lerpStartTime;
    private float lerpDuration = 0.15f;
    private Quaternion lerpStartRotation;

    private float nockLerpStartTime;
    private Quaternion nockLerpStartRotation;

    public float drawOffset = 0.06f;

    public LinearMapping bowDrawLinearMapping;

    [Header("Sounds")]
    public SoundBowClick drawSound;
    public SoundPlayOneshot arrowSlideSound;
    public SoundPlayOneshot releaseSound;
    public SoundPlayOneshot nockSound;

    private void FixedUpdate()
    {
        BowUpdate();
    }
    public void SetArrowhand(ArrowHand _arrowHand)
    {
        arrowHand = _arrowHand;
        //_arrowHand.arrowNockTransform = nockRestTransform;
    }

    private void BowUpdate()
    {
        if(nocked)
        {
            Vector3 nockToarrowHand = arrowHand.arrowNockTransform.parent.position - nockRestTransform.position;

            // Align bow
            // Time lerp value used for ramping into drawn bow orientation
            float lerp = Util.RemapNumberClamped(Time.time, nockLerpStartTime, nockLerpStartTime + 0.1f, 0, 1);
            float pullLerp = Util.RemapNumberClamped(nockToarrowHand.magnitude, minPull, maxPull, 0, 1); //Normalized current state of bow draw 0 - 1

            Vector3 arrowNockTransformToHeadset = ((Camera.main.transform.position + (Vector3.down * 0.05f)) - arrowHand.arrowNockTransform.parent.position).normalized;
            Vector3 arrowHandPosition = (arrowHand.arrowNockTransform.parent.position + ((arrowNockTransformToHeadset * drawOffset) * pullLerp)); // Use this line to lerp arrowHand nock position

            Vector3 pivotToString = (arrowHandPosition - pivotTransform.position).normalized;
            Vector3 pivotToLowerHandle = (handleTransform.position - pivotTransform.position).normalized;

            // Move nock position
            Debug.Log($"The dot product is {Vector3.Dot(nockToarrowHand, -nockTransform.forward)}");
            if (Vector3.Dot(nockToarrowHand, -nockTransform.forward) > 0)
            {
                float distanceToarrowHand = nockToarrowHand.magnitude * lerp;

                nockTransform.localPosition = new Vector3(0f, 0f, (float)(Mathf.Clamp(-distanceToarrowHand, -maxPull, 0f) * (0.5 / maxPull)));

                nockDistanceTravelled = -nockTransform.localPosition.z;

                arrowVelocity = Util.RemapNumber(nockDistanceTravelled, minPull, maxPull, arrowMinVelocity, arrowMaxVelocity);

                drawTension = Util.RemapNumberClamped(nockDistanceTravelled * (float)(maxPull / 0.5), 0, maxPull, 0f, 1f);

                this.bowDrawLinearMapping.value = drawTension; // Send drawTension value to LinearMapping script, which drives the bow draw animation

                if (nockDistanceTravelled > minPull)
                {
                    pulled = true;
                }
                else
                {
                    pulled = false;
                }

                if ((nockDistanceTravelled > (lastTickDistance + hapticDistanceThreshold)) || nockDistanceTravelled < (lastTickDistance - hapticDistanceThreshold))
                {
                    drawSound.PlayBowTensionClicks(drawTension);

                    lastTickDistance = nockDistanceTravelled;
                }

                if (nockDistanceTravelled >= maxPull)
                {
                    if (Time.time > nextStrainTick)
                    {
                        drawSound.PlayBowTensionClicks(drawTension);

                        nextStrainTick = Time.time + Random.Range(minStrainTickTime, maxStrainTickTime);
                    }
                }
            }
            else
            {
                nockTransform.localPosition = new Vector3(0f, 0f, 0f);

                this.bowDrawLinearMapping.value = 0f;
            }
        }
        else
        {
            if (lerpBackToZeroRotation)
            {
                float lerp = Util.RemapNumber(Time.time, lerpStartTime, lerpStartTime + lerpDuration, 0, 1);

                pivotTransform.localRotation = Quaternion.Lerp(lerpStartRotation, Quaternion.identity, lerp);

                if (lerp >= 1)
                {
                    lerpBackToZeroRotation = false;
                }
            }
        }
    }

    public void ArrowReleased()
    {
        if (nocked == false)
            return;

        nocked = false;

        if (releaseSound != null)
        {
            releaseSound.Play();
        }

        this.StartCoroutine(this.ResetDrawAnim());
    }

    private IEnumerator ResetDrawAnim()
    {
        float startTime = Time.time;
        float startLerp = drawTension;

        while (Time.time < (startTime + 0.02f))
        {
            float lerp = Util.RemapNumberClamped(Time.time, startTime, startTime + 0.02f, startLerp, 0f);
            this.bowDrawLinearMapping.value = lerp;
            yield return null;
        }

        this.bowDrawLinearMapping.value = 0;

        yield break;
    }

    public float GetArrowVelocity()
    {
        return arrowVelocity;
    }

    public void StartRotationLerp()
    {
        lerpStartTime = Time.time;
        lerpBackToZeroRotation = true;
        lerpStartRotation = pivotTransform.localRotation;

        Util.ResetTransform(nockTransform);
    }

    public void StartNock()
    {
        nocked = true;
        nockLerpStartTime = Time.time;
        nockLerpStartRotation = pivotTransform.rotation;

        // Sound of arrow sliding on nock as it's being pulled back
        arrowSlideSound.Play();
    }

    public void ArrowInPosition()
    {
        if (nockSound != null)
        {
            nockSound.Play();
        }
    }

    public void ReleaseNock()
    {
        // ArrowHand tells us to do this when we release the buttons when bow is nocked but not drawn far enough
        nocked = false;
        this.StartCoroutine(this.ResetDrawAnim());
    }
}
