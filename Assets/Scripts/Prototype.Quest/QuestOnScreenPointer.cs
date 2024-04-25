using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    public class QuestOnScreenPointer : MonoBehaviour
    {
        public Color defaultPointerColor;
        public RectTransform questPointerPrefab;
        private QuestQueue m_QuestQueue;
        public RectTransform pointersRoot;
        private Camera m_Camera;

        private class PointerData
        {
            public Image img;
            public RectTransform rectTrans;
            public GameObject go;
        }
        List<PointerData> pointers = new List<PointerData>();

        private void Awake()
        {
            m_Camera = Camera.main;
            m_QuestQueue = GetComponent<QuestQueue>();
            CreateNewPointer();
        }
        void CreateNewPointer()
        {
            var instance = GameObject.Instantiate(questPointerPrefab, pointersRoot);
            pointers.Add(new PointerData
            {
                go = instance.gameObject,
                img = instance.GetComponent<Image>(),
                rectTrans = instance
            });
        }

        private void Update()
        {
            var quest = m_QuestQueue.GetCurrentQuest();

            foreach (var item in pointers)
            {
                item.go.SetActive(false);
            }

            if (quest == null)
                return;

            var pointerColor = quest.overridePointrColor ? quest.pointerColor : defaultPointerColor;

            switch (quest.pointerMode)
            {
                case Prototype.TargetPonterMode.None:
                    break;
                case Prototype.TargetPonterMode.One:
                    var questPointer = pointers[0];

                    if (quest)
                    {
                        var questObject = quest.GetQuestTargetObject();
                        UpdateQuestPointer(questPointer, questObject, pointerColor);
                    }
                    break;
                case Prototype.TargetPonterMode.All:
                    if (quest)
                    {
                        var questObjects = quest.GetQuestTargetObjects();
                        var diff = questObjects.Count() - pointers.Count;
                        if (diff > 0)
                        {
                            for (int i = 0; i < diff; i++)
                            {
                                CreateNewPointer();
                            }
                        }

                        int j = 0;
                        foreach (var item in questObjects)
                        {
                            UpdateQuestPointer(pointers[j], item, pointerColor);
                            j++;
                        }
                    }
                    break;
                default:
                    break;
            }


        }

        private void UpdateQuestPointer(PointerData questPointerData, Transform questObject, Color color)
        {
            questPointerData.img.color = color;
            bool enableOnScrenItem;
            var questPointer = questPointerData.rectTrans;

            var pos = questObject.position;
            var speenPos = m_Camera.WorldToScreenPoint(pos);
            var isOnScreen = speenPos.x > 0f && speenPos.x < Screen.width && speenPos.y > 0f && speenPos.y < Screen.height;
            enableOnScrenItem = !isOnScreen;
            var parentRect = (RectTransform)questPointer.parent;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, speenPos, null, out var anchorPos);

            var bounds = parentRect.rect;

            var pointerRect = questPointer.rect;
            var pointerRectWidthHalf = pointerRect.width;
            var pointerRectHeightHalf = pointerRect.height;

            anchorPos.x = Mathf.Clamp(anchorPos.x, bounds.min.x + pointerRectWidthHalf, bounds.max.x - pointerRectWidthHalf);
            anchorPos.y = Mathf.Clamp(anchorPos.y, bounds.min.y + pointerRectHeightHalf, bounds.max.y - pointerRectHeightHalf);

            questPointer.anchoredPosition = anchorPos;

            questPointer.gameObject.SetActive(enableOnScrenItem);
        }
    }

}
