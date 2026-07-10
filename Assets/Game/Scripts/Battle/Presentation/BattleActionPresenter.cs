public class BattleActionPresenter
{
    public void BeginAction(
        BattleUnit attacker,
        BattlePresentationContext presentation)
    {
        presentation.BattleCamera.FocusAttacker(attacker);
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

    public void PlayHitReaction(BattleUnit target)
    {
        if (target == null)
            return;

        target.AnimationBridge?.PlayHit();
    }

    public void PlayScreenHitEffects(
        BattlePresentationContext presentation)
    {
        presentation.PostProcess.PlayDamageFlash();
        presentation.PostProcess.PlayBloomPulse();
        presentation.Distortion.PlayDistortionPulse();
    }

    public void PlayHitAudio(
        BattleUnit target,
        BattleActionSO action,
        BattlePresentationContext presentation)
    {
        if (presentation.Audio == null)
            return;

        if (action?.FX == null)
            return;

        presentation.Audio.Play(
            action.FX.HitSFX,
            target.Visual.HitPoint);
    }

    public void FinishAction(
        BattlePresentationContext presentation)
    {
        presentation.BattleCamera.ResetCamera();
    }
}