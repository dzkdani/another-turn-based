using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AIBehavior", menuName = "ScriptableObjects/AI/AIBehavior", order = 1)]
public class AIBehaviorSO : ScriptableObject
{
    public string BehaviorName = "Default";

    public AIBehaviorType BehaviorType = AIBehaviorType.AttackFirstPlayer;

    // Placeholder for future configuration (weights, priorities, ranges, etc.)
    public BattleUnit SelectTarget(BattleUnit caster, List<BattleUnit> players, List<BattleUnit> enemies)
    {
        if (players == null || players.Count == 0)
            return null;

        switch (BehaviorType)
        {
            case AIBehaviorType.AttackFirstPlayer:
                return players.FirstOrDefault(unit => unit != null && !unit.IsDead);

            case AIBehaviorType.RandomTarget:
            {
                var alive = players.Where(u => u != null && !u.IsDead).ToList();
                return alive.Count > 0 ? alive[Random.Range(0, alive.Count)] : null;
            }

            case AIBehaviorType.LowestHP:
                return players.Where(u => u != null && !u.IsDead).OrderBy(u => u.Data.CurrentHP).FirstOrDefault();

            default:
                return players.FirstOrDefault(unit => unit != null && !unit.IsDead);
        }
    }
}

public enum AIBehaviorType
{
    AttackFirstPlayer,
    RandomTarget,
    LowestHP,
    Custom
}
