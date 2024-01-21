using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FullMetal
{
    public class BalloonPop : MonoBehaviour
    {
        public UnityEvent _balloonHitEvent;

        public void BalloonHit()
        {
            _balloonHitEvent?.Invoke();
        }
    }
}
