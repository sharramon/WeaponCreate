using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand.Demo;

namespace FullMetal
{
    [CreateAssetMenu(fileName = "Pose list", menuName = "ScriptableObjects/FullMetal/Pose List")]

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

        public string CheckHandPoses(OVRAutoHandTracker mainHandTracker, OVRAutoHandTracker offHandTracker)
        {
            bool mainHandPose = CheckIfPose(mainHandTracker, _mainHand);
            bool offHandPose = CheckIfPose(offHandTracker, _offHand);

            if(mainHandPose == true && offHandPose == true)
            {
                return _poseName;
            }

            return null;
        }

        public bool CheckIfPose(OVRAutoHandTracker handTracker, HandData handData)
        {
            for(int i = 0; i < handData.fingerBendPast.Length; i++)
            {
                if(handTracker.GetFingerCurl(handData.fingerBendPast[i]._finger) < handData.fingerBendPast[i]._bendValue)
                {
                    return false;
                }
            }

            for(int i = 0; i < handData.fingerBendBelow.Length; i++)
            {
                if(handTracker.GetFingerCurl(handData.fingerBendBelow[i]._finger) > handData.fingerBendBelow[i]._bendValue)
                {
                    return false;
                }
            }

            return true;
        }
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
