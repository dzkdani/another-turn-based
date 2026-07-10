using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleActionExecutor
{
    private readonly BattleExecutionContext execution;

    public BattleActionExecutor(BattleExecutionContext execution)
    {
        this.execution = execution;
    }

    public IEnumerator Execute(
        BattleUnit attacker,
        List<BattleUnit> targets,
        BattleActionSO actionData)
    {
        if (attacker == null ||
            actionData == null ||
            targets == null ||
            targets.Count == 0)
        {
            yield break;
        }

        IActionEffect effect =
            ActionEffectFactory.CreateEffect(actionData.ActionType);

        if (effect == null)
        {
            Debug.LogError($"No IActionEffect registered for {actionData.ActionType}");
            yield break;
        }

        yield return effect.Execute(
            attacker,
            targets,
            actionData,
            execution);

        if (!actionData.ModifyTurnOrder)
            yield break;

        foreach (BattleUnit target in targets)
        {
            if (target == null || target.IsDead)
                continue;

            execution.TurnManager.ModifyActionValue(
                target,
                actionData.ActionValueModifier);
        }
    }
}