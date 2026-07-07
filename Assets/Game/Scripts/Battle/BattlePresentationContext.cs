using UnityEngine;

public class BattlePresentationContext
{
    public BattleVFXManager VFX { get; }
    public BattleAudioManager Audio { get; }
    public PostProcessController PostProcess { get; }

    // public BattleTimelineManager Timeline { get; }

    // public CameraFXManager CameraFX { get; }

    // public DamagePopupManager DamagePopup { get; }

    public BattlePresentationContext(
        BattleVFXManager vfx,
        BattleAudioManager audio,
        PostProcessController postProcess)
    {
        VFX = vfx;
        Audio = audio;
        PostProcess = postProcess;
    }
}