using System.Collections.Generic;
using UnityEngine;

public class WordlToScreenUIItem
{
    public RectTransform item;
    public Transform worldPositionTransform;
}
public class WorldToScreenUIManager : MonoBehaviour
{
    private List<WordlToScreenUIItem> m_Items = new List<WordlToScreenUIItem>(10);

    [SerializeField]
    public RectTransform m_Root;

    public RectTransform Root => m_Root;

    private Camera m_Camera;

    private void Awake()
    {
        m_Camera = Camera.main;
    }

    public WordlToScreenUIItem Register(WordlToScreenUIItem item)
    {
        item.item.anchorMax = new Vector2(0.5f, 0.5f);
        item.item.anchorMin = new Vector2(0.5f, 0.5f);

        m_Items.Add(item);
        return item;
    }

    public void Unregister(WordlToScreenUIItem item)
    {
        m_Items.Remove(item);
    }

    private void Update()
    {
        foreach (var item in m_Items)
        {
            var pos = item.worldPositionTransform.position;
            var speenPos = m_Camera.WorldToScreenPoint(pos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)item.item.parent, speenPos, null, out var anchorPos);
            item.item.anchoredPosition = anchorPos;
        }
    }
}
