using UnityEngine;
using Zenject;

public class QuestOnScreenPointer : MonoBehaviour
{
    public RectTransform questPointer;
    private WorldToScreenUIManager m_wtsManager;
    private QuestQueue m_QuestQueue;

    [Inject]
    void Construct(WorldToScreenUIManager wtsManager)
    {
        m_wtsManager = wtsManager;
    }
    private Camera m_Camera;

    private void Awake()
    {
        m_Camera = Camera.main;
        m_QuestQueue = GetComponent<QuestQueue>();
    }

    private void Update()
    {
        var quest = m_QuestQueue.GetCurrentQuest();
        bool enableOnScrenItem = false;
        if (quest)
        {
           var questObject = quest.GetQuestTargetObject();

            if (questObject)
            {
                var pos = questObject.position;
                var speenPos = m_Camera.WorldToScreenPoint(pos);
                var isOnScreen = speenPos.x > 0f && speenPos.x < Screen.width && speenPos.y > 0f && speenPos.y < Screen.height;
                enableOnScrenItem = !isOnScreen;            
                var parentRect = (RectTransform)questPointer.parent;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, speenPos, null, out var anchorPos);

                var bounds = parentRect.rect;
                bounds.width -= questPointer.rect.width;
                bounds.height -= questPointer.rect.height;

                anchorPos.x = Mathf.Clamp(anchorPos.x, bounds.min.x, bounds.max.x);
                anchorPos.y = Mathf.Clamp(anchorPos.y, bounds.min.y, bounds.max.y);

                questPointer.anchoredPosition = anchorPos;
            }
        }

        questPointer.gameObject.SetActive(enableOnScrenItem);
    }
}
