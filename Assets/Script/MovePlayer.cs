using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    [SerializeField] private Transform m_player;
    [SerializeField] private float m_speed = 1f;
    [SerializeField] private Transform m_rightPointingHand;
    [SerializeField] private Transform m_leftPointingHand;
    private bool m_isRightMoving = false;
    private bool m_isLeftMoving = false;

    private void Update()
    {
        MoveBasedOnActiveHands();
    }

    private void MoveBasedOnActiveHands()
    {
        Vector3 movementDirection = Vector3.zero;

        if (m_isRightMoving && m_isLeftMoving)
        {
            // Average of both hands' forward directions
            Vector3 rightHandForward = new Vector3(m_rightPointingHand.forward.x, 0, m_rightPointingHand.forward.z).normalized;
            Vector3 leftHandForward = new Vector3(m_leftPointingHand.forward.x, 0, m_leftPointingHand.forward.z).normalized;
            movementDirection = (rightHandForward + leftHandForward);
        }
        else if (m_isRightMoving)
        {
            // Only right hand's forward direction
            movementDirection = new Vector3(m_rightPointingHand.forward.x, 0, m_rightPointingHand.forward.z).normalized;
        }
        else if (m_isLeftMoving)
        {
            // Only left hand's forward direction
            movementDirection = new Vector3(m_leftPointingHand.forward.x, 0, m_leftPointingHand.forward.z).normalized;
        }

        // Apply the movement
        m_player.position += movementDirection * m_speed * Time.deltaTime;
    }

    public void SetIsLeftMoving(bool isMoving)
    {
        m_isLeftMoving = isMoving;
    }
    public void SetIsRightMoving(bool isMoving)
    {
        m_isRightMoving = isMoving;
    }
}