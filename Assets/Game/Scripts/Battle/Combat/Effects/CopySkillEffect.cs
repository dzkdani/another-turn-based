using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CopySkillEffect : IActionEffect
{
    private readonly BattleActionPresenter presenter = new BattleActionPresenter();

    public IEnumerator Execute(BattleUnit attacker, List<BattleUnit> targets, BattleActionSO actionData, BattleExecutionContext execution)
    {
        if (attacker == null || actionData == null || targets == null || targets.Count == 0)
        {
            yield break;
        }

        BattleUnit target = targets[0];
        if (target?.Skills == null) yield break;

        presenter.PlayCastEffects(attacker, actionData, execution.Presentation);
        yield return new WaitForSeconds(0.5f);

        BattleActionSO copiedSkillData = target.Skills
            .FirstOrDefault(s => s != null && s.ActionType != BattleActionType.Run && s.ActionType != BattleActionType.CopySkill);

        if (copiedSkillData != null)
        {
            yield return new WaitForSeconds(0.5f);

            ActionSystem internalSystem = new ActionSystem(execution);
            List<BattleUnit> newTargets = new List<BattleUnit> { target };

            yield return internalSystem.ExecuteAction(attacker, newTargets, copiedSkillData);
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }
    }
}
