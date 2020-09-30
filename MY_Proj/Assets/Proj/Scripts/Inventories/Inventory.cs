using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Proj.Items;

namespace Proj.Inventories
{
    [Serializable]
    public class Inventory
    {
        private const int SLOT_SIZE = 24;
        public InventorySlot[] slots = new InventorySlot[SLOT_SIZE];

        public void Clear()
        {
            foreach(InventorySlot slot in slots)
            {
                slot.RemoveItem();
            }
        }

        public bool IsContain(ItemObject itemObject)
        {
            return IsContain(itemObject.data.id);
        }

        public bool IsContain(int id)
        {
            return slots.FirstOrDefault(i => i.item.id == id) != null;
        }
    }
}