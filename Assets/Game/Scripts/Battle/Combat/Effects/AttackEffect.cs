using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : IActionEffect
{
    protected readonly BattleActionPresenter presenter =
        new BattleActionPresenter();

    public virtual IEnumerator Execute(
        BattleUnit attacker,
        List<BattleUnit> targets,
        BattleActionSO actionData,
        BattleExecutionContext execution)
    {
        if (attacker == null ||
            actionData == null ||
            targets.Count == 0)
        {
            yield break;
        }

        BattleUnit primaryTarget = targets[0];

        attacker.Visual.FaceTarget(primaryTarget.transform);

        presenter.BeginAttack(
            attacker,
            execution.Presentation);

        //
        // Start attack animation
        //

        if (attacker.AnimationBridge != null)
        {
            yield return attacker.AnimationBridge.PlayAttackUntilHitFrame();
        }

        //
        // Hit Frame
        //

        presenter.PlayCastEffects(
            attacker,
            actionData,
            execution.Presentation);

        foreach (BattleUnit target in targets)
        {
            if (target == null || target.IsDead)
                continue;

            presenter.PlayHitEffects(
                attacker,
                target,
                actionData,
                execution.Presentation);

            int damage = (int)DamageSystem.CalculateDamage(
                attacker,
                target,
                actionData.DamageMultiplier);

            target.TakeDamage(attacker, damage);

            float hpPercent =
                (float)target.Data.CurrentHP /
                target.Data.MaxHP;

            execution.Presentation.Distortion
                .SetIntensity(hpPercent);

            if (target.Team == Team.Player)
            {
                execution.Presentation.PostProcess
                    .SetIntensity(hpPercent);
            }
        }

        //
        // Finish animation
        //

        if (attacker.AnimationBridge != null)
        {
            yield return attacker.AnimationBridge.WaitForAnimationFinished();
        }

        presenter.FinishAttack(
            execution.Presentation);
    }
}