using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Proj.UIs
{
    public class DamageText : MonoBehaviour
    {
        private TextMeshProUGUI textMeshPro;
        public float destroyDelayTime = 1.0f;

        public int Damage
        {
            get
            {
                if(textMeshPro != null)
                {
                    return int.Parse(textMeshPro.text);
                }
                return 0;
            }
            set
            {
                if(textMeshPro != null)
                {
                    textMeshPro.text = value.ToString();
                }
            }
        }

        private void Awake()
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            Destroy(gameObject, destroyDelayTime);
        }
    }
}