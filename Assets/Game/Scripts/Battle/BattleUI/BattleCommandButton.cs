using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleCommandButton : MonoBehaviour
{
    public event Action<BattleActionSO> OnPressed;

    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Image icon;

    private BattleActionSO action;

    private void Awake()
    {
        button.onClick.AddListener(Click);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(Click);
    }

    public void Setup(BattleActionSO battleAction)
    {
        action = battleAction;

        label.text = action.ActionName;

        if (icon != null)
            icon.sprite = action.Icon;
    }

    private void Click()
    {
        OnPressed?.Invoke(action);
    }
}