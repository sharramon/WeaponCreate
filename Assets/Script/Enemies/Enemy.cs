using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;

namespace FullMetal
{
    public enum EnemyState
    {
        Empty,
        Appearing,
        Wandering,
        Tracking,
        Attacking,
        Fleeing,
        Dead
    }
    public enum MoveState
    {
        Idle,
        Walking
    }
    public class Enemy : MonoBehaviour
    {
        public Vector3 _appearTransform; 

        [SerializeField] protected MoveState m_moveState = MoveState.Idle;
        [SerializeField] protected EnemyState m_enemyState = EnemyState.Empty;

        [SerializeField] protected float m_walkSpeed = 1f;

        [SerializeField] protected float m_arriveDistance = 0.5f;
        [SerializeField] protected float m_wanderDistance = 1f;
        [SerializeField] protected float m_wanderMinTime = 2f;
        [SerializeField] protected float m_wanderMaxTime = 4f;
        [SerializeField] protected float m_fleeDistance = 1f;
        [SerializeField] protected float m_moveSpeed = 0f;
        [SerializeField] protected float m_turnSpeed = 0f;

        protected Transform m_targetTransform;
        protected Vector3 m_currentTargetPosition;

        protected float m_wanderTimer = 0f;
        protected float m_timeToWander = 0f;

        private void Start()
        {

        }

        public void SetTargetTransform(Transform targetTransform)
        {
            m_targetTransform = targetTransform;
        }

        protected void EnemyAction()
        {
            switch (m_enemyState)
            {
                case EnemyState.Empty:
                    Idle();
                    break;
                case EnemyState.Appearing:
                    Appearing();
                    break;
                case EnemyState.Wandering:
                    Wandering();
                    break;
                case EnemyState.Tracking:
                    Tracking();
                    break;
                case EnemyState.Attacking:
                    Attacking();
                    break;
                case EnemyState.Fleeing:
                    Fleeing();
                    break;
                case EnemyState.Dead:
                    Dead();
                    break;
                default:
                    break;
            }
        }

        //state logic
        public virtual void SetEnemyState(EnemyState state)
        {
            if (state == EnemyState.Wandering && m_enemyState != EnemyState.Wandering)
            {
                m_wanderTimer = 0f;
            }

            m_enemyState = state;
        }

        protected virtual void Idle()
        {
            SetMoveState(MoveState.Idle);
            m_moveSpeed = 0f;
        }
        protected virtual void Appearing()
        {

        }
        protected virtual void Wandering()
        {
            SetMoveState(MoveState.Walking);
            m_moveSpeed = m_walkSpeed;

            if (m_wanderTimer <= 0f || Vector3.Distance(transform.position, m_targetTransform.position) < 0.1f)
            {
                SetRandomTarget();
                m_timeToWander = Random.Range(m_wanderMinTime, m_wanderMaxTime);
                m_wanderTimer = m_timeToWander;
            }
            else
            {
                m_wanderTimer -= Time.deltaTime;
            }

            MoveTowardsTarget();
        }
        protected virtual void Tracking()
        {
            if (m_targetTransform != null)
            {
                m_currentTargetPosition = m_targetTransform.position;
                float distanceToTarget = Vector3.Distance(transform.position, m_currentTargetPosition);

                if (distanceToTarget > m_arriveDistance)
                {
                    SetMoveState(MoveState.Walking);
                    m_moveSpeed = m_walkSpeed;
                    MoveTowardsTarget();
                }
                else
                {
                    SetMoveState(MoveState.Idle);
                    m_moveSpeed = 0f;
                }
            }
            else
            {
                SetEnemyState(EnemyState.Wandering);
            }
        }

        protected virtual void Attacking()
        {
            
        }
        protected virtual void Fleeing()
        {
            if (m_targetTransform != null)
            {
                m_currentTargetPosition = m_targetTransform.position;
                float distanceToTarget = Vector3.Distance(transform.position, m_currentTargetPosition);

                if (distanceToTarget <= m_fleeDistance)
                {
                    SetMoveState(MoveState.Walking);
                    m_moveSpeed = m_walkSpeed;
                    MoveAwayFromTarget();
                }
                else
                {
                    SetEnemyState(EnemyState.Wandering);
                }
            }
            else
            {
                // If there's no target, switch to wandering
                SetEnemyState(EnemyState.Wandering);
            }
        }
        protected virtual void Dead()
        {

        }

        //Movement logic
        public virtual void SetMoveState(MoveState state)
        {
            m_moveState = state;
        }
        public virtual void SetTarget(Transform targetTransform)
        {
            m_targetTransform = targetTransform;
        }

        //this only works for 2d planes
        protected virtual void SetRandomTarget()
        {
            Vector2 randomPointInCircle = Random.insideUnitCircle;
            randomPointInCircle *= m_wanderDistance;

            Vector3 randomTarget = new Vector3(randomPointInCircle.x, transform.position.y, randomPointInCircle.y);
            m_currentTargetPosition = randomTarget + transform.position;

        }

        protected virtual void MoveTowardsTarget()
        {
            // Movement logic towards the currentTargetPosition
            transform.position = Vector3.MoveTowards(transform.position, m_currentTargetPosition, m_walkSpeed * Time.deltaTime);
        }

        protected virtual void MoveAwayFromTarget()
        {
            // Movement logic away from the currentTargetPosition
            Vector3 fleeDirection = (transform.position - m_currentTargetPosition).normalized;
            transform.position += fleeDirection * m_walkSpeed * Time.deltaTime;
        }

        protected virtual void StopMoving()
        {
            m_moveSpeed = 0f;
        }

        //Attack logic
        protected virtual bool ShouldAttack(Transform target, float senseDistance)
        {
            // Logic to determine if the enemy should attack
            return Vector3.Distance(transform.position, target.position) < senseDistance; // Example condition
        }
    }
}