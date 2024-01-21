using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullMetal
{
    public class DummySliced : MonoBehaviour
    {
        [SerializeField] private DummyEnemy m_dummyEnemy;

        private void OnDestroy()
        {
            DummySlicedDead();
        }

        public void DummySlicedDead()
        {
            m_dummyEnemy.SetEnemyState(EnemyState.Dead);
        }
    }
}
