using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleActionExecutor
{
    private readonly BattleExecutionContext execution;
    private readonly BattleActionPresenter presenter = new();

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

        presenter.BeginAttack(
            attacker,
            execution.Presentation);

        presenter.PlayCastEffects(
            attacker,
            actionData,
            execution.Presentation);

        //use only attack for now
        yield return attacker.AnimationBridge.PlayAttackUntilHitFrame();
        //later
        // yield return attacker.AnimationBridge.PlayActionUntilHitFrame();

        yield return effect.Execute(
            attacker,
            targets,
            actionData,
            execution);

        foreach (BattleUnit target in targets)
        {
            if (target == null || target.IsDead)
                continue;

            presenter.PlayHitEffects(
                attacker,
                target,
                actionData,
                execution.Presentation);
        }

        presenter.FinishAttack(execution.Presentation);

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