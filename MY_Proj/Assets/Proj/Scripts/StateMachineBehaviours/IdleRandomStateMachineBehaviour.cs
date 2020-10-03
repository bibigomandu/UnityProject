using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleRandomStateMachineBehaviour : StateMachineBehaviour {
    public int numberOfStates = 2;
    public float minNormTime = 0f;
    public float maxNormTime = 5f;

    public float randomNormTime;

    readonly int hashRandomIdle = Animator.StringToHash("RandomIdle");

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
       // 상태(Ninja Idle)에 진입했을 때 실행.
       randomNormTime = Random.Range(minNormTime, maxNormTime);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // Base Layer에 있거나 현재 애니메이션 상태 변수가 stateInfo와 다르면 hashRandomIdle를 -1로 설정.
        if(animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).fullPathHash == stateInfo.fullPathHash)
            animator.SetInteger(hashRandomIdle, -1);

        // 현재 상태가 된지 randomNormTime를 지났으면 상태 변환.
        if(stateInfo.normalizedTime > randomNormTime && !animator.IsInTransition(0))
            animator.SetInteger(hashRandomIdle, Random.Range(0, numberOfStates));
    }
}
