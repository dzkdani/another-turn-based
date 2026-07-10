using DG.Tweening;
using UnityEngine;
using NaughtyAttributes;

public class ScreenDistortionController : MonoBehaviour
{

#if UNITY_EDITOR
    [Button]
    private void TestPulse()
    {
        PlayDistortionPulse();
    }
#endif

    [Header("References")]
    [SerializeField] private Material distortionMaterial;

    [Header("Pulse")]
    [SerializeField]
    private float distortionStrength = 0.35f;

    [SerializeField]
    private float pulseDuration = 0.15f;

    private static readonly int IntensityID = Shader.PropertyToID("_Intensity");

    private float currentIntensity;

    private Tween distortionTween;

    private void Awake()
    {
        if (distortionMaterial == null)
        {
            Debug.LogError("ScreenDistortionController : Distortion Material not assigned.");
            enabled = false;
            return;
        }

        SetIntensity(0f);
    }

    private void OnDisable()
    {
        distortionTween?.Kill();
    }

    public void PlayDistortionPulse()
    {
        distortionTween?.Kill();

        SetIntensity(0f);  

        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            DOTween.To(
                () => currentIntensity,
                x =>
                {
                    currentIntensity = x;
                    ApplyIntensity();
                },
                distortionStrength,
                pulseDuration * 0.5f));

        sequence.Append(
            DOTween.To(
                () => currentIntensity,
                x =>
                {
                    currentIntensity = x;
                    ApplyIntensity();
                },
                0f,
                pulseDuration * 0.5f));

        distortionTween = sequence;
    }

    public void SetIntensity(float value)
    {
        currentIntensity = Mathf.Clamp01(value);
        ApplyIntensity();
    }

    private void ApplyIntensity()
    {
        distortionMaterial.SetFloat(IntensityID, currentIntensity);
    }
}