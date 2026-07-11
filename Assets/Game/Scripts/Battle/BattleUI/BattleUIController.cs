using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleUIController : MonoBehaviour
{
    public event Action<PlayerDecision> OnDecisionMade;

    [Header("HUD")]
    [SerializeField] private UnitHUD playerHUDPrefab;
    [SerializeField] private UnitHUD enemyHUDPrefab;

    [SerializeField] private Transform playerHUDRoot;
    [SerializeField] private Transform enemyHUDRoot;

    [Header("Panels")]
    [SerializeField] private GameObject commandPanel;
    [SerializeField] private BattleResultPanel resultPanel;
    // [SerializeField] private TurnOrderPanel turnOrderPanel;

    [SerializeField] private BattleCommandButton commandButtonPrefab;
    [SerializeField] private Transform commandRoot;
    private readonly List<BattleCommandButton> commandButtons = new();

    private readonly List<UnitHUD> playerHUDs = new();
    private readonly List<UnitHUD> enemyHUDs = new();

    private readonly Dictionary<BattleUnit, UnitHUD> huds = new();

    private BattleUnit currentUnit;
    private BattleActionSO selectedAction;
    private BattleUnit selectedTarget;

    private void Awake()
    {
        commandPanel.SetActive(false);
    }

    private void OnEnable()
    {
        BattleEvents.OnVictory += resultPanel.ShowVictory;
        BattleEvents.OnDefeat += resultPanel.ShowDefeat;
    }

    private void OnDisable()
    {
        BattleEvents.OnVictory -= resultPanel.ShowVictory;
        BattleEvents.OnDefeat -= resultPanel.ShowDefeat;
    }

    private void OnDestroy()
    {
        ClearHUDs();
    }

    #region Initialization

    public void Initialize(List<BattleUnit> players, List<BattleUnit> enemies)
    {
        ClearHUDs();

        CreateHUDs(players, playerHUDPrefab, playerHUDRoot);
        CreateHUDs(enemies, enemyHUDPrefab, enemyHUDRoot);

        DisableTargetSelection();
    }

    private void CreateHUDs(
        List<BattleUnit> units,
        UnitHUD prefab,
        Transform root)
    {
        foreach (BattleUnit unit in units)
        {
            UnitHUD hud = Instantiate(prefab, root);

            hud.Setup(unit);
            hud.SetInteractable(false);
            hud.OnSelected += OnTargetSelected;

            huds.Add(unit, hud);
        }
    }

    public void ClearHUDs()
    {
        foreach (UnitHUD hud in playerHUDs)
        {
            if (hud == null)
                continue;

            hud.OnSelected -= OnTargetSelected;
            Destroy(hud.gameObject);
        }

        foreach (UnitHUD hud in enemyHUDs)
        {
            if (hud == null)
                continue;

            hud.OnSelected -= OnTargetSelected;
            Destroy(hud.gameObject);
        }

        playerHUDs.Clear();
        enemyHUDs.Clear();
    }

    public void RemoveHUD(BattleUnit unit)
    {
        if (huds.TryGetValue(unit, out var hud))
        {
            Destroy(hud.gameObject);
            huds.Remove(unit);
        }
    }

    #endregion

    #region Commands

    public void ShowCommands(BattleUnit actingUnit)
    {
        currentUnit = actingUnit;

        commandPanel.SetActive(true);

        CreateCommandButtons(actingUnit.Definition.Actions);
    }

    private void CreateCommandButtons(List<BattleActionSO> actions)
    {
        if (actions == null || actions.Count == 0)
        {
            Debug.LogWarning($"{currentUnit.Definition.name} has no battle actions assigned.");
            return;
        }

        HideCommandButtons();

        for (int i = 0; i < actions.Count; i++)
        {
            BattleCommandButton button;

            // Reuse
            if (i < commandButtons.Count)
            {
                button = commandButtons[i];
            }
            //CreateNew
            else
            {
                button = Instantiate(
                    commandButtonPrefab,
                    commandRoot);

                button.OnPressed += OnActionSelected;

                commandButtons.Add(button);
            }

            button.Setup(actions[i]);

            button.gameObject.SetActive(true);
        }
    }

    private void OnActionSelected(BattleActionSO action)
    {
        selectedAction = action;

        HideCommands();


        BattleEvents.OnActionSelected?.Invoke(action);

        EnableTargetSelection(action.TargetRequirement);
    }

    public void HideCommands()
    {
        commandPanel.SetActive(false);

        HideCommandButtons();
    }

    private void HideCommandButtons()
    {
        foreach (BattleCommandButton button in commandButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Target Selection

    public void EnableTargetSelection(TargetRequirement requirement)
    {
        DisableTargetSelection();

        switch (requirement)
        {
            case TargetRequirement.Enemy:

                foreach (var pair in huds)
                {
                    if (pair.Key.Team == Team.Enemy)
                        pair.Value.SetInteractable(true);
                };

                break;

            case TargetRequirement.Ally:

                foreach (var pair in huds)
                {
                    if (pair.Key.Team == Team.Player)
                        pair.Value.SetInteractable(true);
                }

                break;

            case TargetRequirement.Any:

                foreach (var pair in huds)
                {
                    pair.Value.SetInteractable(true);
                }

                break;
        }
    }

    public void DisableTargetSelection()
    {
        foreach (var pair in huds)
        {
            pair.Value.SetInteractable(false);
        }
    }

    private void OnTargetSelected(BattleUnit target)
    {
        selectedTarget = target;

        HighlightTarget(target);

        DisableTargetSelection();

        PlayerDecision decision = new PlayerDecision(
            selectedAction,
            selectedTarget);

        OnDecisionMade?.Invoke(decision);
    }

    public void HighlightTarget(BattleUnit target)
    {
        foreach (UnitHUD hud in playerHUDs)
            hud.SetSelected(hud.Unit == target);

        foreach (UnitHUD hud in enemyHUDs)
            hud.SetSelected(hud.Unit == target);
    }

    #endregion
}