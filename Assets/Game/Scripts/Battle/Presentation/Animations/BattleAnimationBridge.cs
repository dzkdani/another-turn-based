using System.Collections;
using UnityEngine;

public class BattleAnimationBridge : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Trigger Names")]
    [SerializeField] private string attackTrigger = "Attack";
    [SerializeField] private string hitTrigger = "Hit";
    [SerializeField] private string deathTrigger = "Death";

    private int attackHash;
    private int hitHash;
    private int deathHash;

    private bool hitFrameReached;
    private bool animationFinished;

    public void Initialize()
    {
        animator = animator != null ? animator : GetComponent<Animator>();

        attackHash = Animator.StringToHash(attackTrigger);
        hitHash = Animator.StringToHash(hitTrigger);
        deathHash = Animator.StringToHash(deathTrigger);
    }

    public IEnumerator PlayAttackUntilHitFrame()
    {
        PrepareAnimation();

        animator.SetTrigger(attackHash);

        yield return new WaitUntil(() => hitFrameReached);
    }

    public IEnumerator WaitForAnimationFinished()
    {
        yield return new WaitUntil(() => animationFinished);
    }

    public void PlayAction()
    {
        PrepareAnimation();
        animator.SetTrigger(attackHash);
    }

    public void PlayHit()
    {
        PrepareAnimation();
        animator.SetTrigger(hitHash);
    }

    public IEnumerator PlayDieUntilFinished()
    {
        PrepareAnimation();
        animator.SetTrigger(deathHash);
        yield return new WaitUntil(() => animationFinished);
    }

    // public IEnumerator PlaySkillUntilHitFrame()
    // {
    //     PrepareAnimation();
    //     animator.SetTrigger(skillHash);
    //     yield return new WaitUntil(() => hitFrameReached);
    // }

    // public IEnumerator PlayHealUntilHitFrame()
    // {
    //     PrepareAnimation();
    //     animator.SetTrigger(healHash);
    //     yield return new WaitUntil(() => hitFrameReached);
    // }

    // public IEnumerator PlayCounterUntilHitFrame()
    // {
    //     PrepareAnimation();
    //     animator.SetTrigger(counterHash);
    //     yield return new WaitUntil(() => hitFrameReached);
    // }

    private void PrepareAnimation()
    {
        hitFrameReached = false;
        animationFinished = false;
    }

    // ===== Animation Events =====

    public void AnimationEvent_HitFrame()
    {
        hitFrameReached = true;
    }

    public void AnimationEvent_Finished()
    {
        animationFinished = true;
    }
}