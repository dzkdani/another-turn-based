using System.Collections;
using System.Collections.Generic;

public class InterruptEffect : IActionEffect
{
    public IEnumerator Execute(
        BattleUnit attacker,
        List<BattleUnit> targets,
        BattleActionSO actionData,
        BattleExecutionContext execution)
    {
        foreach (BattleUnit target in targets)
        {
            if (target == null || target.IsDead)
                continue;

            int damage =
                (int)DamageSystem.CalculateDamage(
                    attacker,
                    target,
                    actionData.DamageMultiplier);

            target.TakeDamage(attacker, damage);

            execution.TurnManager.ModifyActionValue(
                target,
                actionData.ActionValueModifier);
        }

        yield break;
    }
}