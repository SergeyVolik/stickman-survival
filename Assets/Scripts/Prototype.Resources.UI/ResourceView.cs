using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class ResourceView : MonoBehaviour
    {
        private ResourceContainer m_Resources;

        public Dictionary<ResourceTypeSO, ResourceUIItem> uiItems = new Dictionary<ResourceTypeSO, ResourceUIItem>();

        public void Bind(ResourceContainer resources)
        {
            m_Resources = resources;
            Setup();
            m_Resources.onResourceChanged += UpdateResourceUI;
        }

        private void UpdateResourceUI(ResourceTypeSO arg1, int arg2)
        {
            if (uiItems.TryGetValue(arg1, out var item))
            {
                item.SetText(TextUtils.IntToText(arg2));
                item.DoAnimation();
            }
            else
            {
                SetupUIItem(arg1, arg2);
            }
        }

        private void Setup()
        {
            foreach (var item in m_Resources.ResourceIterator())
            {
                SetupUIItem(item.Key, item.Value);
            }
        }

        private ResourceUIItem SetupUIItem(ResourceTypeSO type, int count)
        {
            var uiItem = GameObject
                .Instantiate(type.ResourceUIItem, transform)
                .GetComponent<ResourceUIItem>();

            uiItem.SetText(TextUtils.IntToText(count));

            uiItem.SetSprite(type.resourceIcon, type.resourceColor);
            uiItems.Add(type, uiItem);

            return uiItem;
        }

        public ResourceUIItem GetResourceItemsByType(ResourceTypeSO resourceType)
        {
            if (!uiItems.TryGetValue(resourceType, out var value))
            {
                value = SetupUIItem(resourceType, 0);
            }

            return value;
        }
    }
}
