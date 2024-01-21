using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullMetal
{
    public class Cannonball : MonoBehaviour
    {
        [SerializeField] private HotAirEnemy m_balloonEnemy;
        private float m_cannonballSpeed = 10f;
        private float m_cannonballLifeTime = 10f;
        private float m_cannonballLifeTimeTimer = 0f;

        //private void OnCollisionEnter(Collision collision)
        //{
        //    if(collision.gameObject.tag == "Floor")
        //    {
        //        HitFloor();
        //    }

        //    if (collision.gameObject.tag == "Player")
        //    {
        //        HitPlayer();
        //    }
        //}

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Floor")
            {
                HitFloor();
            }

            if (other.gameObject.tag == "Player")
            {
                HitPlayer();
            }
        }
        public void SetBallonEnemy(HotAirEnemy balloonEnemy)
        {
            m_balloonEnemy = balloonEnemy;
        }

        private void SetCannonBallSpeed(float speed)
        {
            m_cannonballSpeed = speed;
        }

        public void ShootCannon(float speed)
        {
            SetCannonBallSpeed(speed);
            gameObject.SetActive(true);
            m_cannonballLifeTimeTimer = 0f;
            StartCoroutine(ShootCannonBall());
        }

        private IEnumerator ShootCannonBall()
        {
            while (gameObject.activeSelf && m_cannonballLifeTimeTimer < m_cannonballLifeTime)
            {
                m_cannonballLifeTimeTimer += Time.deltaTime;
                transform.Translate(Vector3.forward * m_cannonballSpeed * Time.deltaTime);
                yield return null;
            }

            ReturnToPool();
        }

        private void HitPlayer()
        {
            Debug.Log("Player hit");
            ReturnToPool();
        }

        private void HitFloor()
        {
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            this.gameObject.SetActive(false);
            StopAllCoroutines();
            m_balloonEnemy.ReturnCannonBall(this.gameObject);
        }
    }
}
