using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proj.CameraControls
{
    public class MainCameraControl : MonoBehaviour
    {
        [Header("Properties about camera control.")]
        public float height = 7f;
        public float distance = 8f;
        [Tooltip("Horizontal angle.")]
        [Range(30f, 60f)]
        public float angle = 45f;
        public float headHeight = 2f;
        public Transform target;

        private void LateUpdate()
        {
            CameraControl();
        }

        private void CameraControl()
        {
            if(!target) return;

            // 타겟과 카메라 간의 위치 차이.
            Vector3 positionCorrect = (Vector3.forward * -distance) + (Vector3.up * height);
            // Debug.DrawLine(target.position, positionCorrect, Color.red);

            // 타겟-카메라 위치 차이에 회전 적용.
            Vector3 rotatedPositionCorrect = Quaternion.AngleAxis(angle, Vector3.up) * positionCorrect;
            // Debug.DrawLine(target.position, rotatedPositionCorrect, Color.green);

            // 캐릭터의 머리를 바라본다.
            Vector3 lookAtPosition = target.position;
            lookAtPosition.y += headHeight;

            // 카메라 새 위치.
            Vector3 cameraPosition = lookAtPosition + rotatedPositionCorrect;
            // Debug.DrawLine(target.position, cameraPosition, Color.blue);

            // 위치 변경 후 캐릭터(머리)를 바라보도록 회전.
            transform.position = cameraPosition;
            transform.LookAt(lookAtPosition);
        }
    }
}