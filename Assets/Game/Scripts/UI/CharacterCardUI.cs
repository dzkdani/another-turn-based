using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCardUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private GameObject selectedBorder;
    [SerializeField] private Button button;

    private UnitSO unitSO;
    public event Action<CharacterCardUI> OnSelected;

    public UnitSO Unit => unitSO;

    public void Initialize(UnitSO unit)
    {
        if (unit == null)
        {
            Debug.LogWarning($"{name} received a null UnitSO.");
            return;
        }

        if (portraitImage == null || nameText == null || selectedBorder == null || button == null)
        {
            Debug.LogWarning($"{name} is missing UI references for CharacterCardUI.");
            return;
        }

        unitSO = unit;

        portraitImage.sprite = unit.Portrait;
        nameText.text = unit.Name;

        SetSelected(false);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        OnSelected?.Invoke(this);
    }

    public void SetSelected(bool selected)
    {
        selectedBorder.SetActive(selected);
    }
}