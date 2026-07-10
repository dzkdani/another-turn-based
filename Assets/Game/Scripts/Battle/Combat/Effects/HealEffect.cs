using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEffect : BattleActionEffectBase
{
    public override IEnumerator Execute(
        BattleUnit attacker,
        List<BattleUnit> targets,
        BattleActionSO actionData,
        BattleExecutionContext execution)
    {
        BattleUnit target = targets[0];

        FaceTarget(attacker, target);

        BeginPresentation(attacker, execution.Presentation);

        yield return PlayActionAnimation(attacker);

        presenter.PlayCastEffects(
            attacker,
            actionData,
            execution.Presentation);

        foreach (BattleUnit unit in targets)
        {
            if (unit == null || unit.IsDead)
                continue;

            unit.Heal(actionData.HealAmount);

            presenter.PlayHitReaction(target);

            presenter.PlayScreenHitEffects(execution.Presentation);

            float hp =
                (float)unit.Data.CurrentHP /
                unit.Data.MaxHP;

            if (unit.Team == Team.Player)
                execution.Presentation.PostProcess.SetIntensity(hp);
        }

        yield return WaitForActionFinish(attacker);

        FinishPresentation(execution.Presentation);
    }
}