using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Proj.Inventories;
using Proj.Items;

namespace Proj.UIs.Inventories
{
    [RequireComponent(typeof(EventTrigger))] // 마우스 입력이 중요하므로 이벤트 트리거가 필요.
    public abstract class InventoryUI : MonoBehaviour
    {
#region
        public InventoryObject inventoryObject;
        private InventoryObject previousInventoryObject;

        public Dictionary<GameObject, InventorySlot> slotUIs
            = new Dictionary<GameObject, InventorySlot>();
#endregion
        
#region     Unity Methods
        private void Awake()
        {
            CreateSlotUIs();

            for(int i = 0; i < inventoryObject.Slots.Length; i++)
            {
                inventoryObject.Slots[i].parent = inventoryObject;
                inventoryObject.Slots[i].OnPostUpdate += OnPostUpdate;
            }

            AddEvent
            (
                gameObject, EventTriggerType.PointerEnter,
                delegate { OnEnter(gameObject); }
            );
            AddEvent
            (
                gameObject, EventTriggerType.PointerExit,
                delegate { OnExit(gameObject); }
            );
        }

        protected virtual void Start()
        {
            for(int i = 0; i < inventoryObject.Slots.Length; i++)
            {
                inventoryObject.Slots[i].UpdateSlot(inventoryObject.Slots[i].item, inventoryObject.Slots[i].amount);
            }
        }
#endregion  Unity Methods

        public abstract void CreateSlotUIs();

        protected void AddEvent
        (
            GameObject gameObject, EventTriggerType type,
            UnityAction<BaseEventData> action
        )
        {
            EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
            if(trigger == null)
            {
                return;
            }

            EventTrigger.Entry eventTrigger = new EventTrigger.Entry {eventID = type};
            eventTrigger.callback.AddListener(action);
            trigger.triggers.Add(eventTrigger);
        }

        public void OnPostUpdate(InventorySlot slot)
        {
            slot.slotUI.transform.GetChild(0).GetComponent<Image>().sprite
                = slot.item.id < 0 ? null : slot.ItemObject.icon;
            slot.slotUI.transform.GetChild(0).GetComponent<Image>().color
                = slot.item.id < 0 ? new Color(1, 1, 1, 0) : new Color(1, 1, 1, 1);
            slot.slotUI.GetComponentInChildren<TextMeshProUGUI>().text
                = slot.item.id < 0 ?
                    string.Empty :
                    (slot.amount == 1 ? string.Empty : slot.amount.ToString("n0"));
        }

        // 마우스가 어떤 인벤토리 창 위에 있는지 구분하기 위해 임시로 컴포넌트를 갖는다.
        public void OnEnter(GameObject go)
        {
            MouseData.interfaceMouseIsOver = go.GetComponent<InventoryUI>();
        }

        public void OnExit(GameObject go)
        {
            MouseData.interfaceMouseIsOver = null;
        }

        public void OnEnterSlot(GameObject go)
        {
            MouseData.slotHoveredOver = go;
        }

        public void OnExitSlot(GameObject go)
        {
            MouseData.slotHoveredOver = null;
        }

        private GameObject CreateDragImage(GameObject go)
        {
            if(slotUIs[go].item.id < 0)
            {
                return null;
            }

            GameObject dragImage = new GameObject();
            RectTransform rectTransform = dragImage.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(40, 40);
            dragImage.transform.SetParent(transform.parent);

            Image image = dragImage.AddComponent<Image>();
            image.sprite = slotUIs[go].ItemObject.icon;
            image.raycastTarget = false;
            
            dragImage.name = "Drag Image";

            return dragImage;
        }

        public void OnStartDrag(GameObject go)
        {
            MouseData.tempItemBeingDragged = CreateDragImage(go);
        }

        public void OnDrag(GameObject go)
        {
            if(MouseData.tempItemBeingDragged == null)
            {
                return;
            }

            MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position
                = Input.mousePosition;
        }

        public void OnEndDrag(GameObject go)
        {
            Destroy(MouseData.tempItemBeingDragged);
            // if(MouseData.interfaceMouseIsOver == null)
            // {
            //     // Remove this item.
            //     slotUIs[go].RemoveItem();
            // }
            // else
            if(MouseData.slotHoveredOver != null)
            {
                InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotUIs[MouseData.slotHoveredOver];
                inventoryObject.SwapItems(slotUIs[go], mouseHoverSlotData);
            }
        }
    } // public abstract class InventoryUI
} // namespace