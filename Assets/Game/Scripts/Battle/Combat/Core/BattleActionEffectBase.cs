using System.Collections;
using System.Collections.Generic;

public abstract class BattleActionEffectBase : IActionEffect
{
    protected readonly BattleActionPresenter presenter =
        new BattleActionPresenter();

    public abstract IEnumerator Execute(
        BattleUnit attacker,
        List<BattleUnit> targets,
        BattleActionSO actionData,
        BattleExecutionContext execution);

    protected virtual void FaceTarget(
        BattleUnit attacker,
        BattleUnit target)
    {
        if (attacker?.Visual != null &&
            target != null)
        {
            attacker.Visual.FaceTarget(target.transform);
        }
    }

    protected virtual IEnumerator PlayActionAnimation(
        BattleUnit attacker)
    {
        if (attacker.AnimationBridge != null)
            yield return attacker.AnimationBridge.PlayAttackUntilHitFrame();
    }

    protected virtual IEnumerator WaitForActionFinish(
        BattleUnit attacker)
    {
        if (attacker.AnimationBridge != null)
            yield return attacker.AnimationBridge.WaitForAnimationFinished();
    }

    protected virtual void BeginPresentation(
        BattleUnit attacker,
        BattlePresentationContext presentation)
    {
        presenter.BeginAction(attacker, presentation);
    }

    protected virtual void FinishPresentation(
        BattlePresentationContext presentation)
    {
        presenter.FinishAction(presentation);
    }
}