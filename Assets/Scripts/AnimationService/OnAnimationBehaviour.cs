using System;
using UnityEngine;

public class OnAnimationBehaviour : StateMachineBehaviour
{

    [SerializeField, Range(0f, 1f)]
    private float triggerNormalizedTime = 0.5f;


    public override void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        if (stateInfo.normalizedTime >= triggerNormalizedTime)
        {
            animator.StopPlayback();
            AnimatingObjectWithBehaviour animatedGO = animator.GetComponent<AnimatingObjectWithBehaviour>();
            if (animatedGO != null)
            {
                animatedGO.EndAnimation.Invoke();
            }
        }
    }

}
