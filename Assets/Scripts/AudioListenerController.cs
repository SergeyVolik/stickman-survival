using UnityEngine;

public class AudioListenerController : MonoBehaviour
{
    public Transform audioListenerTarget;
    private Transform m_Transform;

    private void Awake()
    {
        m_Transform = transform;
    }

    private void Update()
    {

        if (audioListenerTarget)
            m_Transform.position = audioListenerTarget.position;
        else m_Transform.localPosition = Vector3.zero;
    }
}
