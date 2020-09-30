using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proj.Items;

namespace Proj.Inventories
{
    [CreateAssetMenu(fileName = "New Invnetory", menuName = "Inventory System/Inventory")]
    public class InventoryObject : ScriptableObject
    {
        public ItemObjectDatabase database;
        public InterfaceType type;

        [SerializeField]
        private Inventory container = new Inventory();
        public InventorySlot[] Slots => container.slots;

        public int EmptySlotCount
        {
            get
            {
                int counter = 0;
                foreach(InventorySlot slot in Slots)
                {
                    if(slot.item.id < 0)
                    {
                        counter++;
                    }
                }
                return counter;
            }
        }

        public bool AddItem(Item item, int amount)
        {
            if(EmptySlotCount <= 0)
            {
                return false;
            }

            InventorySlot slot = FindItemInInventory(item);
            if(!database.itemObjects[item.id].stackable || slot == null)
            {
                GetEmptySlot().AddItem(item, amount);
            }
            else
            {
                slot.AddAmount(amount);
            }

            return true;
        }

        public InventorySlot FindItemInInventory(Item item)
        {
            return Slots.FirstOrDefault(i => i.item.id == item.id);
        }

        public InventorySlot GetEmptySlot()
        {
            return Slots.FirstOrDefault(i => i.item.id <= -1);
        }

        public bool IsContainItem(ItemObject itemObject)
        {
            return Slots.FirstOrDefault(i => i.item.id == itemObject.data.id) != null;
        }

        public void SwapItems(InventorySlot itemSlotA, InventorySlot itemSlotB)
        {
            if(itemSlotA == itemSlotB)
            {
                return;
            }

            if(itemSlotA.CanPlaceInSlot(itemSlotB.ItemObject)
                && itemSlotB.CanPlaceInSlot(itemSlotA.ItemObject))
            {
                InventorySlot tmp = new InventorySlot(itemSlotB.item, itemSlotB.amount);
                itemSlotB.UpdateSlot(itemSlotA.item, itemSlotA.amount);
                itemSlotA.UpdateSlot(tmp.item, tmp.amount);
            }
        }
    }
}