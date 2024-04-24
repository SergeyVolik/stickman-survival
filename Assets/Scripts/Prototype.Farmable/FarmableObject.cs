using DG.Tweening;
using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Prototype
{
    [RequireComponent(typeof(HealthData))]
    public class FarmableObject : MonoBehaviour, IRequiredMeleeWeapon
    {
        [SerializeField]
        private MeleeWeaponType m_RequiredWeapon;

        [SerializeField]
        private ParticleSystem m_HitParticle;

        [SerializeField]
        private ParticleSystem m_DeathParticle;

        [SerializeField]
        private ParticleSystem m_PartRemoveParticle;

        public MeleeWeaponType RequiredWeapon => m_RequiredWeapon;

        [field: SerializeField]
        public int RequiredWeaponLevel { get; set; }

        [SerializeField]
        private Transform m_PartsParent;

        [FormerlySerializedAs("Parts")]
        [SerializeField]
        private GameObject[] m_Parts;

        [SerializeField]
        private ResourceContainer m_StartResource;
        [Range(0, 1)]
        public float partsVisualizationStartPercent = 1f;
        [SerializeField]
        private ResourceContainer m_OnDeathDrop;

        private ResourceContainer m_ArlreadyDroppedResources;
        private ResourceContainer m_ToDrop;
        private HealthData m_Health;
        private Collider m_Collider;
        private DropExecutor m_DropExecutor;

        public event Action onMeleeWeaponFailed = delegate { };

        private void Awake()
        {
            m_Health = GetComponent<HealthData>();
            m_Collider = GetComponent<Collider>();
            m_ArlreadyDroppedResources = new ResourceContainer();
            m_ToDrop = new ResourceContainer();
            m_Health.onHealthChanged += M_Health_onHealthChaged;
            m_Health.onDeath += M_Health_onDeath;
            m_Health.onResurrected += M_Health_onResurrected;
            m_DropExecutor = GetComponent<DropExecutor>();
        }

        private void M_Health_onResurrected()
        {
            m_PartsParent.localScale = new Vector3(1, 0, 1);
            m_PartsParent.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutSine);
            m_ArlreadyDroppedResources.Clear();
        }

        private void M_Health_onDeath()
        {
            var target = m_Health.KilledBy;
            if (m_Health.KilledBy.TryGetComponent<IOwnable>(out var owner))
            {
                target = owner.Owner;
            }

            m_DropExecutor.ExecuteDrop(target, m_OnDeathDrop);

            if (m_DeathParticle != null)
            {               
                m_DeathParticle.Play();
            }
        }

        private void M_Health_onHealthChaged(HealthChangeData obj)
        {
            UpdateObjectParts();

            m_Collider.enabled = !m_Health.IsDead;

            if (obj.Source != null && obj.IsDamage)
            {
                if (m_HitParticle != null)
                {
                    m_HitParticle.Play();
                }

                var holderObj = obj.Source;

                if (!TryAddResource(holderObj) && holderObj.TryGetComponent<IOwnable>(out var owner))
                {
                    TryAddResource(owner.Owner);
                }
            }
        }

        private void UpdateObjectParts()
        {
            if (m_Health.HasMaxHealth())
            {
                for (int i = 0; i < m_Parts.Length; i++)
                {
                    m_Parts[i].SetActive(true);
                }
            }
            else if (m_Health.IsDead)
            {
                for (int i = 0; i < m_Parts.Length; i++)
                {
                    m_Parts[i].SetActive(false);
                }
            }
            else
            {
                var currentHealthPercent = m_Health.currentHealth / (float)m_Health.maxHealth;

                float visualizationPercent = currentHealthPercent;
                if (visualizationPercent >= partsVisualizationStartPercent)
                {
                    visualizationPercent = 1f;
                }
                else {
                    visualizationPercent = math.remap(0, partsVisualizationStartPercent, 0, 1, visualizationPercent);
                }

                var lastIndex = m_Parts.Length - 1;
                int ActiveParts = lastIndex - (int)(visualizationPercent * lastIndex);
               
                bool partRemoved = false;

                for (int i = 0; i < m_Parts.Length; i++)
                {
                    bool activateFirstElement = i == 0 && m_Health.HasMaxHealth();
                    bool activateLastElement = !m_Health.IsDead && i == m_Parts.Length - 1;
                    bool otherPartsActivate = ActiveParts <= i;

                    bool activatePart = otherPartsActivate || activateFirstElement || activateLastElement;
                    if (partRemoved == false)
                    {
                        partRemoved = m_Parts[i].activeSelf && !activatePart;
                    }
                    m_Parts[i].SetActive(otherPartsActivate || activateFirstElement || activateLastElement);
                }

                if (partRemoved == true)
                {
                    if (m_PartRemoveParticle != null)
                    {
                        m_PartRemoveParticle.Play();
                    }
                }
            }
        }


        private bool TryAddResource(GameObject holderObj)
        {
            if (holderObj.TryGetComponent<IResourceHolder>(out var holder))
            {
                var currentHealthPercent = m_Health.currentHealth / (float)m_Health.maxHealth;

                m_ToDrop.Clear();

                foreach (var item in m_StartResource.ResourceIterator())
                {
                    ResourceTypeSO resourceType = item.Key;
                    int resourceCount = item.Value;

                    int healthDiffToDrop = resourceCount - Mathf.RoundToInt(resourceCount * currentHealthPercent);

                    var alreadyDropped = m_ArlreadyDroppedResources.GetResource(resourceType);
                    int toDrop = healthDiffToDrop - alreadyDropped;

                    if (toDrop == 0)
                        continue;
                 
                    m_ArlreadyDroppedResources.SetResource(resourceType, healthDiffToDrop);
                    m_ToDrop.AddResource(resourceType, toDrop);            
                }

                m_DropExecutor.ExecuteDrop(holderObj, m_ToDrop);

                return true;
            }

            return false;
        }

        public bool Validate(int level, MeleeWeaponType weaponType)
        {
            if (RequiredWeaponLevel > level)
            {
                onMeleeWeaponFailed.Invoke();
                return false;
            }

            if (weaponType > m_RequiredWeapon)
            {
                onMeleeWeaponFailed.Invoke();
                return false;
            }
            return true;
        }
    }
}
