using Prototype;
using UnityEngine;
using Zenject;

public class ZombieBehaviour : MonoBehaviour
{
    private IPlayerFactory m_PlayerFactory;
    private MoveToTargetBehaviour m_MoveToTarget;
    private CharacterZombieAnimator m_Animator;
    private CustomCharacterController m_CharacterController;
    private Transform m_PlayerTransform;
    public CollisionDamageBehaviour attackCollider;
    private bool m_Attaking;

    public float minSpeed;
    public float maxSpeed;


    [Inject]
    void Construct(IPlayerFactory factory)
    {
        m_PlayerFactory = factory;
        m_PlayerFactory.onPlayerSpawned += M_factory_onPlayerSpawned;

        if (m_PlayerFactory.CurrentPlayerUnit)
        {
            m_PlayerTransform = m_PlayerFactory.CurrentPlayerUnit.transform;
        }
    }

    private void Awake()
    {
        m_MoveToTarget = GetComponent<MoveToTargetBehaviour>();
        m_Animator = GetComponentInChildren<CharacterZombieAnimator>();
        m_CharacterController = GetComponent<CustomCharacterController>();
       
        m_Animator.onAttackEnded += M_Animator_onAttackEnded;
        m_Animator.onDisableCollider += M_Animator_onDisableCollider;
        m_Animator.onEnableCollider += M_Animator_onEnableCollider;

        
        m_CharacterController.speed = Random.Range(minSpeed, maxSpeed);

        m_Animator.SetMoveSpeed(m_CharacterController.speed);
        attackCollider.Deactivate();
        m_MoveToTarget.enabled = true;
        m_MoveToTarget.SetTarget(m_PlayerTransform);
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
        m_CharacterController.UnblockMovement();
        m_MoveToTarget.enabled = true;
        m_MoveToTarget.SetTarget(m_PlayerTransform);
    }

    private void M_Animator_onAttackStarted()
    {
        m_Attaking = true;
        m_CharacterController.BlockMovement();
        m_MoveToTarget.enabled = false;
    }

    private void M_factory_onPlayerSpawned(GameObject obj)
    {
        m_PlayerTransform = obj.transform;
        m_MoveToTarget.target = obj.transform;
    }

    private void Update()
    {
        if (m_PlayerTransform == null)
            return;

        if (!m_Attaking)
        {
            if (m_MoveToTarget.targetReached)
            {
                m_Animator.Attack();
                M_Animator_onAttackStarted();
                return;
            }

            m_MoveToTarget.enabled = true;
            m_MoveToTarget.SetTarget(m_PlayerTransform);  
        }
    }
}
