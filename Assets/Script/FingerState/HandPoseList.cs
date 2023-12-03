using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand.Demo;

namespace FullMetal
{
    [CreateAssetMenu(fileName = "Pose list", menuName = "ScriptableObjects/FullMetal")]

    public class HandPoseList : ScriptableObject
    {
        [SerializeField]
        private List<HandPoses> _handPoses;

        public List<HandPoses> GetHandPoses()
        {
            return _handPoses;
        }
    }
    public enum HandChoice
    {
        BothHands,
        MainOnly,
        OffOnly
    }
    [Serializable]
    public struct HandPoses
    {
        public string _poseName;
        public HandChoice _handChoice;
        public HandData _mainHand;
        public HandData _offHand;
    }
    [Serializable]
    public struct HandData
    {
        public FingerBend[] fingerBendPast;
        public FingerBend[] fingerBendBelow;
    }
    [Serializable]
    public struct FingerBend
    {
        public float _bendValue;
        public OVRFingerEnum _finger;
    }
}
