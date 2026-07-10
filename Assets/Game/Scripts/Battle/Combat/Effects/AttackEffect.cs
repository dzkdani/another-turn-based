using System.Collections;
using System.Collections.Generic;

public class AttackEffect : BattleActionEffectBase
{
    public override IEnumerator Execute(
        BattleUnit attacker,
        List<BattleUnit> targets,
        BattleActionSO actionData,
        BattleExecutionContext execution)
    {
        BattleUnit primary = targets[0];

        FaceTarget(attacker, primary);

        BeginPresentation(attacker, execution.Presentation);

        yield return PlayActionAnimation(attacker);

        presenter.PlayCastEffects(
            attacker,
            actionData,
            execution.Presentation);

        foreach (BattleUnit target in targets)
        {
            if (target == null || target.IsDead)
                continue;

            int damage = (int)DamageSystem.CalculateDamage(
                attacker,
                target,
                actionData.DamageMultiplier);

            target.TakeDamage(attacker, damage);

            // Trigger enemy hit animation immediately
            presenter.PlayHitReaction(target);

            // Wait exactly one frame so Animator enters Hit state
            yield return null;

            // Now fire all impact effects together
            presenter.PlayScreenHitEffects(execution.Presentation);
            presenter.PlayHitAudio(
                target,
                actionData,
                execution.Presentation);

            float hp =
                (float)target.Data.CurrentHP /
                target.Data.MaxHP;

            execution.Presentation.Distortion.SetIntensity(hp);

            if (target.Team == Team.Player)
                execution.Presentation.PostProcess.SetIntensity(hp);
        }

        yield return WaitForActionFinish(attacker);

        FinishPresentation(execution.Presentation);
    }
}