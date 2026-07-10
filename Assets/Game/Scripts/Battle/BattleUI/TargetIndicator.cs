using UnityEngine;
using System.Collections.Generic;

public class TargetIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Vector3 offset = new(0f, 2f, 0f);

    private BattleUnit currentTarget;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        BattleEvents.OnTargetSelectionStarted += HandleSelectionStarted;
        BattleEvents.OnTargetSelectionCancelled += HandleSelectionCancelled;
        BattleEvents.OnTargetSelected += HandleTargetSelected;
    }

    private void OnDisable()
    {
        BattleEvents.OnTargetSelectionStarted -= HandleSelectionStarted;
        BattleEvents.OnTargetSelectionCancelled -= HandleSelectionCancelled;
        BattleEvents.OnTargetSelected -= HandleTargetSelected;
    }

    private void LateUpdate()
    {
        if (currentTarget == null)
            return;

        transform.position =
            currentTarget.transform.position + offset;

        if (mainCamera != null)
        {
            transform.forward = mainCamera.transform.forward;
        }
    }

    private void HandleSelectionStarted(IReadOnlyList<BattleUnit> targets)
    {
        if (targets == null || targets.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        HandleTargetSelected(targets[0]);
    }
    private void HandleSelectionCancelled()
    {
        currentTarget = null;
        gameObject.SetActive(false);
    }

    private void HandleTargetSelected(BattleUnit target)
    {
        currentTarget = target;

        if (currentTarget == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
    }
}