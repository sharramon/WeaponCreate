using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour
{
    [SerializeField] private Transform followTransform;
    void Update()
    {
        this.transform.position = followTransform.position;
        this.transform.rotation = followTransform.rotation;
    }
}
