using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proj.Inventories;

namespace Proj.UIs.Inventories
{
    public class DynamicInventoryUI : InventoryUI
    {
        [SerializeField]
        protected GameObject slotPrefab;
        [SerializeField]
        protected Vector2 start;
        [SerializeField]
        protected Vector2 size;
        [SerializeField]
        protected Vector2 space;
        [Min(1), SerializeField]
        protected int numberOfColumn = 4;

        public override void CreateSlotUIs()
        {
            slotUIs = new Dictionary<GameObject, InventorySlot>();

            for(int i = 0; i < inventoryObject.Slots.Length; i++)
            {
                GameObject uiGO = Instantiate(slotPrefab, Vector3.zero, Quaternion.identity,
                    transform);
                uiGO.GetComponent<RectTransform>().anchoredPosition = CalculatePosition(i);

                AddEvent(uiGO, UnityEngine.EventSystems.EventTriggerType.PointerEnter,
                    delegate { OnEnter(uiGO); });
                AddEvent(uiGO, UnityEngine.EventSystems.EventTriggerType.PointerExit,
                    delegate { OnExit(uiGO); });
                AddEvent(uiGO, UnityEngine.EventSystems.EventTriggerType.BeginDrag,
                    delegate { OnStartDrag(uiGO); });
                AddEvent(uiGO, UnityEngine.EventSystems.EventTriggerType.EndDrag,
                    delegate { OnEndDrag(uiGO); });
                AddEvent(uiGO, UnityEngine.EventSystems.EventTriggerType.Drag,
                    delegate { OnDrag(uiGO); });

                inventoryObject.Slots[i].slotUI = uiGO;
                slotUIs.Add(uiGO, inventoryObject.Slots[i]);
            }
        }

        public Vector3 CalculatePosition(int i)
        {
            float x = start.x + ((space.x + size.x) * (i % numberOfColumn));
            float y = start.y + (-(space.y + size.y) * (i / numberOfColumn));

            return new Vector3(x, y, 0);
        }
    }
}