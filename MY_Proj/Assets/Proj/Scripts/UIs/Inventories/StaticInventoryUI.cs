using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proj.Inventories;

namespace Proj.UIs.Inventories
{
    public class StaticInventoryUI : InventoryUI
    {
        public GameObject[] staticSlots = null;

        public override void CreateSlotUIs()
        {
            slotUIs = new Dictionary<GameObject, InventorySlot>();
            for(int i = 0; i < inventoryObject.Slots.Length; i++)
            {
                GameObject uiGO = staticSlots[i];

                AddEvent(uiGO, UnityEngine.EventSystems.EventTriggerType.PointerEnter, delegate { OnEnter(uiGO); });
                AddEvent(uiGO, UnityEngine.EventSystems.EventTriggerType.PointerExit, delegate { OnExit(uiGO); });
                AddEvent(uiGO, UnityEngine.EventSystems.EventTriggerType.BeginDrag, delegate { OnStartDrag(uiGO); });
                AddEvent(uiGO, UnityEngine.EventSystems.EventTriggerType.EndDrag, delegate { OnEndDrag(uiGO); });
                AddEvent(uiGO, UnityEngine.EventSystems.EventTriggerType.Drag, delegate { OnDrag(uiGO); });

                inventoryObject.Slots[i].slotUI = uiGO;
                slotUIs.Add(uiGO, inventoryObject.Slots[i]);

                uiGO.name += ": " + i;
            }
        }
    }
}