using UnityEngine;

public class VictoryPanelUI : MonoBehaviour
{
    [SerializeField]
    private UIPanelAnimator animator;

    private void OnEnable()
    {
        BattleEvents.OnVictory += Show;
        BattleEvents.OnDefeat += Hide;
    }

    private void OnDisable()
    {
        BattleEvents.OnVictory -= Show;
        BattleEvents.OnDefeat -= Hide;
    }

    private void Start()
    {
        animator.HideImmediate();
    }

    private void Show()
    {
        animator.Show();
    }

    private void Hide()
    {
        animator.Hide();
    }
}