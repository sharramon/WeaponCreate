using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullMetal
{
    public class Bow : Weapon
    {
        [SerializeField] private Longbow m_longBow;
        [SerializeField] private ArrowHand m_arrowHand;

        private void Start()
        {
            m_arrowHand = m_handInfo.GetArrowHand();
            m_arrowHand.SetBow(this.m_longBow);
            m_longBow.SetArrowhand(m_arrowHand);
        }

        private void OnDestroy()
        {
            if(m_arrowHand.GetBow() == m_longBow) //just in case it was overwritten by another bow
                m_arrowHand.SetBow(null);
        }
    }
}
