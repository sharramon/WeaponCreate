using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand.Demo;

namespace FullMetal
{
    public class WeaponCreate : MonoBehaviour
    {
        [Header("Hands")]
        [SerializeField] private OVRAutoHandTracker m_leftHandTracker;
        [SerializeField] private OVRAutoHandTracker m_rightHandTracker;

        [Header("Create Hands")]
        [SerializeField] private HandPoseList m_frontHandPoseList;
        [SerializeField] private GameObject m_createEffect;
        [SerializeField] private float m_createHandDIstance = .20f;

        [Header("Weapons to create")]
        [SerializeField] private WeaponList m_weaponList;

        private GameObject m_mainHand;
        private GameObject m_offHand;
        private Coroutine m_countdownFailureCoroutine;
        private bool m_currentlyCreating = false;
        private GameObject m_instantiatedCreateEffect;

        private void Start()
        {
            SubscribeEvents();
        }
        private void Update()
        {
            if(m_currentlyCreating)
            {
                if(CheckHandDistance())
                {

                }
            }
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
                    Debug.Log($"Create object triggered with tag Back");
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
                    if (m_instantiatedCreateEffect != null)
                        Destroy(m_instantiatedCreateEffect);

                }
            }

            m_currentlyCreating = true;
            m_countdownFailureCoroutine = StartCoroutine(CountdownToFail());

            WeaponCreateEffect(m_isLeft, gameObject);

            if (m_isLeft)
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
            m_instantiatedCreateEffect = Instantiate(m_createEffect);

            if(m_isLeft)
            {
                m_instantiatedCreateEffect.transform.position = handObject.transform.position + handObject.transform.right * 0.06f;
            }
            else
            {
                m_instantiatedCreateEffect.transform.position = handObject.transform.position - handObject.transform.right * 0.06f;

            }
            m_instantiatedCreateEffect.transform.SetParent(handObject.transform);
            m_instantiatedCreateEffect.transform.localEulerAngles = new Vector3(0, 0, 90);
        } 

        private IEnumerator CountdownToFail()
        {
            yield return new WaitForSeconds(10f);

            if (m_instantiatedCreateEffect != null)
                Destroy(m_instantiatedCreateEffect);

            m_currentlyCreating = false;

            m_offHand = null;
            m_mainHand = null;
        }

        private bool CheckHandDistance()
        {
            if(m_offHand == null || m_mainHand == null)
            {
                Debug.Log($"Either offhand is null ({m_offHand == null}), or mainhand is null ({m_mainHand == null}), which shouldn't be the case");
                return false;
            }

            if(Vector3.Distance(m_offHand.transform.position, m_mainHand.transform.position) >= m_createHandDIstance)
            {
                return true;
            }
            return false;
        }

        private void CheckHandAngles()
        {
            Vector3 mainToOffHandVector = m_offHand.transform.position - m_mainHand.transform.position;
            mainToOffHandVector = mainToOffHandVector.normalized;
            Vector3 mainHandForwardVector = m_mainHand.transform.forward.normalized;

            float angle = Vector3.Angle(mainHandForwardVector, mainToOffHandVector);

            if(angle < 25f)
            {
                CheckFrontHandPoseLists();
            }
        }

        private void CheckFrontHandPoseLists()
        {
            List<HandPoses> handPoses = m_frontHandPoseList.GetHandPoses();

            foreach(HandPoses poses in handPoses)
            {
                string poseName = poses.CheckHandPoses(m_mainHand.GetComponent<OVRAutoHandTracker>(), m_offHand.GetComponent<OVRAutoHandTracker>());

                if(poseName != null)
                {
                    CreateWeaopn(poseName);
                    return;
                }
            }
        }

        private void CreateWeaopn(string weaponName)
        {

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
