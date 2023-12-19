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
        [SerializeField] private HandPoseList m_backHandPoseList;
        [SerializeField] private GameObject m_createEffect;
        [SerializeField] private float m_createHandDistance = .20f;

        [Header("Weapons to create")]
        [SerializeField] private WeaponList m_weaponList;

        private GameObject m_mainHand;
        private GameObject m_offHand;
        private Coroutine m_countdownFailureCoroutine;
        private bool m_isCurrentlyCreating = false;
        private GameObject m_instantiatedCreateEffect;
        private GameObject m_instantedLeftWeapon;
        private GameObject m_instantedRightWeapon;

        //for testing
        public float _angle = 0f;

        private void Start()
        {
            SubscribeEvents();
        }
        private void Update()
        {
            if(m_isCurrentlyCreating)
            {
                if(CheckHandDistance())
                {
                    CheckHandAngles();
                }

                //CheckHandAnglesTest();
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
            if (m_isCurrentlyCreating == true)
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

            m_isCurrentlyCreating = true;
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

            m_isCurrentlyCreating = false;

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

            float distance = Utils.FindDistanceOnLocalXZPlane(m_mainHand.transform, m_offHand.transform);

            //Debug.Log($"Distance between hands is {distance}");

            if (distance >= m_createHandDistance)
            {
                return true;
            }
            return false;
        }


        private void CheckHandAnglesTest()
        {
            if (m_mainHand == null || m_offHand == null)
                return;

            Vector3 mainToOffHandVector = m_offHand.transform.position - m_mainHand.transform.position;
            mainToOffHandVector = mainToOffHandVector.normalized;
            Vector3 mainHandForwardVector = m_mainHand.transform.forward.normalized;

            float angle = Vector3.Angle(mainHandForwardVector, mainToOffHandVector);

            _angle = angle;
        }

        private void CheckHandAngles()
        {
            Vector3 mainToOffHandVector = m_offHand.transform.position - m_mainHand.transform.position;
            mainToOffHandVector = mainToOffHandVector.normalized;
            Vector3 mainHandForwardVector = m_mainHand.transform.forward.normalized;

            float angle = Vector3.Angle(mainHandForwardVector, mainToOffHandVector);

            _angle = angle;
            //Debug.Log();
            if (angle < 45f)
            {
                Debug.Log("Angle is below 45");
                CheckFrontHandPoseLists(m_frontHandPoseList.GetHandPoses());
            }

            if (angle > 135f)
            {
                Debug.Log("Angle is above 135");
                CheckFrontHandPoseLists(m_backHandPoseList.GetHandPoses());
            }
        }

        private void CheckFrontHandPoseLists(List<HandPoses> handPoses)
        {
            foreach(HandPoses poses in handPoses)
            {
                //Debug.Log($"checking pose {poses._poseName}");
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
            Debug.Log($"Creating weapon {weaponName} at {m_mainHand.gameObject.name}");
            GameObject weapon;

            if (m_mainHand.gameObject.name.Contains("(L)"))
            {
                if (m_instantedLeftWeapon != null)
                    Destroy(m_instantedLeftWeapon);

                m_instantedLeftWeapon = Instantiate(m_weaponList.GetWeapon(weaponName));
                weapon = m_instantedLeftWeapon;
            }
            else
            {
                if (m_instantedRightWeapon != null)
                    Destroy(m_instantedRightWeapon);

                m_instantedRightWeapon = Instantiate(m_weaponList.GetWeapon(weaponName));
                weapon = m_instantedRightWeapon;
            }

            //end the creation effect and reset the boolean
            m_isCurrentlyCreating = false;
            Destroy(m_instantiatedCreateEffect);

            if (m_countdownFailureCoroutine != null)
                StopCoroutine(m_countdownFailureCoroutine);

            if(weapon == null)
            {
                Debug.LogError($"The weapon {weaponName} doesn't exist!");
                return;
            }

            weapon.transform.position = m_mainHand.transform.position;
            weapon.transform.SetParent(m_mainHand.transform);
            weapon.transform.localEulerAngles = Vector3.zero;
            //this is to account for the negative scale of the right hand. It's a workaround
            weapon.transform.localScale = new Vector3(1, 1, 1);
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
