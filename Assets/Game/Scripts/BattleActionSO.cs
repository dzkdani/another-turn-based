using UnityEngine;
using UnityEngine.Timeline;

[CreateAssetMenu(
    fileName = "New Battle Action",
    menuName = "Battle/Action",
    order = 1)]
public class BattleActionSO : ScriptableObject
{
    [Header("General")]
    public string ActionName;

    [TextArea(2, 5)]
    public string Description;

    public Sprite Icon;

    [Space]

    [Header("Gameplay")]
    public BattleActionType ActionType;

    public TargetType TargetType;

    public TargetRequirement TargetRequirement;

    public BattleActionFX FX;

    [Min(0f)]
    public float DamageMultiplier = 1f;

    [Min(0)]
    public int HealAmount = 0;

    [Space]

    [Header("Presentation")]

    [Tooltip("Timeline yang dimainkan saat skill digunakan.")]
    public TimelineAsset Timeline;

    [Tooltip("VFX yang muncul di caster (contoh: charge, cast circle).")]
    public GameObject CastVFXPrefab;

    [Tooltip("VFX yang muncul di target (contoh: hit slash, explosion).")]
    public GameObject HitVFXPrefab;

    [Tooltip("SFX utama skill.")]
    public AudioClip SFX;
}

[System.Serializable]
public class BattleActionFX
{
    public TimelineAsset Timeline;
    public GameObject CastVFXPrefab;
    public GameObject HitVFXPrefab;
    public AudioClip SFX;
}