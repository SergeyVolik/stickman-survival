using Pathfinding;
using Prototype;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class ZombieBehaviour : MonoBehaviour
    {
        private CharacterZombieAnimator m_Animator;
        private IAstarAI m_AiMovement;
        private Transform m_TargetTransform;
        public CollisionDamageBehaviour attackCollider;
        private bool m_Attaking;

        public float minSpeed;
        public float maxSpeed;

        public float checkTargetDistance;
        public bool isTargetDetected = false;
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

            m_Animator.SetMove(false);
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
            m_Attaking = false;
            m_AiMovement.canMove = true;
            m_AiMovement.destination = m_TargetTransform.position;
        }

        private void M_Animator_onAttackStarted()
        {
            m_Attaking = true;
            m_AiMovement.canMove = false;
        }

        private void Update()
        {
            if (m_Attaking)
            {
                AttackState();
            }
            else if (isTargetDetected)
            {
                ÑhasePlayerState();
            }
            else
            {
                FindTargetState();
            }
        }

        private void ÑhasePlayerState()
        {            
            if (m_AiMovement.reachedDestination)
            {
              
                m_Animator.Attack();
                M_Animator_onAttackStarted();
                return;
            }

            m_Animator.SetMove(true);
            m_AiMovement.destination = m_TargetTransform.position;           
        }

        private void AttackState()
        {
            m_Animator.SetMove(false);
        }

        private void FindTargetState()
        {
            int targetsFinded = PhysicsHelper.GetAllTargetWithoutWalls(m_Transfrom, m_RaycastHits, checkTargetDistance, targetMask, wallLayerMask, 0.5f);
            isTargetDetected = targetsFinded != 0;

            if (isTargetDetected)
            {
                m_TargetTransform = m_RaycastHits[0].transform;
                m_AiMovement.destination = m_TargetTransform.position;
            }
        }
    }
}