using System.Collections;
using System.Collections.Generic;

public class HealEffect : IActionEffect
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

            target.Heal(actionData.HealAmount);
        }

        yield break;
    }
}