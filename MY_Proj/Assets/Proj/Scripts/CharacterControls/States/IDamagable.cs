using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proj.CharacterControls.States {
    public interface IDamagable {
        bool IsAlive {
            get;
        }

        void TakeDamage(int damage, GameObject hitEffect);
    }
}