using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proj.CharacterControls
{
    public class RigidbodyCharacterControl : MonoBehaviour
    {
        // Character의 Rigidbody.
        private Rigidbody characterRigidbody;
        // Character의 transform.
        private Transform characterTransform;
        // 방향 입력을 받는 Vector3.
        private Vector3 inputVector3;

        // 이동 속도 / 대쉬 거리 / 점프력.
        public float moveSpeed = 1;
        public float dashDist = 3;
        public float jumpPower = 5f;

        private float jumpCheckDist = 1.1f;
        public LayerMask groundLayerMask;

        void Start()
        {
            // Rigidbody를 이용해 제어하기 위해 Character의 Rigidbody를 받아온다.
            characterRigidbody = this.gameObject.GetComponent<Rigidbody>();
            // 'this.gameObject.' 은 생략가능.
            // characterRigidbody = GetComponent<Rigidbody>();

            // transform으로 바로 사용해도 됨.
            characterTransform = this.gameObject.GetComponent<Transform>();

        }

        void Update()
        {
            // 전후좌우.
            inputVector3 = Vector3.zero;
            inputVector3.x = Input.GetAxis("Horizontal");
            inputVector3.z = Input.GetAxis("Vertical");
            // 방향 입력이 있으면 그 방향을 바라보도록 한다.
            if(inputVector3 != Vector3.zero)    characterTransform.forward = inputVector3;
            // transform으로 바로 사용하는 경우.
            // if(inputVector3 != Vector3.zero)    transform.forward = inputVector3;

            // 점프.
            if(Input.GetButtonDown("Jump") && CheckCanJump())
            {
                characterRigidbody.AddForce(Vector3.up * jumpPower * -0.1f * Physics.gravity.y, ForceMode.VelocityChange);
            }

            // 대쉬.
            if(Input.GetButtonDown("Dash"))
            {
                characterRigidbody.AddForce(characterTransform.forward * dashDist, ForceMode.VelocityChange);
            }

            // 이동 처리.
            characterRigidbody.MovePosition(characterRigidbody.position + inputVector3 * moveSpeed * Time.deltaTime);
        }

        bool CheckCanJump()
        {
            RaycastHit jumpHit;

            // 캐릭터의 position(이번 경우는 중심)에서 아래로 쏴서 Ground 레이어에 닿는지 확인.
            if(Physics.Raycast(characterTransform.position, Vector3.down, out jumpHit, jumpCheckDist, groundLayerMask))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}