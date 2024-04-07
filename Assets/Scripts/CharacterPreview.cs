using Prototype;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class CharacterPreview : MonoBehaviour, IDragHandler
{
    private IPlayerFactory m_playerFactory;
    public float rotationSpeed;
    [Inject]
    void Construct(IPlayerFactory playerFactory)
    {
        m_playerFactory = playerFactory;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var delta = eventData.delta;

        m_playerFactory.CurrentPlayerUnit.GetComponent<PlayerPreviewCamera>().RotateCamera(rotationSpeed * delta.x);
    }
}
