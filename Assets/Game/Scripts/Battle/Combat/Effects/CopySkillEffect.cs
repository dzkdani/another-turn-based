using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CopySkillEffect : IActionEffect
{
    public IEnumerator Execute(
        BattleUnit attacker,
        List<BattleUnit> targets,
        BattleActionSO actionData,
        BattleExecutionContext execution)
    {
        if (targets.Count == 0)
            yield break;

        BattleUnit target = targets[0];

        if (target == null ||
            target.Actions == null)
            yield break;

        BattleActionSO copiedSkill =
            target.Actions.FirstOrDefault(
                x => x != null &&
                     x.ActionType != BattleActionType.Run &&
                     x.ActionType != BattleActionType.CopySkill);

        if (copiedSkill == null)
            yield break;

        BattleActionExecutor executor =
            new BattleActionExecutor(execution);

        yield return executor.Execute(
            attacker,
            new List<BattleUnit> { target },
            copiedSkill);
    }
}