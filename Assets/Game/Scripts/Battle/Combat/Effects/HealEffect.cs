using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEffect : IActionEffect
{
    public IEnumerator Execute(BattleUnit attacker, List<BattleUnit> targets, BattleActionSO actionData, BattleExecutionContext execution)
    {
        yield return new WaitForSeconds(0.3f);

        foreach (var target in targets)
        {
            if (target == null || target.IsDead) continue;

            target.Heal(actionData.HealAmount);
        }

        yield return new WaitForSeconds(0.8f);
    }
}
