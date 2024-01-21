using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullMetal
{
    public class GolemEnemy : Enemy
    {
        [SerializeField] private float m_instantiateStartHeight;
        [SerializeField] private float m_instantiateEndHeight;
        [SerializeField] private float m_appearTimer = 2f;
        [SerializeField] private GameObject m_smokePrefab;
        [SerializeField] private GameObject m_head;
        [SerializeField] private Transform m_shootTransform;
        [SerializeField] private float m_headTurnSpeed = 10f;
        [SerializeField] private float m_shootTerm = 2f;
        [SerializeField] private GameObject m_energyBallPrefab;
        [SerializeField] private Sliceable m_sliceable;
        [SerializeField] private float m_deathCountdown = 5f;

        private bool m_isAppeared = false;
        private GameObject m_instantiatedSmokePrefab;
        private float m_appearCurrentTimer = 0f;
        private float m_shootCurrentTimer = 0f;
        private Vector3 m_startPos;
        private Vector3 m_endPos;

        private void Start()
        {
            m_startPos = new Vector3(transform.position.x, transform.position.y + m_instantiateStartHeight, transform.position.z);
            m_endPos = new Vector3(transform.position.x, transform.position.y + m_instantiateEndHeight, transform.position.z);
        }

        private void Update()
        {
            EnemyAction();
        }

        protected override void Appearing()
        {
            if(m_isAppeared == false)
            {
                m_isAppeared = true;
                m_instantiatedSmokePrefab = Instantiate(m_smokePrefab, _appearTransform, Quaternion.identity);
            }

            m_appearCurrentTimer += Time.deltaTime;
            float factor = m_appearCurrentTimer / m_appearTimer;
            transform.position = Vector3.Lerp(m_startPos, m_endPos, factor);

            if (m_appearCurrentTimer >= m_appearTimer)
            {
                m_appearCurrentTimer = 0f;
                if (m_instantiatedSmokePrefab != null)
                    Destroy(m_instantiatedSmokePrefab);

                SetEnemyState(EnemyState.Attacking);
            }
        }

        protected override void Attacking()
        {
            Utils.RotateTowardsOnXZPlane(m_head.transform, m_targetTransform, m_headTurnSpeed);

            if(m_shootCurrentTimer >= m_shootTerm)
            {
                Shoot();
                m_shootCurrentTimer = 0f;
            }
            else
            {
                m_shootCurrentTimer += Time.deltaTime;
            }
        }

        private void Shoot()
        {
            GameObject ball = Instantiate(m_energyBallPrefab, m_shootTransform.position, Quaternion.identity);
            EnergyBall energyball = ball.GetComponent<EnergyBall>();
            energyball._targetTransform = m_targetTransform;
            energyball._isShot = true;
        }

        protected override void Dead()
        {
            StartCoroutine(DeathCountdown());
        }

        private IEnumerator DeathCountdown()
        {
            yield return new WaitForSeconds(m_deathCountdown);
            DestroyAllObjects();
        }

        private void DestroyAllObjects()
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                Transform child = this.transform.GetChild(i);
                // Do something with child
                Destroy(child.gameObject);
            }

            Destroy(this.gameObject);
        }
    }
}
