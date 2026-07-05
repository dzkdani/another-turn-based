using System.Collections.Generic;
using System.Linq;

public class BattleAISystem
{
    public BattleUnit ChooseTarget(
        BattleUnit enemy,
        List<BattleUnit> players,
        List<BattleUnit> enemies)
    {
        if (enemy == null)
            return null;

        return enemy.AIBehavior?.SelectTarget(enemy, players, enemies)
            ?? players.FirstOrDefault(unit => unit != null && !unit.IsDead);
    }
}
