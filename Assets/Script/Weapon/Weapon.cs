using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullMetal
{
    public class Weapon : MonoBehaviour
    {
        protected bool m_isLeftHand = false;
        [SerializeField] protected HandInfo m_handInfo = null;

        public void SetHandedness(bool isLeft)
        {
            m_isLeftHand = isLeft;
        }

        public void SetHandInfo(HandInfo handInfo)
        {
            m_handInfo = handInfo;
        }
    }
}
