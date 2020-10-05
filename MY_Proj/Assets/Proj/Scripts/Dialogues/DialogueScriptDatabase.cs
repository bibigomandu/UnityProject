using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proj.Dialogues {
    [CreateAssetMenu(fileName ="DialogueScriptDatabase", menuName = "Dialogue/dialogueScriptDatabase")]
    public class DialogueScriptDatabase : ScriptableObject {
        public DialogueScript[] scripts;
    }
}