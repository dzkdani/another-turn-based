using UnityEngine;

public class BattleVFXManager : MonoBehaviour
{
    [Header("Optional Parent")]
    [SerializeField] private Transform worldVFXRoot;

    /// <summary>
    /// Spawn a VFX prefab at a world position.
    /// </summary>
    public GameObject PlayEffect(GameObject vfxPrefab, Vector3 position)
    {
        if (vfxPrefab == null)
            return null;

        Transform parent = worldVFXRoot != null ? worldVFXRoot : null;

        return Instantiate(
            vfxPrefab,
            position,
            Quaternion.identity,
            parent);
    }

    /// <summary>
    /// Spawn a VFX prefab attached to a transform.
    /// Useful for cast effects, buffs, auras, etc.
    /// </summary>
    public GameObject PlayAttached(GameObject vfxPrefab, Transform parent)
    {
        if (vfxPrefab == null || parent == null)
            return null;

        return Instantiate(
            vfxPrefab,
            parent.position,
            parent.rotation,
            parent);
    }

    /// <summary>
    /// Spawn a hit effect using the target's hit point.
    /// </summary>
    public GameObject PlayHit(BattleUnit target, GameObject vfxPrefab)
    {
        if (target == null)
            return null;

        BattleUnitVisual visual = target.Visual;

        if (visual == null)
            return null;

        return PlayEffect(
            vfxPrefab,
            visual.HitPoint.position);
    }

    /// <summary>
    /// Spawn a cast effect using the caster's cast point.
    /// </summary>
    public GameObject PlayCast(BattleUnit caster, GameObject vfxPrefab)
    {
        if (caster == null)
            return null;

        BattleUnitVisual visual = caster.Visual;

        if (visual == null)
            return null;

        return PlayAttached(
            vfxPrefab,
            visual.CastPoint);
    }

    /// <summary>
    /// Spawn an aura or looping effect attached to the VFX root.
    /// </summary>
    public GameObject PlayAura(BattleUnit target, GameObject vfxPrefab)
    {
        if (target == null)
            return null;

        BattleUnitVisual visual = target.Visual;

        if (visual == null)
            return null;

        return PlayAttached(
            vfxPrefab,
            visual.VFXRoot);
    }

    /// <summary>
    /// Destroy a spawned VFX instance.
    /// This will later be replaced by object pooling.
    /// </summary>
    public void Release(GameObject spawnedEffect)
    {
        if (spawnedEffect == null)
            return;

        Destroy(spawnedEffect);
    }
}