using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proj.UIs {
    public class HPBarControl : MonoBehaviour {
        public Image image;
        private RectTransform rectTransform;
        private float max;
        private float val;
        private float width;
        private float height;

        void Start() {
            rectTransform = GetComponent<RectTransform>();
            width = rectTransform.rect.width;
            height = rectTransform.rect.height;
        }

        public void SetMaxValue(float value) {
            max = value;
        }

        public float GetMaxValue() {
            return max;
        }

        public void SetValue(float value) {
            val = value;
            Resize();
        }

        public float GetValue() {
            return val;
        }

        public void Resize() {
            float per = val / max;
            rectTransform.sizeDelta = new Vector2(per * width, height);
        }
    }
}