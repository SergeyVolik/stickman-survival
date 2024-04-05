using DG.Tweening;
using Prototype;
using RobinGoodfellow.CircleGenerator;
using UnityEngine;

public class AimCirclerBehaviour : MonoBehaviour
{
    [SerializeField]
    public CircleGenerator m_AimCorclePrefab;

    private CircleGenerator m_AimRangeCircle;

    private Transform m_CircleTransform;
    private Transform m_Transform;
    public float rotationSpeed;

    private void Awake()
    {
        m_AimRangeCircle = GameObject.Instantiate(m_AimCorclePrefab);
        m_CircleTransform = m_AimRangeCircle.transform;
        m_Transform = transform;
        GetComponent<CharacterGunBehaviourV2>().onGunChanged += AimCirclerBehaviour_onGunChanged;
        m_CircleTransform.gameObject.SetActive(false);
    }

    bool m_Showed = false;

    void Show()
    {
        if (m_Showed) return;

        m_Showed = true;
        m_CircleTransform.gameObject.SetActive(true);
        m_CircleTransform.localScale = Vector3.zero;
        m_CircleTransform.DOScale(Vector3.one, 1f).SetEase(Ease.OutSine);
    }

    private void AimCirclerBehaviour_onGunChanged(Gun obj)
    {
        if (obj == null) return;

        var circle = m_AimRangeCircle.CircleData;
        circle.Radius = obj.aimDistance;
        m_AimRangeCircle.CircleData = circle;
        m_AimRangeCircle.Generate();

        Show();
    }

    private void Update()
    {
        m_CircleTransform.position = m_Transform.position + new Vector3(0, 0.01f, 0);
        m_CircleTransform.Rotate(new Vector3(0, 0, Time.deltaTime * rotationSpeed));
    }
}
