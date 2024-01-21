using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullMetal
{
    public class ReinstantiateEnemies : MonoBehaviour
    {
        [SerializeField] private GameObject m_enemies;
        [SerializeField] private GameObject m_enemiesPrefab;

        public void Reinstantiate()
        {
            ResetEnemies();
        }
        private void ResetEnemies()
        {
            Destroy(m_enemies);
            m_enemies = Instantiate(m_enemiesPrefab);
        }
    }
}
