using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullMetal
{
    public class EnergyBall : MonoBehaviour
    {
        [HideInInspector] public Transform _targetTransform;
        [SerializeField] private float m_arriveDistance = 0.1f;
        public float m_moveSpeed = 1f;
        public float m_lifeTime = 10f;
        public bool _isShot = false;

        private float m_currentLifeTime = 0f;

        private void Update()
        {
            if(_isShot == true)
            {
                GoToTarget();
                CheckLifeTime();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            //collison logic here
        }

        private void GoToTarget()
        {
            if (_targetTransform == null)
                return;

            transform.position = Vector3.MoveTowards(this.transform.position, _targetTransform.position, m_moveSpeed * Time.deltaTime);

            if(Vector3.Distance(transform.position, _targetTransform.position) < m_arriveDistance)
            {
                HitPlayer();
            }
        }

        private void CheckLifeTime()
        {
            if(m_currentLifeTime > m_lifeTime)
            {
                Destroy(this.gameObject);
            }
            else
            {
                m_currentLifeTime += Time.deltaTime;
            }
        }

        private void HitPlayer()
        {
            Destroy(this.gameObject);
        }
    }
}