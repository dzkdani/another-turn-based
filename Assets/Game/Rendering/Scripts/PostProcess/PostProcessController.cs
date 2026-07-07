using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using NaughtyAttributes;

public class PostProcessController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Volume globalVolume;

    [Header("General")]
    [SerializeField] private float intenseTransitionDuration = 0.35f;

    [Header("Intensity Effect")]
    [SerializeField, Range(0f,1f)]
    private float effectThreshold = 0.3f;
    [SerializeField]
    private float maxDesaturation = -80f;
    [SerializeField]
    private float maxVignette = 0.45f;
    [SerializeField]
    private Color effectColor = new Color(0.85f, 0.45f, 0.45f)
    ;
    [Header("Low HP Chromatic Aberration")]
    [SerializeField, Range(0f, 1f)]
    private float maxChromaticAberration = 0.15f;

    [Header("Attack")]
    [SerializeField]
    private float bloomBoost = 0.8f;
    [SerializeField]
    private float attackPulseDuration = 0.15f;

    [Header("Damage")]
    [SerializeField]
    private float flashExposure = 1f;
    [SerializeField]
    private float damageFlashDuration = 0.12f;

    [Header("Chromic Aberration")]
    private ChromaticAberration chromaticAberration;


    private Bloom bloom;
    private ColorAdjustments colorAdjustments;
    private Vignette vignette;

    private float baseBloom;
    private float baseExposure;
    private float baseSaturation;
    private float baseVignette;
    private Color baseColorFilter;
    private float baseChromaticAberration;

    private float currentBloom;
    private float currentExposure;
    private float currentSaturation;
    private float currentVignette;    
    private Color currentColorFilter;
    private float currentChromaticAberration;


    private Tween bloomTween;
    private Tween exposureTween;
    private Tween saturationTween;
    private Tween vignetteTween;
    private Tween colorFilterTween;
    private Tween chromaticTween;

#if UNITY_EDITOR

    [Button("Test Bloom Pulse")]
    private void DebugBloom()
    {
        PlayBloomPulse();
    }

    [Button("Test Damage Flash")]
    private void DebugFlash()
    {
        PlayDamageFlash();
    }

    [Button("Low HP 25%")]
    private void DebugLowHP()
    {
        SetEffectIntensity(0.05f);
    }

