using System.Collections.Generic;
using System.Collections;

public class ActionSystem
{
    private readonly BattleActionExecutor executor;

    public ActionSystem(BattleExecutionContext execution)
    {
        executor = new BattleActionExecutor(execution);
    }

    public List<BattleUnit> ResolveTargets(BattleUnit caster, BattleActionSO action, List<BattleUnit> players, List<BattleUnit> enemies, BattleUnit preferredTarget, TargetSystem targetSystem)
    {
        return targetSystem.GetTargets(caster, action.TargetType, players, enemies, preferredTarget);
    }

    public IEnumerator ExecuteAction(BattleUnit attacker, List<BattleUnit> targets, BattleActionSO actionData)
    {
        yield return executor.Execute(attacker, targets, actionData);
    }
}