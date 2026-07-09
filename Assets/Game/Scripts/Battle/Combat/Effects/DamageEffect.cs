using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : IActionEffect
{
    protected readonly BattleActionPresenter presenter = new BattleActionPresenter();

    public virtual IEnumerator Execute(BattleUnit attacker, List<BattleUnit> targets, BattleActionSO actionData, BattleExecutionContext execution)
    {
        if (attacker == null || actionData == null)
        {
            yield break;
        }

        presenter.PlayCastEffects(attacker, actionData, execution.Presentation);
        yield return PlayAttackAnimation(attacker);

        foreach (var target in targets)
        {
            if (target == null || target.IsDead) continue;

            presenter.PlayHitEffects(attacker, target, actionData, execution.Presentation);

            int damage = (int)DamageSystem.CalculateDamage(attacker, target, actionData.DamageMultiplier);
            target.TakeDamage(attacker, damage);

            execution.Presentation.PostProcess.SetEffectIntensity(
                target.Data.CurrentHP /
                target.Data.MaxHP);
        }

        yield return FinishAnimation(attacker);
    }

    protected IEnumerator PlayAttackAnimation(BattleUnit attacker)
    {
        if (attacker?.Visual != null && attacker.Visual.CastPoint != null && attacker.AnimationBridge != null)
        {
            yield return attacker.AnimationBridge.PlayAttackUntilHitFrame();
        }
    }

    protected IEnumerator FinishAnimation(BattleUnit attacker)
    {
        if (attacker?.AnimationBridge != null)
        {
            yield return attacker.AnimationBridge.WaitForAnimationFinished();
        }
    }
}
