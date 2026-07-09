using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterruptEffect : IActionEffect
{
    private readonly BattleActionPresenter presenter = new BattleActionPresenter();

    public IEnumerator Execute(BattleUnit attacker, List<BattleUnit> targets, BattleActionSO actionData, BattleExecutionContext execution)
    {
        if (attacker == null || actionData == null)
        {
            yield break;
        }

        presenter.PlayCastEffects(attacker, actionData, execution.Presentation);
        yield return new WaitForSeconds(0.5f);

        foreach (var target in targets)
        {
            if (target == null || target.IsDead)
                continue;

            int damage = 
                (int)DamageSystem.CalculateDamage(
                    attacker,
                    target,
                    actionData.DamageMultiplier);

            target.TakeDamage(attacker, damage);

            presenter.PlayHitEffects(
                attacker,
                target,
                actionData,
                execution.Presentation);
        }

        yield return new WaitForSeconds(1f);
    }
}
