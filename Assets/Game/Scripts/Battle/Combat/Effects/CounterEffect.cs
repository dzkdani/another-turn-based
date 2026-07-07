using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CounterEffect : IActionEffect
{
    private readonly BattleActionPresenter presenter = new BattleActionPresenter();

    public IEnumerator Execute(BattleUnit attacker, List<BattleUnit> targets, BattleActionSO actionData, BattleExecutionContext execution)
    {
        if (attacker == null || actionData == null)
        {
            yield break;
        }

        presenter.PlayCastEffects(attacker, actionData, execution.Presentation);
        yield return new WaitForSeconds(0.6f);

        Action<BattleUnit, BattleUnit, float> counterLogic = null;

        counterLogic = (sourceUnit, targetUnit, damageReceived) =>
        {
            if (attacker == null || attacker.IsDead || targetUnit == null || targetUnit == attacker)
            {
                if (attacker != null)
                {
                    attacker.OnTakeDamage -= counterLogic;
                }
                return;
            }

            float counterDamage = DamageSystem.CalculateDamage(attacker, targetUnit, actionData.DamageMultiplier);
            targetUnit.TakeDamage(attacker, counterDamage);
            presenter.PlayHitEffects(attacker, targetUnit, actionData, execution.Presentation);

            attacker.OnTakeDamage -= counterLogic;
        };

        attacker.OnTakeDamage += counterLogic;
    }
}
