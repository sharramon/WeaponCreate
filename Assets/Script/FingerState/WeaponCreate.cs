using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand.Demo;

namespace FullMetal
{
    public class WeaponCreate : MonoBehaviour
    {
        [SerializeField] private OVRAutoHandTracker m_leftHandTracker;
        [SerializeField] private OVRAutoHandTracker m_rightHandTracker;
        [SerializeField] private HandPoseList m_handPoseList;

        private GameObject m_mainHand;
        private GameObject m_offHand;
        private Coroutine m_countdownFailureCoroutine;
        private bool m_currentlyCreating = false;
        private GameObject m_weaponCreateParticle;

        private void Start()
        {
            SubscribeEvents();
        }
        private void OnDestroy()
        {
            UnsubscribeEvents();
            StopAllCoroutines();
        }
        private void SubscribeEvents()
        {
            EventManager.Instance._tagTouchedEvent += CreateObjectTrigger;
        }
        private void UnsubscribeEvents()
        {
            EventManager.Instance._tagTouchedEvent -= CreateObjectTrigger;
        }
        private void CreateObjectTrigger(string tag, bool m_isLeft, GameObject parentObject)
        {
            switch(tag)
            {
                case "Back":
                    CreateWeaponTrigger(m_isLeft, parentObject);
                    break;
                default:
                    break;
            }
        }
        private void CreateWeaponTrigger(bool m_isLeft, GameObject gameObject)
        {
            if (m_currentlyCreating == true)
            {
                if(m_offHand != null && (m_offHand == m_leftHandTracker.gameObject) == m_isLeft)
                {
                    return;
                }
                else
                {
                    if(m_countdownFailureCoroutine != null)
                        StopCoroutine(m_countdownFailureCoroutine);
                    if (m_weaponCreateParticle != null)
                        Destroy(m_weaponCreateParticle);

                }
            }

            m_currentlyCreating = true;
            m_countdownFailureCoroutine = StartCoroutine(CountdownToFail());

            if(m_isLeft)
            {
                m_offHand = m_leftHandTracker.gameObject;
                m_mainHand = m_rightHandTracker.gameObject;
            }
            else
            {
                m_offHand = m_rightHandTracker.gameObject;
                m_mainHand = m_leftHandTracker.gameObject;
            }


        }

        private void WeaponCreateEffect(bool m_isLeft, GameObject handObject)
        {

        } 

        private IEnumerator CountdownToFail()
        {
            yield return new WaitForSeconds(10f);

            if (m_weaponCreateParticle != null)
                Destroy(m_weaponCreateParticle);

            m_currentlyCreating = false;
        }



        /// <summary>
        /// Gets the finger curl of any finger. Uses OVRAutoHandTracker to get the values
        /// </summary>
        public float GetFingerCurl(bool isLeft, OVRFingerEnum finger)
        {
            if (isLeft)
            {
                if (m_leftHandTracker == null)
                {
                    Debug.Log("There is not left hand");
                    return 0;
                }

                return m_leftHandTracker.GetFingerCurl(finger);
            }
            else
            {
                if (m_rightHandTracker == null)
                {
                    Debug.Log("There is no right hand");
                    return 0;
                }

                return m_rightHandTracker.GetFingerCurl(finger);
            }
        }
    }
}
