using System.Collections;
using UnityEngine;

public class BattleUnitVisual : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    public Animator Animator => animator;

    [Header("Model")]
    [SerializeField] private Transform modelRoot;

    [Header("VFX Points")]
    [SerializeField] private Transform hitPoint;
    [SerializeField] private Transform castPoint;
    [SerializeField] private Transform vfxRoot;

    [Header("Hit Shake")]
    [SerializeField] private float shakeDuration = 0.12f;
    [SerializeField] private float shakeStrength = 0.08f;

    private Coroutine shakeRoutine;
    private Vector3 originalLocalPosition;

    public Transform ModelRoot => modelRoot;
    public Transform HitPoint => hitPoint;
    public Transform CastPoint => castPoint;
    public Transform VFXRoot => vfxRoot;

    private void Awake()
    {
        if (modelRoot == null)
            modelRoot = transform;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        originalLocalPosition = modelRoot.localPosition;
    }

    #region Facing

    public void FaceTarget(Transform target)
    {
        if (target == null)
            return;

        FacePosition(target.position);
    }

    public void FacePosition(Vector3 worldPosition)
    {
        Vector3 direction = worldPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        transform.rotation = Quaternion.LookRotation(direction);
    }

    #endregion

    #region Shake

    public void PlayHitShake()
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        originalLocalPosition = modelRoot.localPosition;

        float timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;

            Vector3 offset = Random.insideUnitSphere * shakeStrength;
            offset.y *= 0.4f;

            modelRoot.localPosition = originalLocalPosition + offset;

            yield return null;
        }

        modelRoot.localPosition = originalLocalPosition;
        shakeRoutine = null;
    }

    #endregion

    #region Flash

    public void PlayFlash()
    {
        // Placeholder.
        // Next step kita implement MaterialPropertyBlock.
    }

    #endregion
}