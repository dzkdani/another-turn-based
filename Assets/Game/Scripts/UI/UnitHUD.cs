using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

public class UnitHUD : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text hpText;
    [SerializeField] Slider hpBar;
    [SerializeField] private Image selectionImage;
    private Button button;
    private BattleUnit unit;

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

    void OnEnable()
    {
        // BattleEvents.OnUnitDamaged += OnDamaged;
        BattleEvents.OnTargetSelected += OnTargetSelected;
    }

    void OnDisable()
    {
        button.onClick.RemoveAllListeners();
        // BattleEvents.OnUnitDamaged -= OnDamaged;
        BattleEvents.OnTargetSelected -= OnTargetSelected;
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

    private void OnTargetSelected(BattleUnit selectedUnit)
    {
        SetSelected(selectedUnit == unit);
    }

    private void SelectThisUnit()
    {
        if (unit != null)
        {
            BattleEvents.OnTargetSelected?.Invoke(unit);
        }
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