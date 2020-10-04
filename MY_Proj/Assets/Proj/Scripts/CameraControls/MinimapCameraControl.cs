using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proj.CameraControls {
    public class MinimapCameraControl : MonoBehaviour {
        [Tooltip("Player")]
        public Transform target;
        public int height = 500;

        void Update() {
            Vector3 newPos = target.transform.position;
            newPos.y = height;
            transform.position = newPos;
        }
    }
}