using MoreMountains.Tools;
using Prototype;
using UnityEngine;

public class CharacterHealthbar : MonoBehaviour
{
    public MMHealthBar healthbar;
    public Transform UIRoot;

    private HealthData m_Health;
    private Camera m_Camera;

    private void Awake()
    {
        m_Health = GetComponent<HealthData>();
        m_Health.onHealthChanged += CharacterHealthbar_onHealthChanged;
        m_Camera = Camera.main;

        healthbar.TargetProgressBar.TextValueMultiplier = m_Health.maxHealth;
        healthbar.UpdateBar(m_Health.currentHealth, 0, m_Health.maxHealth, true);
    }

    private void Update()
    {
        UIRoot.forward = m_Camera.transform.forward;
    }

    private void CharacterHealthbar_onHealthChanged(HealthChangeData obj)
    {
        bool show = m_Health.currentHealth == 0;

        healthbar.UpdateBar(m_Health.currentHealth, 0, m_Health.maxHealth, show);
    }
}
