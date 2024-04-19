using Prototype;
using Prototype.Ads;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BustButton : MonoBehaviour
{
    public Effect effectPrefab;
    private IPlayerFactory m_factory;
    private IAdsPlayer m_adsManager;

    [Inject]
    void Construc(IPlayerFactory factory, IAdsPlayer adsManager)
    {
        m_factory = factory;
        m_adsManager = adsManager;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            m_adsManager.ShowRewardAd(null);
            m_factory.CurrentPlayerUnit.GetComponent<UnitEffects>().AddEffect(effectPrefab);
            gameObject.SetActive(false);
        });
    }
}
