using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace FullMetal
{
    public class DummyEnemy : Enemy
    {
        [SerializeField] private float m_arriveBuffer = 0.3f;
        [SerializeField] private float m_instantiateHeight;
        [SerializeField] private GameObject m_smokePrefab;
        [SerializeField] private Transform m_smokePos;
        [SerializeField] private GameObject m_dynamiteEffect;
        [SerializeField] private Sliceable m_sliceable;
        [SerializeField] private GameObject m_spear;
        [SerializeField] private Dynamite m_dynamite;
        [SerializeField] private GameObject m_wheel;
        [SerializeField] private Animator m_wheelAnimator;
        [SerializeField] private float m_deathCountdown = 5f; //use when sliced
        [SerializeField] private float m_dynamiteTimer = 4f;
        [SerializeField] private float m_disappearCountdown = 1f; //use when dynmatie explodes

        //Dynamite logic
        private bool m_isDynamiteStarted = false;
        private GameObject m_instantiatedDynamiteEffect = null;
        private Coroutine m_dyanimateCoroutine = null;
        private float m_bufferedArriveDistance;
        private float m_actualArriveDistance;

        //Appear
        private bool m_isAppeared = false;
        private void Start()
        {
            m_actualArriveDistance = m_arriveDistance;
            m_bufferedArriveDistance = m_arriveDistance + m_arriveBuffer;
        }
        private void Update()
        {
            EnemyAction();
        }

        private void OnDestroy()
        {
            EnemyManager.Instance.WheelEnemyDestroyed();
            StopAllCoroutines();
        }

        protected override void Idle()
        {
            base.Idle();
        }

        protected override void Appearing()
        {
            if(m_isAppeared == false)
            {
                m_isAppeared = true;

                this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + m_instantiateHeight, this.gameObject.transform.position.z);
                GameObject smokePrefab = Instantiate(m_smokePrefab);
                smokePrefab.transform.position = m_smokePos.position;

                StartCoroutine(AppearCoroutine());
            }

            Utils.RotateTowardsOnXZPlane(this.transform, m_targetTransform, m_turnSpeed);
        }

        private IEnumerator AppearCoroutine()
        {
            yield return new WaitForSeconds(2f);
            SetEnemyState(EnemyState.Attacking);
        }

        protected override void Wandering()
        {
            base.Wandering();
        }

        protected override void Tracking()
        {
            base.Tracking();
        }

        protected override void Attacking()
        {
            //attack logic
            if (m_targetTransform != null)
            {
                m_currentTargetPosition = m_targetTransform.position;
                float distanceToTarget = Vector3.Distance(transform.position, m_currentTargetPosition);

                if (distanceToTarget > m_actualArriveDistance)
                {
                    m_actualArriveDistance = m_arriveDistance;

                    SetMoveState(MoveState.Walking);
                    m_moveSpeed = m_walkSpeed;
                    MoveTowardsTarget();

                    StopDynamite();
                }
                else
                {
                    m_actualArriveDistance = m_bufferedArriveDistance;

                    SetMoveState(MoveState.Idle);
                    m_moveSpeed = 0f;

                    StartDynamite();
                }

                Utils.RotateTowardsOnXZPlane(this.transform, m_targetTransform, m_turnSpeed);
            }else
            {
                SetEnemyState(EnemyState.Wandering);
            }
        }

        private void StartDynamite()
        {
            if (m_isDynamiteStarted == true)
                return;

            m_isDynamiteStarted = true;
            m_dynamite.PlayHiss();

            if(m_dyanimateCoroutine != null)
                m_dyanimateCoroutine = null;

            m_dyanimateCoroutine = StartCoroutine(DynamiteCoroutine());
        }

        private void StopDynamite()
        {
            if (m_isDynamiteStarted == false)
                return;

            m_isDynamiteStarted = false;
            m_dynamite.StopHiss();

            if(m_dyanimateCoroutine != null)
            {
                StopCoroutine(m_dyanimateCoroutine);
                m_dyanimateCoroutine = null;
            }
        }

        private IEnumerator DynamiteCoroutine()
        {
            Debug.Log("Dynamite coroutine started");
            yield return new WaitForSeconds(m_dynamiteTimer);

            m_sliceable._isSliceable = false;
            m_dynamite.Explode();

            SetEnemyState(EnemyState.Empty);
            StartCoroutine(DisappearCoroutine());
        }

        private IEnumerator DisappearCoroutine()
        {
            yield return new WaitForSeconds(m_disappearCountdown);

            DestroyAllObjects();
        }

        protected override void Fleeing()
        {
            base.Fleeing();
        }

        protected override void Dead()
        {
            //death logic
            if(m_dyanimateCoroutine != null)
                StopCoroutine(m_dyanimateCoroutine);

            Destroy(m_spear);
            m_wheel.GetComponent<Rigidbody>().isKinematic = false;
            m_wheel.GetComponent<Rigidbody>().useGravity = true;
            //m_wheel.GetComponent<Animator>().enabled = false;
            StartCoroutine(DeathCountdown());
        }

        private IEnumerator DeathCountdown()
        {
            yield return new WaitForSeconds(m_deathCountdown);
            DestroyAllObjects();
        }

        private void DestroyAllObjects()
        {
            //for (int i = 0; i < this.transform.childCount; i++)
            //{
            //    Transform child = this.transform.GetChild(i);
            //    // Do something with child
            //    if(m_enemyState == EnemyState.Empty)
            //        CreateSmoke(child);
            //}

            for (int i = 0; i < this.transform.childCount; i++)
            {
                Transform child = this.transform.GetChild(i);
                // Do something with child
                Destroy(child.gameObject);
            }

            Destroy(this.gameObject);
        }

        private void CreateSmoke(Transform transform)
        {
            Instantiate(m_smokePrefab, transform.position, Quaternion.identity);
        }

        protected override void MoveTowardsTarget()
        {
            //keep current y position
            float currentYPosition = transform.position.y;
            Vector3 targetPositionWithSameY = new Vector3(m_currentTargetPosition.x, currentYPosition, m_currentTargetPosition.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPositionWithSameY, m_walkSpeed * Time.deltaTime);
        }
    }
}
