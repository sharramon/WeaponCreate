using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullMetal
{
    //A class that holds info about various hands for use in the weapon system
    public class HandInfo : MonoBehaviour
    {
        [SerializeField] private ArrowHand m_arrowHand;

        public ArrowHand GetArrowHand()
        {
            return m_arrowHand;
        }
    }
}