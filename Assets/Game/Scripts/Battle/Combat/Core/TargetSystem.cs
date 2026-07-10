using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetSystem
{
    private BattleUnit selectedTarget;

    private readonly List<BattleUnit> validTargets = new();

    public bool IsSelecting { get; private set; }

    public void BeginSelection(
        BattleUnit caster,
        BattleActionSO action,
        List<BattleUnit> players,
        List<BattleUnit> enemies)
    {
        validTargets.Clear();

        selectedTarget = null;

        IsSelecting = true;

        validTargets.AddRange(
            GetSelectableTargets(
                caster,
                action.TargetType,
                players,
                enemies));

        BattleEvents.OnTargetSelectionStarted?.Invoke(validTargets);

        if (validTargets.Count > 0)
        {
            selectedTarget = validTargets[0];

            BattleEvents.OnTargetSelected?.Invoke(selectedTarget);
        }
    }

    public List<BattleUnit> GetSelectableTargets(
        BattleUnit caster,
        TargetType type,
        List<BattleUnit> players,
        List<BattleUnit> enemies)
    {
        List<BattleUnit> allUnits = new();

        if (players != null)
            allUnits.AddRange(players);

        if (enemies != null)
            allUnits.AddRange(enemies);

        allUnits = allUnits
            .Where(x => x != null && !x.IsDead)
            .ToList();

        List<BattleUnit> allies = allUnits
            .Where(x => x.Team == caster.Team)
            .ToList();

        List<BattleUnit> enemiesOfCaster = allUnits
            .Where(x => x.Team != caster.Team)
            .ToList();

        switch (type)
        {
            case TargetType.SingleEnemy:
            case TargetType.AllEnemies:
                return enemiesOfCaster;

            case TargetType.SingleAlly:
            case TargetType.AllAllies:
                return allies;

            case TargetType.Self:
                return new List<BattleUnit> { caster };

            case TargetType.Everyone:
                return allUnits;

            default:
                return new List<BattleUnit>();
        }
    }

    public bool CanSelect(BattleUnit target)
    {
        return target != null &&
               validTargets.Contains(target);
    }

    public bool SelectTarget(BattleUnit target)
    {
        if (!CanSelect(target))
            return false;

        selectedTarget = target;

        BattleEvents.OnTargetSelected?.Invoke(target);

        return true;
    }

    public void RemoveTarget(BattleUnit unit)
    {
        validTargets.Remove(unit);

        if (selectedTarget == unit)
            selectedTarget = null;
    }

    public void ClearSelectedTarget()
    {
        selectedTarget = null;

        validTargets.Clear();

        IsSelecting = false;
    }

    public void SetSelectedTarget(BattleUnit target)
    {
        if (!CanSelect(target))
            return;

        selectedTarget = target;
    }

    public List<BattleUnit> GetTargets(
        BattleUnit caster,
        TargetType type,
        List<BattleUnit> players,
        List<BattleUnit> enemies,
        BattleUnit preferredTarget = null)
    {
        List<BattleUnit> allUnits = new();

        if (players != null)
            allUnits.AddRange(players);

        if (enemies != null)
            allUnits.AddRange(enemies);

        allUnits = allUnits
            .Where(x => x != null && !x.IsDead)
            .ToList();

        List<BattleUnit> allies = allUnits
            .Where(x => x.Team == caster.Team)
            .ToList();

        List<BattleUnit> enemiesOfCaster = allUnits
            .Where(x => x.Team != caster.Team)
            .ToList();

        switch (type)
        {
            case TargetType.SingleEnemy:
            case TargetType.SingleAlly:
                return preferredTarget != null && !preferredTarget.IsDead
                    ? new List<BattleUnit> { preferredTarget }
                    : new List<BattleUnit>();

            case TargetType.AllEnemies:
                return enemiesOfCaster;

            case TargetType.AllAllies:
                return allies;

            case TargetType.Self:
                return new List<BattleUnit> { caster };

            case TargetType.Everyone:
                return allUnits;

            default:
                return new List<BattleUnit>();
        }
    }
}