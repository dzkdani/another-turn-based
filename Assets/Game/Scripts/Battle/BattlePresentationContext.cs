using UnityEngine;

public class BattlePresentationContext
{
    public BattleVFXManager VFX { get; }
    public BattleAudioManager Audio { get; }

    // public BattleTimelineManager Timeline { get; }

    // public CameraFXManager CameraFX { get; }

    // public DamagePopupManager DamagePopup { get; }

    public BattlePresentationContext(
        BattleVFXManager vfx,
        BattleAudioManager audio)
    {
        VFX = vfx;
        Audio = audio;
    }
}