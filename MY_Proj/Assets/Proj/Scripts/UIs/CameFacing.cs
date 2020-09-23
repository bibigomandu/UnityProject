using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proj.UIs
{
    public class CameFacing : MonoBehaviour
    {
        Camera refCamera;
        public bool reverseFace = false; // 반전 여부.
        public enum Axis // 빌보드 회전 기준.
        {
            up, down, left, right, forward, back
        };
        public Axis axis = Axis.up;
        public Vector3 GetAxis(Axis refAxis) {
            switch(refAxis) {
                case Axis.forward:
                    return Vector3.forward;
                case Axis.back:
                    return Vector3.back;
                case Axis.left:
                    return Vector3.left;
                case Axis.right:
                    return Vector3.right;
                case Axis.down:
                    return Vector3.down;
                default:
                    return Vector3.up;
            }
        }

        void Awake()
        {
            if(!refCamera)
            {
                refCamera = Camera.main;
            }
        }

        private void LateUpdate()
        {
            Vector3 targetPos = transform.position + refCamera.transform.rotation * (reverseFace ? Vector3.forward : Vector3.back);
            Vector3 targetOrientation = refCamera.transform.rotation * GetAxis(axis);

            transform.LookAt(targetPos, targetOrientation);
        }
    }
}