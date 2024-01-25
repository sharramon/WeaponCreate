using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullMetal
{
    public class HotAirEnemy : Enemy
    {
        [SerializeField] private GameObject m_smokePrefab;
        [SerializeField] private float m_instantiateHeight;

        [Header("balloon destroy logic")]
        [SerializeField] private Rigidbody m_balloonRigidbody;
        [SerializeField] private GameObject m_balloonObject;
        [SerializeField] private Animator m_balloonAnimator;
        public float _torqueMultiplier = 1f; // Adjust this value as needed

        [Header("Shooting logic")]
        [SerializeField] private GameObject m_cannonAxle;
        [SerializeField] private Transform m_cannonBarrel;
        [SerializeField] private GameObject m_cannonBall;
        [SerializeField] private float m_cannonShootInterval = 2f;
        [SerializeField] private float m_cannonShootSpeed = 10f;
        [SerializeField] private AudioSource m_shootAudio;

        [Header("Cannonball")]
        [SerializeField] private GameObject m_explosion;

        private Coroutine m_shootCannonCoroutine;

        private int m_cannonPoolSize = 5;
        private HashSet<GameObject> m_cannonPool = new HashSet<GameObject>();
        private HashSet<GameObject> m_cannonActive = new HashSet<GameObject>();

        private bool m_isAppeared = false;

        private void Awake()
        {
            CreateCannonPool();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            EnemyAction();
        }

        protected override void Appearing()
        {
            if (m_isAppeared == false)
            {
                m_isAppeared = true;

                this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + m_instantiateHeight, this.gameObject.transform.position.z);
                GameObject smokePrefab = Instantiate(m_smokePrefab);
                smokePrefab.transform.position = this.gameObject.transform.position;

                StartCoroutine(AppearCoroutine());
            }
            AxleFacePlayer();
        }

        protected override void Attacking()
        {
            if (m_shootCannonCoroutine == null)
                m_shootCannonCoroutine = StartCoroutine(ShootCannonRoutine());

            AxleFacePlayer();
        }

        protected override void Dead()
        {
            
        }

        private IEnumerator AppearCoroutine()
        {
            yield return new WaitForSeconds(2f);
            SetEnemyState(EnemyState.Attacking);
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
            cannonball.SetBallonEnemy(this);
            cannonball.SetExplodePrefab(m_explosion);
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
             m_shootAudio.Play();
        }

        public void PoppedBalloon()
        {
            if (m_shootCannonCoroutine != null)
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
            if (m_targetTransform == null)
            {
                Debug.LogError("Player head not found");
                return;
            }

            Utils.LookAtOnXZPlane(m_cannonAxle.transform, m_targetTransform.transform);
        }

        private IEnumerator ShootCannonRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(m_cannonShootInterval);
                ShootCannon(m_targetTransform.gameObject);
            }
        }
    }

}