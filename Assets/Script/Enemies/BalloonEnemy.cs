using Oculus.Interaction.Body.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullMetal
{
    public class BalloonEnemy : MonoBehaviour
    {
        [Header("balloon destroy logic")]
        [SerializeField] private Rigidbody m_balloonRigidbody;
        [SerializeField] private GameObject m_balloonObject;
        [SerializeField] private Animator m_balloonAnimator;
        public float _torqueMultiplier = 1f; // Adjust this value as needed

        [Header("Shooting logic")]
        [SerializeField] private GameObject m_cannonAxle;
        [SerializeField] private Transform m_cannonBarrel;
        [SerializeField] private GameObject m_cannonBall;
        [SerializeField] private GameObject m_playerHead;
        [SerializeField] private float m_cannonShootInterval = 2f;
        [SerializeField] private float m_cannonShootSpeed = 10f;

        private Coroutine m_shootCannonCoroutine;

        private int m_cannonPoolSize = 5;
        private HashSet<GameObject> m_cannonPool = new HashSet<GameObject>();
        private HashSet<GameObject> m_cannonActive = new HashSet<GameObject>();

        private void Awake()
        {
            FindPlayerHead();
            CreateCannonPool();
        }
        private void Start()
        {
            //m_shootCannonCoroutine = StartCoroutine(ShootCannonRoutine());
        }
        private void Update()
        {
            AxleFacePlayer();
        }

        private void FindPlayerHead()
        {
            m_playerHead = Camera.main.gameObject;
        }

        //Creates a pool of cannonballs
        private void CreateCannonPool()
        {
            for (int i = 0; i < m_cannonPoolSize; i++)
            {
                CreateCannonball();
            }
        }

        //Finds an inactive cannonball in the pool and returns it
        private GameObject GetCannonBall()
        {
            GameObject cannonBall = null;

            if (m_cannonPool.Count > 0)
            {
                foreach (GameObject cb in m_cannonPool)
                {
                    cannonBall = cb;
                    break;
                }
                m_cannonPool.Remove(cannonBall);
                m_cannonActive.Add(cannonBall);
            }
            else
            {
                cannonBall = CreateCannonball();
                m_cannonActive.Add(cannonBall);
            }

            return cannonBall;
        }

        private GameObject CreateCannonball()
        {
            GameObject cannonBallObject = Instantiate(m_cannonBall, m_cannonBarrel.position, Quaternion.identity);
            cannonBallObject.SetActive(false);
            Cannonball cannonball = cannonBallObject.AddComponent<Cannonball>();
            //cannonball.SetBallonEnemy(this);
            m_cannonPool.Add(cannonBallObject);

            return cannonBallObject;
        }

        //Returns a cannonball to the pool
        public void ReturnCannonBall(GameObject cannonBall)
        {
            m_cannonActive.Remove(cannonBall);
            m_cannonPool.Add(cannonBall);
        }

        //shoots a cannonball at player
        public void ShootCannon(GameObject playerHead)
        {
            GameObject cannonBall = GetCannonBall();
            Debug.Log($"CannonBall is null : {cannonBall == null}"); 
            Debug.Log($"CannonBarrel is null : {m_cannonBarrel == null}");
            cannonBall.transform.position = m_cannonBarrel.position;
            cannonBall.transform.rotation = Quaternion.LookRotation(playerHead.transform.position - m_cannonBarrel.position);
            cannonBall.GetComponent<Cannonball>().ShootCannon(m_cannonShootSpeed);
            //Vector3 direction = playerHead.transform.position - m_cannonBarrel.position;

        }

        public void PoppedBalloon()
        {
            if(m_shootCannonCoroutine != null)
                StopCoroutine(m_shootCannonCoroutine);

            m_balloonAnimator.enabled = false;
            m_balloonObject.SetActive(false);
            m_balloonRigidbody.isKinematic = false;
            m_balloonRigidbody.useGravity = true;

            // Add a random torque
            Vector3 randomTorque = new Vector3(0, Random.Range(-1f, 1f), 0);
            m_balloonRigidbody.AddTorque(randomTorque * _torqueMultiplier, ForceMode.Impulse);
        }

        private void AxleFacePlayer()
        {
            if (m_playerHead == null)
            {
                Debug.LogError("Player head not found");
                return;
            }

            Utils.LookAtOnXZPlane(m_cannonAxle.transform, m_playerHead.transform);
        }

        private IEnumerator ShootCannonRoutine()
        {
            while(true)
            {
                yield return new WaitForSeconds(m_cannonShootInterval);
                ShootCannon(m_playerHead);
            }
        }
    }
}
