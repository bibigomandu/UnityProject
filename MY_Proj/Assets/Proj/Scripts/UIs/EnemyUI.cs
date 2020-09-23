using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proj.UIs
{
    public class EnemyUI : MonoBehaviour
    {
        private Slider hpSlider;
        public GameObject damageTextPrefab;

        public float MinimumValue
        {
            get => hpSlider.minValue;
            set
            {
                hpSlider.minValue = value;
            }
        }

        public float MaximumValue
        {
            get => hpSlider.maxValue;
            set
            {
                hpSlider.maxValue = value;
            }
        }

        public float value
        {
            get => hpSlider.value;
            set
            {
                hpSlider.value = value;
            }
        }

        private void Awake()
        {
            hpSlider = gameObject.GetComponentInChildren<Slider>();
        }

        private void OnEnable()
        {
            GetComponent<Canvas>().enabled = true;
        }

        private void OnDisable()
        {
            GetComponent<Canvas>().enabled = false;
        }

        public void TakeDamage(int damage)
        {
            if(damageTextPrefab != null)
            {
                GameObject damageTextGO = Instantiate(damageTextPrefab, transform);
                DamageText damageText = damageTextGO.GetComponent<DamageText>();
                if(damageText == null)
                {
                    Destroy(damageTextGO);
                }

                damageText.Damage = damage;
            }
        }
    }
}