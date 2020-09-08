using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Proj.CharacterControls
{
    public class CharacterControllerCharacterControl : MonoBehaviour
    {
        private CharacterController characterController;
        private NavMeshAgent agent;
        private Camera mainCamera;
        public Animator animator;
        public LayerMask groundLayerMask;

        readonly int moveHash = Animator.StringToHash("Move");
        readonly int fallingHash = Animator.StringToHash("Falling");

        // Start is called before the first frame update
        void Start()
        {
            characterController = GetComponent<CharacterController>();
            agent = GetComponent<NavMeshAgent>();
            
            agent.updatePosition = false;
            agent.updateRotation = true;

            mainCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                // 마우스가 가리키는 곳으로 Ray를 쏴서 위치를 얻는다.
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit, 100, groundLayerMask))
                {
                    agent.SetDestination(hit.point);
                }
            }

            // Animation 제어.
            if(agent.remainingDistance > agent.stoppingDistance)
            {
                characterController.Move(agent.velocity * Time.deltaTime);
                animator.SetBool(moveHash, true);
            }
            else
            {
                characterController.Move(Vector3.zero);
                animator.SetBool(moveHash, false);
            }

            if (agent.isOnOffMeshLink)
            {
                animator.SetBool(fallingHash, agent.velocity.y != 0.0f);
            }
            else
            {
                animator.SetBool(fallingHash, false);
            }
        }

        // private void LateUpdate()
        private void OnAnimatorMove()
        {
            animator.rootPosition = agent.nextPosition;
            transform.position = agent.nextPosition;
        }
    }
}