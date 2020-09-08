using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Proj.CameraControls
{
    [CustomEditor(typeof(MainCameraControl))] // 이게 있어야 표시됨.
    public class MainCameraScreenEditor : Editor
    {
        private MainCameraControl mainCameraControl;

        public override void OnInspectorGUI()
        {
            mainCameraControl = (MainCameraControl)target; // 어디서 들어온 건지 모르겠으나 target외에는 안 됨.
            base.OnInspectorGUI();
        }

        private void OnSceneGUI()
        {
            if(!mainCameraControl || !mainCameraControl.target)
            {
                return;
            }

            Transform cameraTarget = mainCameraControl.target;
            Vector3 targetPosition = cameraTarget.position;
            targetPosition.y += mainCameraControl.headHeight;

            Handles.color = new Color(1f, 0f, 0f, 0.15f);
            Handles.DrawSolidDisc(targetPosition, Vector3.up, mainCameraControl.distance);

            Handles.color = new Color(0f, 1f, 0f, 0.75f);
            Handles.DrawWireDisc(targetPosition, Vector3.up, mainCameraControl.distance);

            // 거리-높이 제어.
            Handles.color = new Color(1f, 0f, 0f, 0.5f);
            mainCameraControl.distance = Handles.ScaleSlider(mainCameraControl.distance,
                targetPosition, -cameraTarget.forward, Quaternion.identity,
                mainCameraControl.distance, 0.1f);
            // 최대-최소값 제한.
            mainCameraControl.distance = Mathf.Clamp(mainCameraControl.distance, 4f, 16f);
            
            Handles.color = new Color(0f, 0f, 1f, 0.5f);
            mainCameraControl.height = Handles.ScaleSlider(mainCameraControl.height,
                targetPosition, Vector3.up, Quaternion.identity,
                mainCameraControl.height, 0.1f);
            mainCameraControl.height = Mathf.Clamp(mainCameraControl.height, 4f, 16f);

            // Label 생성.
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontSize = 15;
            labelStyle.normal.textColor = Color.white;
            labelStyle.alignment = TextAnchor.UpperCenter;

            Handles.Label(targetPosition + (-cameraTarget.forward * mainCameraControl.distance),
                "Distance", labelStyle);

            labelStyle.alignment = TextAnchor.MiddleRight;
            Handles.Label(targetPosition + (Vector3.up * mainCameraControl.height),
                "Height", labelStyle);
        }
    }
}