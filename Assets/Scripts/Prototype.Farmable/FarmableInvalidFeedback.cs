using MoreMountains.Feedbacks;
using Prototype;
using UnityEngine;
using Zenject;

public class FarmableInvalidFeedback : MonoBehaviour
{
    public MMF_Player feedback;
    private WorldSpaceMessageFactory m_factory;

    [Inject]
    void Constrct(WorldSpaceMessageFactory factory)
    {
        m_factory = factory;
    }

    private void Awake()
    {
        var farmable = GetComponent<FarmableObject>();
        farmable.onMeleeWeaponFailed += () => {
            m_factory.SpawnAtPosition(feedback.transform.position, $"Required Weapon {farmable.RequiredWeaponLevel}");
            feedback.StopFeedbacks();
            feedback.PlayFeedbacks(feedback.transform.position);
        };
    }
}
