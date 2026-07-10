using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterEffect : BattleActionEffectBase
{
    public override IEnumerator Execute(
        BattleUnit attacker,
        List<BattleUnit> targets,
        BattleActionSO actionData,
        BattleExecutionContext execution)
    {
        BattleUnit target = targets[0];

        FaceTarget(attacker, target);

        BeginPresentation(attacker, execution.Presentation);

        yield return PlayActionAnimation(attacker);

        presenter.PlayCastEffects(
            attacker,
            actionData,
            execution.Presentation);

        int damage = (int)DamageSystem.CalculateDamage(
            attacker,
            target,
            actionData.DamageMultiplier);

        target.TakeDamage(attacker, damage);

        presenter.PlayHitReaction(target);

        presenter.PlayScreenHitEffects(execution.Presentation);

        yield return WaitForActionFinish(attacker);

        FinishPresentation(execution.Presentation);
    }
}