#endif

    private void Awake()
    {
        if (globalVolume == null)
        {
            Debug.LogError("PostProcessController : Global Volume not assigned.");
            enabled = false;
            return;
        }

        if (!globalVolume.profile.TryGet(out bloom))
            Debug.LogError("Bloom override missing.");

        if (!globalVolume.profile.TryGet(out colorAdjustments))
            Debug.LogError("Color Adjustments override missing.");

        if (!globalVolume.profile.TryGet(out chromaticAberration))
            Debug.LogError("Chromatic Aberration override missing.");

        if (!globalVolume.profile.TryGet(out vignette))
            Debug.LogError("Vignette override missing.");

        baseBloom = bloom.intensity.value;
        baseExposure = colorAdjustments.postExposure.value;
        baseSaturation = colorAdjustments.saturation.value;
        baseVignette = vignette.intensity.value;
        baseColorFilter = colorAdjustments.colorFilter.value;
        baseChromaticAberration = chromaticAberration.intensity.value;

        currentBloom = baseBloom;
        currentExposure = baseExposure;
        currentSaturation = baseSaturation;
        currentVignette = baseVignette;
        currentColorFilter = baseColorFilter;
        currentChromaticAberration = baseChromaticAberration;

        ApplyVolume();
    }

    private void OnDisable()
    {
        bloomTween?.Kill();
        exposureTween?.Kill();
        saturationTween?.Kill();
        vignetteTween?.Kill();
        colorFilterTween?.Kill();
        chromaticTween?.Kill();
    }


    public void PlayBloomPulse()
    {
        //-----------------------------------
        // Bloom Pulse
        //-----------------------------------

        bloomTween?.Kill();

        currentBloom = baseBloom;

        Sequence bloomSequence = DOTween.Sequence();

        bloomSequence.Append(
            DOTween.To(
                () => currentBloom,
                x =>
                {
                    currentBloom = x;
                    ApplyBloom();
                },
                baseBloom + bloomBoost,
                attackPulseDuration * 0.5f));

        bloomSequence.Append(
            DOTween.To(
                () => currentBloom,
                x =>
                {
                    currentBloom = x;
                    ApplyBloom();
                },
                baseBloom,
                attackPulseDuration * 0.5f));

        bloomTween = bloomSequence;
    }

    public void PlayDamageFlash()
    {
        //-----------------------------------
        // Damage Flash
        //-----------------------------------

        exposureTween?.Kill();

        currentExposure = baseExposure;

        Sequence exposureSequence = DOTween.Sequence();

        exposureSequence.Append(
            DOTween.To(
                () => currentExposure,
                x =>
                {
                    currentExposure = x;
                    ApplyExposure();
                },
                baseExposure + flashExposure,
                damageFlashDuration * 0.5f));

        exposureSequence.Append(
            DOTween.To(
                () => currentExposure,
                x =>
                {
                    currentExposure = x;
                    ApplyExposure();
                },
                baseExposure,
                damageFlashDuration * 0.5f));

        exposureTween = exposureSequence;
    }

    public void SetEffectIntensity(float factor)
    {
        float intensity =
            Mathf.Clamp01(
                (effectThreshold - factor) /
                effectThreshold);

        Color targetColor =
            Color.Lerp(
                baseColorFilter,
                effectColor,
                intensity);

        float targetChromatic =
            Mathf.Lerp(
                baseChromaticAberration,
                maxChromaticAberration,
                intensity);

        float targetSaturation =
            Mathf.Lerp(
                baseSaturation,
                maxDesaturation,
                intensity);

        float targetVignette =
            Mathf.Lerp(
                baseVignette,
                maxVignette,
                intensity);

        saturationTween?.Kill();

        saturationTween =
            DOTween.To(
                () => currentSaturation,
                x =>
                {
                    currentSaturation = x;
                    ApplySaturation();
                },
                targetSaturation,
                intenseTransitionDuration);

        colorFilterTween?.Kill();

        colorFilterTween =
            DOTween.To(
                () => currentColorFilter,
                x =>
                {
                    currentColorFilter = x;
                    ApplyColorFilter();
                },
                targetColor,
                intenseTransitionDuration);

        vignetteTween?.Kill();

        vignetteTween =
            DOTween.To(
                () => currentVignette,
                x =>
                {
                    currentVignette = x;
                    ApplyVignette();
                },
                targetVignette,
                intenseTransitionDuration);

             chromaticTween?.Kill();

        chromaticTween =
            DOTween.To(
                () => currentChromaticAberration,
                x =>
                {
                    currentChromaticAberration = x;
                    ApplyChromaticAberration();
                },
                targetChromatic,
                intenseTransitionDuration);
    }

    private void ApplyVolume()
    {
        ApplyBloom();
        ApplyExposure();
        ApplySaturation();
        ApplyColorFilter();
        ApplyVignette();
        ApplyChromaticAberration();
    }

    private void ApplyBloom()
    {
        bloom.intensity.value = currentBloom;
    }

    private void ApplyExposure()
    {
        colorAdjustments.postExposure.value = currentExposure;
    }

    private void ApplySaturation()
    {
        colorAdjustments.saturation.value = currentSaturation;
    }

    private void ApplyColorFilter()
    {
        colorAdjustments.colorFilter.value = currentColorFilter;
    }

    private void ApplyVignette()
    {
        vignette.intensity.value = currentVignette;
    }

    private void ApplyChromaticAberration()
    {
        chromaticAberration.intensity.value = currentChromaticAberration;
    }
}