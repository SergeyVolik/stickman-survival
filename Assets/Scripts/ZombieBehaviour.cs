using Pathfinding;
using Prototype;
using UnityEngine;
using Zenject;

public class ZombieBehaviour : MonoBehaviour
{
    private IPlayerFactory m_PlayerFactory;
    private CharacterZombieAnimator m_Animator;
    private IAstarAI m_AiMovement;
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
        m_Animator = GetComponentInChildren<CharacterZombieAnimator>();
        m_AiMovement = GetComponent<IAstarAI>();
       
        m_Animator.onAttackEnded += M_Animator_onAttackEnded;
        m_Animator.onDisableCollider += M_Animator_onDisableCollider;
        m_Animator.onEnableCollider += M_Animator_onEnableCollider;


        m_AiMovement.maxSpeed = Random.Range(minSpeed, maxSpeed);

        m_Animator.SetMoveSpeed(m_AiMovement.maxSpeed);
        attackCollider.Deactivate();
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
        m_AiMovement.destination = m_PlayerTransform.position;
    }

    private void M_Animator_onAttackStarted()
    {
        m_Attaking = true;
        m_AiMovement.canMove = false;       
    }

    private void M_factory_onPlayerSpawned(GameObject obj)
    {
        m_PlayerTransform = obj.transform;
        m_AiMovement.destination = m_PlayerTransform.position;
    }

    private void Update()
    {
        if (m_PlayerTransform == null)
            return;

        if (!m_Attaking)
        {
            if (m_AiMovement.reachedDestination)
            {
                m_Animator.Attack();
                M_Animator_onAttackStarted();
                return;
            }


            m_AiMovement.destination = m_PlayerTransform.position;  
        }
    }
}
