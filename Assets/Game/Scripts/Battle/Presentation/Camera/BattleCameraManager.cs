using UnityEngine;

public class BattleCameraManager : MonoBehaviour
{
    [Header("Rig")]
    [SerializeField] private Transform cameraRig;

    [Header("Anchors")]
    [SerializeField] private Transform defaultAnchor;
    [SerializeField] private Transform playerAnchor;
    [SerializeField] private Transform enemyAnchor;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [SerializeField] private float targetOffsetStrength = 0.35f;

    private Transform currentAnchor;

    private Vector3 currentOffset;

    private void Awake()
    {
        currentAnchor = defaultAnchor;
    }

    private void LateUpdate()
    {
        if (cameraRig == null || currentAnchor == null)
            return;

        Vector3 targetPosition =
            currentAnchor.position + currentOffset;

        cameraRig.position = Vector3.Lerp(
            cameraRig.position,
            targetPosition,
            Time.deltaTime * moveSpeed);

        cameraRig.rotation = Quaternion.Slerp(
            cameraRig.rotation,
            currentAnchor.rotation,
            Time.deltaTime * moveSpeed);
    }

    public void FocusAttacker(BattleUnit attacker)
    {
        if (attacker == null)
            return;

        currentAnchor =
            attacker.Team == Team.Player
            ? playerAnchor
            : enemyAnchor;

        ApplyHorizontalOffset(attacker);
    }

    public void FocusTarget(BattleUnit target)
    {
        if (target == null)
            return;

        currentAnchor =
            target.Team == Team.Player
            ? playerAnchor
            : enemyAnchor;

        ApplyHorizontalOffset(target);
    }

    public void ResetCamera()
    {
        currentAnchor = defaultAnchor;
        currentOffset = Vector3.zero;
    }

    private void ApplyHorizontalOffset(BattleUnit unit)
    {
        currentOffset = Vector3.right *
                        unit.Visual.CameraFocusPoint.position.x 
                        * targetOffsetStrength;

        // currentOffset = Vector3.zero;
    }
}