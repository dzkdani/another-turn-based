using UnityEngine;

public class BattlePresentationContext
{
    public BattleVFXManager VFX { get; }
    public BattleAudioManager Audio { get; }
    public PostProcessController PostProcess { get; }
    public ScreenDistortionController Distortion { get; }
    public BattleCameraManager BattleCamera { get; }

    // public BattleTimelineManager Timeline { get; }

    // public CameraFXManager CameraFX { get; }

    // public DamagePopupManager DamagePopup { get; }

    public BattlePresentationContext(
        BattleVFXManager vfx,
        BattleAudioManager audio,
        PostProcessController postProcess,
        ScreenDistortionController distortion,
        BattleCameraManager battleCamera)
    {
        VFX = vfx;
        Audio = audio;
        PostProcess = postProcess;
        Distortion = distortion;
        BattleCamera = battleCamera;
    }
}