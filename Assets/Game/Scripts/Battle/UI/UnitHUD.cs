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

    }

    public void Setup(BattleUnit target)
    {
        unit = target;
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
        BattleEvents.OnUnitDamaged += OnDamaged;
        BattleEvents.OnTargetSelected += OnTargetSelected;
    }

    void OnDisable()
    {
        button.onClick.RemoveAllListeners();
        BattleEvents.OnUnitDamaged -= OnDamaged;
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