using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proj.Items
{
    [Serializable]
    public class ItemBuff
    {
        public CharacterAttribute stat;
        public int value;

        private int min;
        private int max;

        public int Min => Min;
        public int Max => Max;

        public ItemBuff(int min, int max)
        {
            this.min = min;
            this.max = max;

            GenerateValue();
        }

        public void GenerateValue()
        {
            value = UnityEngine.Random.Range(min, max);
        }

        public void AddValue(ref int v)
        {
            v += value;
        }
    }
}