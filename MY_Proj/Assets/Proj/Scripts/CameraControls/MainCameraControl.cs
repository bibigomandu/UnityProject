using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proj.CameraControls {
    public class MainCameraControl : MonoBehaviour {
        [Header("Properties about camera control.")]
        [Range(2f, 4f)]
        public float distance = 3f;
        private float minDistance = 2f;
        private float maxDistance = 4f;

        [Tooltip("Character stature.")] // stature = 키.
        [Range(1f, 2f)]
        public float characterStature = 2f;

        [Range(5f, 10f)]
        public float rotateSpeed = 7.0f;
        // 60 - 0(360) - 300
        private float downVerticalRotLimit = 60f;
        private float upVerticalRotLimit = 300f;

        [Range(5f, 10f)]
        public float zoomSpeed = 10.0f;

        [Tooltip("Player")]
        public Transform target;

        private void LateUpdate() {
            CameraControl();
        }

        private void CameraControl() {
            if(!target) return;

            ChangeDistance();
            ChangeRotation();
            ChangePosition();
        }

        private void ChangeDistance() {
            // 마우스 휠을 스크롤해 카메라와 캐릭터 사이의 거리를 변경.
            float deltaDist = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            distance -= deltaDist;
            if(distance < minDistance)  distance = minDistance;
            if(distance > maxDistance)  distance = maxDistance;
        }

        private void ChangeRotation() {
            // 커서가 보이지 않을 때에만 회전.
            if(Cursor.lockState == CursorLockMode.Locked) {
                float deltaVerticalRot = Input.GetAxis("Mouse Y") * -1 * rotateSpeed;
                float deltaHorizontalRot = Input.GetAxis("Mouse X") * rotateSpeed;

                Vector3 rot = transform.rotation.eulerAngles;
                rot.x += deltaVerticalRot;
                rot.y += deltaHorizontalRot;

                // rotation의 x(수직회전) 값을 지정된 범위를 벗어나지 않도록 처리.
                // 60(downVerticalRotLimit) - 0(360) - 300(upVerticalRotLimit)
                rot.x %= 360;

                if(downVerticalRotLimit < rot.x && rot.x <= 180)
                    rot.x = downVerticalRotLimit;
                else if(180 <= rot.x && rot.x < upVerticalRotLimit)
                    rot.x = upVerticalRotLimit;
                
                Quaternion newRot = Quaternion.Euler(rot);
                transform.rotation = newRot;
            }
        }

        private void ChangePosition() {
            // 카메라가 캐릭터를 뒤에서 바라보도록 position 변경.
            Vector3 normRot = transform.forward.normalized;
            Vector3 positionDiff = normRot * -distance;

            // 캐릭터 키를 고려해 y값 수정.
            positionDiff.y += characterStature;
            transform.position = positionDiff + target.position;
        }
    }
}