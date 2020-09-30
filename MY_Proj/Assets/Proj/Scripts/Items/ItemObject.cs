using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proj.Items
{
    [CreateAssetMenu(fileName ="New Item", menuName = "Inventory System/Items/New Item")]
    public class ItemObject : ScriptableObject
    {
        public ItemType type;
        // 물약과 같이 수량을 가지는가.
        public bool stackable;

        public Sprite icon;
        public GameObject modelPrefab;

        public Item data = new Item();
        public List<string> boneNames = new List<string>();

        [TextArea(15, 20)]
        public string description;

        private void OnValidate()
        {
            boneNames.Clear();

            if(modelPrefab == null || modelPrefab.GetComponentInChildren<SkinnedMeshRenderer>() == null)
            {
                return;
            }

            SkinnedMeshRenderer renderer = modelPrefab.GetComponentInChildren<SkinnedMeshRenderer>();
            Transform[] bones = renderer.bones;

            foreach(Transform t in bones)
            {
                boneNames.Add(t.name);
            }
        }

        public Item CreateItem()
        {
            Item newItem = new Item(this);
            return newItem;
        }
    }
}