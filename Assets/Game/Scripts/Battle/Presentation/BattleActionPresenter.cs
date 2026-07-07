public class BattleActionPresenter
{
    public void PlayCastEffects(BattleUnit attacker, BattleActionSO actionData, BattlePresentationContext presentation)
    {
        presentation.PostProcess.PlayBloomPulse();

        if (presentation.Audio == null || actionData.FX == null)
        {
            return;
        }

        presentation.Audio.Play(actionData.FX.CastSFX, attacker?.Visual?.CastPoint);
    }

    public void PlayHitEffects(BattleUnit attacker, BattleUnit target, BattleActionSO actionData, BattlePresentationContext presentation)
    {
        if (target.Visual == null || target.Visual.HitPoint == null)
        {
            return;
        }

        if (target.AnimationBridge != null)
        {
            target.AnimationBridge.PlayHit();
        }
        presentation.PostProcess.PlayDamageFlash();

        if (presentation.Audio != null && actionData?.FX != null)
        {
            presentation.Audio.Play(actionData.FX.HitSFX, target.Visual.HitPoint);
        }
    }
}
