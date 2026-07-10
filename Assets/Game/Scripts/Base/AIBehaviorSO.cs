using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(
    fileName = "newAIBehavior",
    menuName = "Battle/AI Behavior",
    order = 1)]
public class AIBehaviorSO : ScriptableObject
{
    [Header("General")]
    public string BehaviorName = "Default";

    [Header("Target Selection")]
    public AIBehaviorType BehaviorType = AIBehaviorType.AttackFirstPlayer;

    /// <summary>
    /// Decide which action the AI wants to use.
    /// For now AI always prefers Basic Attack.
    /// Later this can be expanded for Heal, Buff, Counter, etc.
    /// </summary>
    public virtual BattleActionSO SelectAction(BattleUnit caster)
    {
        BattleUnit unit = caster;

        if (caster == null)
        {
            Debug.LogWarning("AI can't find unit's actions");
            return null;
        }
        
        return caster.Actions[
                Random.Range(0, caster.Actions.Count)];
    }

    /// <summary>
    /// Decide who will become the target.
    /// Strategy depends on BehaviorType.
    /// </summary>
    public virtual BattleUnit SelectTarget(
        BattleUnit caster,
        BattleActionSO action,
        List<BattleUnit> players,
        List<BattleUnit> enemies)
    {
        if (caster == null || action == null)
            return null;

        List<BattleUnit> candidates;

        switch (action.TargetType)
        {
            case TargetType.SingleEnemy:
                candidates = caster.Team == Team.Player
                    ? enemies
                    : players;
                break;

            case TargetType.SingleAlly:
                candidates = caster.Team == Team.Player
                    ? players
                    : enemies;
                break;

            default:
                return null;
        }

        candidates = candidates
            .Where(x => x != null && !x.IsDead)
            .ToList();

        if (candidates.Count == 0)
            return null;

        switch (BehaviorType)
        {
            case AIBehaviorType.AttackFirstPlayer:
                return candidates.First();

            case AIBehaviorType.RandomTarget:
                return candidates[
                    Random.Range(0, candidates.Count)];

            case AIBehaviorType.LowestHP:
                return candidates
                    .OrderBy(x => x.Data.CurrentHP)
                    .First();

            case AIBehaviorType.Custom:
                return SelectCustomTarget(
                    caster,
                    action,
                    candidates);

            default:
                return candidates.First();
        }
    }

    /// <summary>
    /// Extension point for future boss AI.
    /// </summary>
    protected virtual BattleUnit SelectCustomTarget(
        BattleUnit caster,
        BattleActionSO action,
        List<BattleUnit> candidates)
    {
        return candidates.FirstOrDefault();
    }
}

public enum AIBehaviorType
{
    AttackFirstPlayer,
    RandomTarget,
    LowestHP,
    Custom
}