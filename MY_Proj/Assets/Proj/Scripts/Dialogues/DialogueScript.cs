using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proj.Dialogues {
    [CreateAssetMenu(fileName ="Dialogue", menuName = "Dialogue/dialogue")]
    public class DialogueScript : ScriptableObject {
        // 필요 진행도.
        public int requiredProcessivity;
        public int NPCID;
        public string[] dialogues;
    }
}