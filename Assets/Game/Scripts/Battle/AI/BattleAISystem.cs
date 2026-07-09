using System.Collections.Generic;
using UnityEngine;

public class BattleAISystem
{
    public BattleActionSO ChooseAction(BattleUnit unit)
    {
        if (unit == null)
            return null;

        if (unit.AIBehavior == null)
            return unit.BasicAttack;

        return unit.AIBehavior.SelectAction(unit);
    }

    public BattleUnit ChooseTarget(
        BattleUnit unit,
        BattleActionSO action,
        List<BattleUnit> players,
        List<BattleUnit> enemies)
    {
        if (unit == null || action == null)
            return null;

        if (unit.AIBehavior == null)
            return null;

        return unit.AIBehavior.SelectTarget(
            unit,
            action,
            players,
            enemies);
    }
}