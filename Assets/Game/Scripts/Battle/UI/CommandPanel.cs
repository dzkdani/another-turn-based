using UnityEngine;

public class CommandPanel : MonoBehaviour
{
    [SerializeField]
    private UIPanelAnimator animator;

    private void OnEnable()
    {
        BattleEvents.OnPlayerTurn += Show;
        BattleEvents.OnEnemyTurn += Hide;
        BattleEvents.OnAttackPressed += Hide;
        BattleEvents.OnVictory += Hide;
        BattleEvents.OnDefeat += Hide;
    }

    private void OnDisable()
    {
        BattleEvents.OnPlayerTurn -= Show;
        BattleEvents.OnEnemyTurn -= Hide;
        BattleEvents.OnAttackPressed -= Hide;
        BattleEvents.OnVictory -= Hide;
        BattleEvents.OnDefeat -= Hide;
    }

    private void Show()
    {
        animator.Show();
    }

    private void Hide()
    {
        animator.Hide();
    }

    public void Attack()
    {
        BattleEvents.OnAttackPressed?.Invoke();
    }

    public void Run()
    {
        BattleEvents.OnRunPressed?.Invoke();
    }
}