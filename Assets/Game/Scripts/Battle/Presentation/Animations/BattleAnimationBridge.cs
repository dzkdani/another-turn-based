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
        animator ??= GetComponentInChildren<Animator>();

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

    public void PlayHit()
    {
        PrepareAnimation();
        animator.SetTrigger(hitHash);
    }

    public void PlayDeath()
    {
        PrepareAnimation();
        animator.SetTrigger(deathHash);
    }

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