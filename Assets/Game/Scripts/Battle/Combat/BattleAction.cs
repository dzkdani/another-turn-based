using System.Collections;
using System.Collections.Generic;

public abstract class BattleAction
{
    public abstract IEnumerator Execute
    (
        BattleUnit attacker,
        List<BattleUnit> targets,
        BattleActionSO actionData,
        BattlePresentationContext presentation
    );
}