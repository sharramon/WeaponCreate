using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand.Demo;

public class FingerCurl : MonoBehaviour
{
    [SerializeField] private OVRAutoHandTracker m_leftHandTracker;
    [SerializeField] private OVRAutoHandTracker m_rightHandTracker;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Gets the finger curl of any finger. Uses OVRAutoHandTracker to get the values
    /// </summary>
    public float GetFingerCurl(bool isLeft, OVRFingerEnum finger)
    {
        if(isLeft)
        {
            if(m_leftHandTracker == null)
            {
                Debug.Log("There is not left hand");
                return 0;
            }

            return m_leftHandTracker.GetFingerCurl(finger);
        }
        else
        {
            if(m_rightHandTracker == null)
            {
                Debug.Log("There is no right hand");
                return 0;
            }

            return m_rightHandTracker.GetFingerCurl(finger);
        }
    }
}
