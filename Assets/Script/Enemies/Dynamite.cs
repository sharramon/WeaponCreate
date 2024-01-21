using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullMetal
{
    public class Dynamite : MonoBehaviour
    {
        [SerializeField] private AudioClip m_hiss;
        [SerializeField] private GameObject m_hissEffect;
        [SerializeField] private GameObject m_fuseFireEffect;
        [SerializeField] private Transform m_dynamiteEffectPos;
        [SerializeField] private Transform m_dynamiteFirePos;
        [SerializeField] private AudioClip m_explode;
        [SerializeField] private GameObject m_explosionEffect;
        [SerializeField] private AudioSource audioSource;

        private GameObject m_instantiatedFuseFire = null;
        private GameObject m_instantiatedDynamiteEffect = null;

        public void PlayHiss()
        {
            if (audioSource != null)
            {
                audioSource.clip = m_hiss;
                audioSource.loop = true;
                audioSource.Play();
            }

            //instantiate hiss effects
            if(m_instantiatedDynamiteEffect != null)
                Destroy(m_instantiatedDynamiteEffect);
            if(m_instantiatedFuseFire != null)
                Destroy(m_instantiatedFuseFire);

            m_instantiatedFuseFire = Instantiate(m_fuseFireEffect, m_dynamiteFirePos.position, Quaternion.identity);
            m_instantiatedFuseFire.transform.parent = m_dynamiteFirePos;
            m_instantiatedDynamiteEffect = Instantiate(m_hissEffect, m_dynamiteEffectPos.position, Quaternion.identity);
            m_instantiatedDynamiteEffect.transform.parent = m_dynamiteEffectPos;

        }

        public void Explode()
        {
            if (m_instantiatedDynamiteEffect != null)
                Destroy(m_instantiatedDynamiteEffect);
            if (m_instantiatedFuseFire != null)
                Destroy(m_instantiatedFuseFire);

            Instantiate(m_explosionEffect, m_dynamiteEffectPos.position, Quaternion.identity);
            PlayExplosion();

            this.gameObject.GetComponent<Renderer>().forceRenderingOff = true;
        }

        public void PlayExplosion()
        {
            if (audioSource == null)
                return;

            audioSource.clip = m_explode;
            audioSource.loop = false;
            audioSource.Play();
        }

        public void StopHiss()
        {
            if (audioSource != null)
                audioSource.Stop();

            if (m_instantiatedDynamiteEffect != null)
                Destroy(m_instantiatedDynamiteEffect);
            if (m_instantiatedFuseFire != null)
                Destroy(m_instantiatedFuseFire);
        }

    }
}
