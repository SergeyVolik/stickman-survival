using Pathfinding;
using Prototype;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public interface ITargetSeeker
    {
        public void SetTarget(Transform target);
    }
    public enum ZombiebehaviourState
    {
        Idle,
        Chase,
        Attacking
    }

    public class ZombieBehaviour : MonoBehaviour, ITargetSeeker
    {
        private CharacterZombieAnimator m_Animator;
        private IAstarAI m_AiMovement;
        private Transform m_TargetTransform;
        public CollisionDamageBehaviour attackCollider;
        private bool m_AttakingAnimation;
        private bool m_IsAttaking;
        public float delayBetweenAttacks;
        public float delayAttatckT;
        public float minSpeed;
        public float maxSpeed;
        private ZombiebehaviourState m_CurrentState;

        public float checkTargetDistance;
        public bool IsTargetDetected => m_TargetTransform != null;
        private Transform m_Transfrom;

        RaycastHit[] m_RaycastHits;
        public LayerMask targetMask;
        public LayerMask wallLayerMask;

        private void Awake()
        {
            m_Animator = GetComponentInChildren<CharacterZombieAnimator>();
            m_AiMovement = GetComponent<IAstarAI>();

            m_Animator.onAttackEnded += M_Animator_onAttackEnded;
            m_Animator.onDisableCollider += M_Animator_onDisableCollider;
            m_Animator.onEnableCollider += M_Animator_onEnableCollider;

            var health = GetComponent<HealthData>();

            health.onDeath += ZombieBehaviour_onDeath;
            health.onHealthChanged += Health_onHealthChanged;
            m_AiMovement.maxSpeed = Random.Range(minSpeed, maxSpeed);

            m_Animator.SetMoveSpeed(m_AiMovement.maxSpeed);
            attackCollider.Deactivate();
            m_RaycastHits = new RaycastHit[5];
            m_Transfrom = transform;

            m_AiMovement.destination = m_Transfrom.position;
        }

        private void Health_onHealthChanged(HealthChangeData obj)
        {
            if (obj.Source)
            {
                m_AiMovement.destination = obj.Source.transform.position;
            }
        }

        private void ZombieBehaviour_onDeath()
        {
            m_AiMovement.canMove = false;
        }

        private void M_Animator_onEnableCollider()
        {
            attackCollider.Activate();
        }

        private void M_Animator_onDisableCollider()
        {
            attackCollider.Deactivate();
        }

        private void M_Animator_onAttackEnded()
        {
            m_AttakingAnimation = false;
            m_AiMovement.canMove = true;
            m_AiMovement.destination = m_TargetTransform.position;
        }

        private void M_Animator_onAttackStarted()
        {
            m_AttakingAnimation = true;
            m_AiMovement.canMove = false;
        }

        public void SetTarget(Transform target)
        {
            m_CurrentState = ZombiebehaviourState.Chase;
            m_TargetTransform = target;
        }

        private void Update()
        {
            m_Animator.SetMove(!m_AiMovement.reachedDestination);

            switch (m_CurrentState)
            {
                case ZombiebehaviourState.Idle:
                    IdleState();
                    break;
                case ZombiebehaviourState.Chase:
                    ChaseState();
                    break;
                case ZombiebehaviourState.Attacking:
                    AttackState();
                    break;
                default:
                    break;
            }
        }

        private void ChaseState()
        {            
            if (m_AiMovement.reachedDestination)
            {
                m_CurrentState = ZombiebehaviourState.Attacking;              
                return;
            }
          
            m_AiMovement.destination = m_TargetTransform.position;           
        }

        private void AttackState()
        {
            if (!m_IsAttaking)
            {
                m_Animator.Attack();
                M_Animator_onAttackStarted();
                m_IsAttaking = true;
            }
            else
            {
                if (!m_AttakingAnimation)
                {
                    delayAttatckT += Time.deltaTime;
                    if (delayBetweenAttacks < delayAttatckT)
                    {
                        delayAttatckT = 0;
                        m_IsAttaking = false;
                        m_CurrentState = ZombiebehaviourState.Chase;
                    }
                }
            }
        }

        private void IdleState()
        {
            int targetsFinded = PhysicsHelper.GetAllTargetWithoutWalls(m_Transfrom, m_RaycastHits, checkTargetDistance, targetMask, wallLayerMask, 0.5f);
            var isTargetDetected = targetsFinded != 0;

            if (isTargetDetected)
            {
                m_TargetTransform = m_RaycastHits[0].transform;
                m_AiMovement.destination = m_TargetTransform.position;
                m_CurrentState = ZombiebehaviourState.Chase;
            }
        }
    }
}