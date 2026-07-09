using UnityEngine;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BattleInputController inputController;

    [Header("Panels")]
    [SerializeField] private GameObject commandPanel;

    [Header("Buttons")]
    [SerializeField] private Button attackButton;
    [SerializeField] private Button passButton;

    private BattleUnit currentUnit;

    private void Awake()
    {
        commandPanel.SetActive(false);

        attackButton.onClick.AddListener(OnAttackPressed);
        passButton.onClick.AddListener(OnPassPressed);
    }

    /// <summary>
    /// Opens the command UI for the current player.
    /// </summary>
    public void ShowCommands(BattleUnit actingUnit)
    {
        currentUnit = actingUnit;

        commandPanel.SetActive(true);
    }

    /// <summary>
    /// Hides all command UI.
    /// </summary>
    public void HideCommands()
    {
        commandPanel.SetActive(false);

        currentUnit = null;
    }

    private void OnAttackPressed()
    {
        if (currentUnit == null)
            return;

        BattleActionSO attackAction = currentUnit.Definition.BasicAttack;

        PlayerDecision decision = new PlayerDecision(
            attackAction,
            null
        );

        HideCommands();

        inputController.SubmitDecision(decision);
    }

    private void OnPassPressed()
    {
        if (currentUnit == null)
            return;

        PlayerDecision decision = new PlayerDecision(
            null,
            null
        );

        HideCommands();

        inputController.SubmitDecision(decision);
    }
}