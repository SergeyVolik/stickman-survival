using UnityEngine;

namespace Prototype
{
    public class ShowParticleEffect : MonoBehaviour, IEffectExtention
    {
        public ParticleSystem effectParticle;
        private ParticleSystem m_particle;

        public void Clear()
        {
            m_particle.Stop();
            m_particle.GetComponent<PoolObject>().Release();
        }

        public void Setup(GameObject target)
        {
            m_particle = GameObjectPool.GetPoolObject<ParticleSystem>(effectParticle);
            m_particle.gameObject.SetActive(true);
            m_particle.transform.parent = transform;
            m_particle.transform.localPosition = Vector3.zero;
            m_particle.Play();
        }
    }
}
