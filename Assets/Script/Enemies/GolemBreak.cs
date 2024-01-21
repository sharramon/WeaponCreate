using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullMetal
{
    public class GolemBreak : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_headRigidBody;
        [SerializeField] private GolemEnemy m_golemEnemy;

        private void OnDestroy()
        {
            TurnOnRigidbody();
            RockSmashedDead();
        }
        private void TurnOnRigidbody()
        {
            m_headRigidBody.isKinematic = false;
            m_headRigidBody.useGravity = true;
        }
        public void RockSmashedDead()
        {
            m_golemEnemy.SetEnemyState(EnemyState.Dead);
            EnemyManager.Instance.GolemEnemyDestroyed();
        }
    }
}
