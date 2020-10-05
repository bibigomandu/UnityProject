using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proj.Dialogues {
    public class DialogueControl : MonoBehaviour {
        public int NPCID;
        public GameObject btnPrefab;
        public GameObject dialogueUIPrefab;
        public Processivity processivity;
        public DialogueScriptDatabase dialogueScriptDatabase;

        void Start() {
            processivity = processivity.GetComponent<Processivity>();
        }

        public void ShowDialogueBtn() {
            if(dialogueScriptDatabase.scripts[processivity.GetProcessivity()].NPCID == NPCID) {
                GameObject btnGO = Instantiate(btnPrefab, transform);
                Destroy(btnGO, 0.5f);
            }
        }

        public void OpenDialogueUI() {
            if(dialogueScriptDatabase.scripts[processivity.GetProcessivity()].NPCID == NPCID) {
                GameObject uiGO = Instantiate(dialogueUIPrefab);
                DialogueUI ui = uiGO.GetComponent<DialogueUI>();
                ui.SetDialoguiScript(dialogueScriptDatabase.scripts[processivity.GetProcessivity()], processivity);
            }
        }
    }
}