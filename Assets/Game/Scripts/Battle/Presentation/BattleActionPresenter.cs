public class BattleActionPresenter
{
    public void BeginAttack(
        BattleUnit attacker,
        BattlePresentationContext presentation)
    {
        presentation.BattleCamera.FocusAttacker(attacker);

        attacker.AnimationBridge?.PlayAction();

        // attacker.AnimationBridge?.PlaySkill();
        // attacker.AnimationBridge?.PlayHeal();
        // attacker.AnimationBridge?.PlayCounter();
    }

    public void PlayCastEffects(
        BattleUnit attacker,
        BattleActionSO action,
        BattlePresentationContext presentation)
    {
        if (action?.FX == null)
            return;

        if (presentation.Audio != null)
        {
            presentation.Audio.Play(
                action.FX.CastSFX,
                attacker.Visual.CastPoint);
        }
    }

    public void PlayHitEffects(
        BattleUnit attacker,
        BattleUnit target,
        BattleActionSO action,
        BattlePresentationContext presentation)
    {
        presentation.BattleCamera.FocusTarget(target);

        if (target.AnimationBridge != null)
            target.AnimationBridge.PlayHit();

        presentation.PostProcess.PlayDamageFlash();
        presentation.PostProcess.PlayBloomPulse();
        presentation.Distortion.PlayDistortionPulse();

        if (presentation.Audio != null &&
            action?.FX != null)
        {
            presentation.Audio.Play(
                action.FX.HitSFX,
                target.Visual.HitPoint);
        }
    }

    public void FinishAttack(
        BattlePresentationContext presentation)
    {
        presentation.BattleCamera.ResetCamera();
    }
}