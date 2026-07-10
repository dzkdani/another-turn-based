using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class UnitHUD : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text hpText;
    [SerializeField] Slider hpBar;
    [SerializeField] private Image selectionImage;
    public event Action<BattleUnit> OnSelected;
    private Button button;
    private BattleUnit unit;
    public BattleUnit Unit => unit;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
        }

        if (nameText == null) Debug.LogWarning($"{name} is missing nameText reference.");
        if (hpText == null) Debug.LogWarning($"{name} is missing hpText reference.");
        if (hpBar == null) Debug.LogWarning($"{name} is missing hpBar reference.");
        if (selectionImage == null) Debug.LogWarning($"{name} is missing selectionImage reference.");
    }
    
    void OnEnable()
    {
        BattleEvents.OnUnitHPChanged += OnDamaged;
        BattleEvents.OnTargetSelected += OnTargetSelected;
    }

    void OnDisable()
    {
        button.onClick.RemoveAllListeners();
        BattleEvents.OnUnitHPChanged -= OnDamaged;
        BattleEvents.OnTargetSelected -= OnTargetSelected;
    }

    public void Setup(BattleUnit target)
    {
        if (target == null)
        {
            Debug.LogWarning($"{name} received a null BattleUnit target.");
            return;
        }

        unit = target;

        if (nameText == null || hpText == null || hpBar == null)
        {
            Debug.LogWarning($"{name} is missing HUD references for setup.");
            return;
        }

        nameText.text = unit.Data.Name;
        hpBar.maxValue = unit.Data.MaxHP;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(SelectThisUnit);
        }

        Refresh();
    }

    void OnDamaged(BattleUnit damaged)
    {
        if(damaged != unit)
            return;

        Refresh();
    }

    public void SetSelected(bool selected)
    {
        if (selectionImage != null)
        {
            selectionImage.gameObject.SetActive(selected);
        }
    }

    public void SetInteractable(bool value)
    {
        button.interactable = value;
    }

    private void OnTargetSelected(BattleUnit selectedUnit)
    {
        SetSelected(selectedUnit == unit);
    }

    private void SelectThisUnit()
    {
        OnSelected?.Invoke(unit);
    }

    void Refresh()
    {
        if (unit == null)
        {
            Debug.LogWarning($"{name} cannot refresh because unit is null.");
            return;
        }

        if (nameText == null || hpText == null || hpBar == null)
        {
            Debug.LogWarning($"{name} cannot refresh because HUD references are missing.");
            return;
        }

        nameText.text = unit.Data.Name;
        float currentHp = unit.Data.CurrentHP;
        if (currentHp < 0) currentHp = 0;
        hpText.text = $"{currentHp}/{unit.Data.MaxHP}";
        hpBar.DOValue(
            unit.Data.CurrentHP,
            0.3f
        );
    }
}