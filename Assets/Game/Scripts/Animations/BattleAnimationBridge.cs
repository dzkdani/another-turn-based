using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(BattleUnit))]
public class BattleAnimationBridge : MonoBehaviour
{
    private Animator animator;
    private BattleUnitVisual visual;

    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int HitHash = Animator.StringToHash("TakeDamage");
    private static readonly int DeathHash = Animator.StringToHash("IsDead");
    private static readonly int TurnHash = Animator.StringToHash("OnTurnStart");

    private void Awake()
    {
        visual = GetComponent<BattleUnitVisual>();
        animator = visual.Animator;

        if (animator == null)
        {
            Debug.LogWarning($"Animator not found on {gameObject.name}");
        }
    }

    [Button]
    public void PlayAttackAnimation()
    {
        if (animator == null)
            return;

        animator.SetTrigger(AttackHash);
    }

    [Button]
    public void PlayHitAnimation()
    {
        if (animator == null)
            return;

        visual?.PlayFlash();
        visual?.PlayHitShake();

        animator.SetTrigger(HitHash);
    }

    [Button]
    public void PlayDeathAnimation()
    {
        if (animator == null)
            return;

        animator.SetBool(DeathHash, true);
    }

    public void SetCurrentTurn(bool active)
    {
        if (animator == null)
            return;

        animator.SetBool(TurnHash, active);
    }
}