using Prototype;
using UnityEngine;
using Zenject;

public class CameraController : MonoBehaviour
{
    public Transform cameraTarget;
    private PlayerSpawnFactory m_factory;

    [Inject]
    void Construct(PlayerSpawnFactory factory)
    {
        m_factory = factory;
    }

    private void Update()
    {
        if (!m_factory.CurrentPlayerUnit)
            return;

        cameraTarget.position = m_factory.CurrentPlayerUnit.transform.position;
    }
}
