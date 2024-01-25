using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullMetal
{
    public class EnemyManager : Singleton<EnemyManager>
    {
        [SerializeField] private Transform m_spawnCenter;
        [SerializeField] private LayerMask m_floorLayer;
        [SerializeField] private Transform m_playerTransform;

        [Header("Wheel dummies")]
        [SerializeField] private GameObject m_wheelEnemyPrefab;
        [SerializeField] private float m_wheelSpawnRadius = 5f;
        [SerializeField] private float m_spawnRate = 4f;
        [SerializeField] private int m_wheelMaxSpawn = 5;
        private float m_wheelSpawnTimer = 0f;
        private int m_wheelSpawnCount = 0;

        [Header("Golem")]
        [SerializeField] private GameObject m_golemEnemyPrefab;
        [SerializeField] private float m_golemSpawnRadius = 5f;
        [SerializeField] private float m_golemSpawnRate = 6f;
        [SerializeField] private int m_golemMaxSpawn = 5;
        private float m_golemSpawnTimer = 0f;
        private int m_golemSpawnCount = 0;

        [Header("Balloons")]
        [SerializeField] private GameObject m_balloonEnemyPrefab;
        [SerializeField] private float m_balloonSpawnRadius = 5f;
        [SerializeField] private float m_balloonSpawnRate = 6f;
        [SerializeField] private int m_balloonMaxSpawn = 5;
        private float m_balloonSpawnTimer = 0f;
        private int m_balloonSpawnCount = 0;

        private void Update()
        {
            CheckIfSpawnWheel();
            CheckIfSpawnGolem();
            CheckIfSpawnBalloon();
        }

        #region wheel dummy spawn
        private void CheckIfSpawnWheel()
        {
            if(m_wheelSpawnTimer > m_spawnRate)
            {
                m_wheelSpawnTimer = 0f;

                if(m_wheelSpawnCount < m_wheelMaxSpawn)
                {
                    SpawnWheelEnemy();
                }
            }

            m_wheelSpawnTimer += Time.deltaTime;
        }

        private void SpawnWheelEnemy()
        {
            Vector3? spawnPos = GetWheelSpawnPos(m_spawnCenter);
            if(spawnPos != null)
            {
                GameObject wheelEnemy = Instantiate(m_wheelEnemyPrefab, spawnPos.Value, Quaternion.identity);
                Enemy enemy = wheelEnemy.GetComponent<Enemy>();
                enemy.SetTargetTransform(m_playerTransform);
                enemy.SetEnemyState(EnemyState.Appearing);
                m_wheelSpawnCount++;
            }
        }

        private Vector3? GetWheelSpawnPos(Transform centerTransform)
        {
            Vector2 randomPointInCircle = Random.insideUnitCircle;
            randomPointInCircle *= m_wheelSpawnRadius;

            Vector3 randomTarget = new Vector3(randomPointInCircle.x, 1f, randomPointInCircle.y);
            Vector3 m_currentTargetPosition = randomTarget + centerTransform.position;

            RaycastHit hit;
            if (Physics.Raycast(m_currentTargetPosition, Vector3.down, out hit, 10f, m_floorLayer))
            {
                m_currentTargetPosition = hit.point;
                return m_currentTargetPosition;
            }
            else
            {
                return null;
            }
        }

        public void WheelEnemyDestroyed()
        {
            m_wheelSpawnCount--;
        }
        #endregion

        #region golem spawn
        private void CheckIfSpawnGolem()
        {
            if(m_golemSpawnTimer > m_golemSpawnRate)
            {
                m_golemSpawnTimer = 0;

                if(m_golemSpawnCount < m_golemMaxSpawn)
                {
                    SpawnGolem();
                }
            }

            m_golemSpawnTimer += Time.deltaTime;
        }
        private void SpawnGolem()
        {
            Debug.Log("Started spawn golem");
            Vector3? spawnPos = GetGolemSpawnPos(m_spawnCenter);

            if(spawnPos != null)
            {
                Debug.Log("Got spawn pos");
                GameObject golemEnemy = Instantiate(m_golemEnemyPrefab, spawnPos.Value, Quaternion.identity);
                Utils.LookAtOnXZPlane(golemEnemy.transform, m_playerTransform);
                Enemy enemy = golemEnemy.GetComponent<Enemy>();
                enemy.SetTargetTransform(m_playerTransform);
                enemy._appearTransform = (Vector3)spawnPos;
                enemy.SetEnemyState(EnemyState.Appearing);
                m_golemSpawnCount++;
            }
        }

        private Vector3? GetGolemSpawnPos(Transform centerTransform)
        {
            Vector2 randomPointInCircle = Random.insideUnitCircle;
            randomPointInCircle *= m_golemSpawnRadius;

            Vector3 randomTarget = new Vector3(randomPointInCircle.x, 1f, randomPointInCircle.y);
            Vector3 m_currentTargetPosition = randomTarget + centerTransform.position;

            RaycastHit hit;
            if (Physics.Raycast(m_currentTargetPosition, Vector3.down, out hit, 10f, m_floorLayer))
            {
                m_currentTargetPosition = hit.point;
                return m_currentTargetPosition;
            }
            else
            {
                return null;
            }
        }

        public void GolemEnemyDestroyed()
        {
            m_golemSpawnCount--;
        }
        #endregion

        private void CheckIfSpawnBalloon()
        {
            if (m_balloonSpawnTimer > m_balloonSpawnRate)
            {
                m_balloonSpawnTimer = 0;

                if (m_balloonSpawnCount < m_balloonMaxSpawn)
                {
                    SpawnBalloon();
                }
            }

            m_balloonSpawnTimer += Time.deltaTime;
        }

        private void SpawnBalloon()
        {
            Debug.Log("Started spawn golem");
            Vector3? spawnPos = GetBalloonSpawnPos(m_spawnCenter);

            if (spawnPos != null)
            {
                Debug.Log("Got spawn pos");
                GameObject balloonEnemy = Instantiate(m_balloonEnemyPrefab, spawnPos.Value, Quaternion.identity);
                Enemy enemy = balloonEnemy.GetComponent<Enemy>();
                enemy.SetTargetTransform(m_playerTransform);
                enemy._appearTransform = (Vector3)spawnPos;
                enemy.SetEnemyState(EnemyState.Appearing);
                m_balloonSpawnCount++;
            }
        }

        private Vector3? GetBalloonSpawnPos(Transform centerTransform)
        {
            Vector2 randomPointOnCircle = Random.insideUnitCircle.normalized;
            randomPointOnCircle *= m_balloonSpawnRadius;

            Vector3 randomTarget = new Vector3(randomPointOnCircle.x, 1f, randomPointOnCircle.y);
            Vector3 m_currentTargetPosition = randomTarget + centerTransform.position;

            RaycastHit hit;
            if (Physics.Raycast(m_currentTargetPosition, Vector3.down, out hit, 10f, m_floorLayer))
            {
                m_currentTargetPosition = hit.point;
                return m_currentTargetPosition;
            }
            else
            {
                return null;
            }
        }

        public void BalloonEnemyDestroyed()
        {
            m_balloonSpawnCount--;
        }
    }
}
