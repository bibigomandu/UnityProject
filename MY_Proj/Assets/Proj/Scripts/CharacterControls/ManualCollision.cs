using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proj.CharacterControls {
    public class ManualCollision : MonoBehaviour {
        public Vector3 boxSize = new Vector3(3, 2, 1.5f);

        public Collider[] CheckOverlapBox(LayerMask layerMask) {
            return Physics.OverlapBox(transform.position, boxSize * 0.5f, transform.rotation, layerMask);
        }

        void OnDrawGizmos() {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
        }
    }
}