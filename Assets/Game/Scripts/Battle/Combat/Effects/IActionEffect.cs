using System.Collections;
using System.Collections.Generic;

public interface IActionEffect
{
    IEnumerator Execute(
        BattleUnit attacker,
        List<BattleUnit> targets,
        BattleActionSO actionData,
        BattleExecutionContext execution);
}
