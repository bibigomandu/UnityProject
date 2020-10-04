using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proj.UIs {
    public class BarControl : MonoBehaviour {
        private Slider slider;
        
        public float MinimumValue {
            get => slider.minValue;
            set {
                slider.minValue = value;
            }
        }

        public float MaximumValue {
            get => slider.maxValue;
            set {
                slider.maxValue = value;
            }
        }

        public float value {
            get => slider.value;
            set {
                if(value <= slider.minValue)        slider.value = slider.minValue;
                else if(value >= slider.maxValue)   slider.value = slider.maxValue;
                else                                slider.value = value;
            }
        }

        private void Awake() {
            slider = gameObject.GetComponentInChildren<Slider>();
        }

        private void OnEnable() {
            GetComponent<Canvas>().enabled = true;
        }

        private void OnDisable() {
            GetComponent<Canvas>().enabled = false;
        }
    }
}