using System;
using System.Collections;
using System.Collections.Generic;

public class CounterEffect : IActionEffect
{
    public IEnumerator Execute(
        BattleUnit attacker,
        List<BattleUnit> targets,
        BattleActionSO actionData,
        BattleExecutionContext execution)
    {
        Action<BattleUnit, BattleUnit, float> counterLogic = null;

        counterLogic = (source, target, damage) =>
        {
            if (attacker == null ||
                attacker.IsDead ||
                target == null ||
                target == attacker)
            {
                attacker.OnTakeDamage -= counterLogic;
                return;
            }

            int counterDamage =
                (int)DamageSystem.CalculateDamage(
                    attacker,
                    target,
                    actionData.DamageMultiplier);

            target.TakeDamage(attacker, counterDamage);

            attacker.OnTakeDamage -= counterLogic;
        };

        attacker.OnTakeDamage += counterLogic;

        yield break;
    }
}