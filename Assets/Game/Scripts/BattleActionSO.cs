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
}

[System.Serializable]
public class BattleActionFX
{
    public TimelineAsset Timeline;

    public string AnimationTrigger = "Attack";

    public float HitDelay = 0.35f;

    public GameObject CastVFXPrefab;

    public GameObject HitVFXPrefab;

    public AudioClip CastSFX;
    
    public AudioClip HitSFX;
}