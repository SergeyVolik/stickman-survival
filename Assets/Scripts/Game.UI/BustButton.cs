using Prototype;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BustButton : MonoBehaviour
{
    public Effect effectPrefab;
    private IPlayerFactory m_factory;

    [Inject]
    void Construc(IPlayerFactory factory)
    {
        m_factory = factory;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            m_factory.CurrentPlayerUnit.GetComponent<UnitEffects>().AddEffect(effectPrefab);
            gameObject.SetActive(false);
        });
    }
}
