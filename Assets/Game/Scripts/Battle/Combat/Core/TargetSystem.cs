using System.Collections.Generic;
using System.Linq;

public class TargetSystem
{
    private BattleUnit selectedTarget;

    public void SetSelectedTarget(BattleUnit target)
    {
        selectedTarget = target;
    }

    public void ClearSelectedTarget()
    {
        selectedTarget = null;
    }

    public List<BattleUnit> GetTargets(
        BattleUnit caster,
        TargetType type,
        List<BattleUnit> players,
        List<BattleUnit> enemies,
        BattleUnit preferredTarget = null)
    {
        if (preferredTarget != null && !preferredTarget.IsDead)
        {
            selectedTarget = preferredTarget;
        }

        List<BattleUnit> alivePlayers = players?
            .Where(unit => unit != null && !unit.IsDead)
            .ToList() ?? new List<BattleUnit>();

        List<BattleUnit> aliveEnemies = enemies?
            .Where(unit => unit != null && !unit.IsDead)
            .ToList() ?? new List<BattleUnit>();

        switch (type)
        {
            case TargetType.SingleEnemy:
                if (selectedTarget != null && selectedTarget.Team == Team.Enemy && !selectedTarget.IsDead)
                {
                    return new List<BattleUnit> { selectedTarget };
                }

                return aliveEnemies.Count > 0
                    ? new List<BattleUnit> { aliveEnemies[0] }
                    : new List<BattleUnit>();

            case TargetType.AllEnemies:
                return aliveEnemies;

            case TargetType.SingleAlly:
                if (selectedTarget != null && selectedTarget.Team == caster.Team && !selectedTarget.IsDead)
                {
                    return new List<BattleUnit> { selectedTarget };
                }

                List<BattleUnit> allies = caster.Team == Team.Player ? alivePlayers : aliveEnemies;
                return allies.Count > 0
                    ? new List<BattleUnit> { allies[0] }
                    : new List<BattleUnit>();

            case TargetType.AllAllies:
                return caster.Team == Team.Player ? alivePlayers : aliveEnemies;

            case TargetType.Self:
                return new List<BattleUnit> { caster };

            case TargetType.Everyone:
                List<BattleUnit> allUnits = new List<BattleUnit>();
                allUnits.AddRange(alivePlayers);
                allUnits.AddRange(aliveEnemies);
                return allUnits;

            default:
                return new List<BattleUnit>();
        }
    }
}