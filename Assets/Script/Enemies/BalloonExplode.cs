using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullMetal
{
    public class BalloonExplode : MonoBehaviour
    {
        [SerializeField] private GameObject m_balloonObject;
        [SerializeField] private LayerMask m_floorLayer;
        [SerializeField] private GameObject m_explodParticle;

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log($"Collision occured with layer {collision.gameObject.layer}");
            if ((m_floorLayer.value & (1 << collision.gameObject.layer)) != 0)
            {
                Explode();
            }
        }

        private void Explode()
        {
            Instantiate(m_explodParticle, transform.position, Quaternion.identity);
            EnemyManager.Instance.BalloonEnemyDestroyed();
            Destroy(m_balloonObject);
        }
    }
}
