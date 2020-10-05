using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proj.Dialogues {
    public class DialogueUI : MonoBehaviour {
        private DialogueScript script;
        public Text text;
        private Processivity processivity;
        private int idx = 0;

        // Start is called before the first frame update
        void Start() {
            
        }

        // Update is called once per frame
        void Update() {
            
        }

        public void SetDialoguiScript(DialogueScript sc, Processivity pcv) {
            script = sc;
            processivity = pcv;
            text.text = script.dialogues[idx];
        }

        public void Next() {
            if(idx != script.dialogues.Length - 1) {
                idx++;
                text.text = script.dialogues[idx];
            } else {
                processivity.SetProcessivity(processivity.GetProcessivity() + 1);
                Destroy(this.gameObject);
            }
        }
    }
}