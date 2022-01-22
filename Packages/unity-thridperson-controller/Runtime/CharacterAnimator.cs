using System;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public Animator animator;

    public void OnGrounded(bool grounded)
    {
        animator.SetBool("grounded", grounded);
    }

    public void OnMove(float speed)
    {
        animator.SetFloat("speed", speed);
    }

    public void OnAim(bool isAim)
    {
        animator.SetBool("aim", isAim);
    }

    public void OnAttack()
    {
        animator.SetTrigger("attack");
    }


    public float GetMoveSpeed()
    {
        return animator.GetFloat("speed");
    }

    public void OnJump()
    {
        animator.SetTrigger("jump");
    }

    public void OnCrouched(bool crouched)
    {
        animator.SetBool("crouched", crouched);
    }
}