using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPreviewUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image portraitImage;

    [SerializeField] private TMP_Text nameText;
    // [SerializeField] private TMP_Text descriptionText;

    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text defenseText;
    [SerializeField] private TMP_Text speedText;

    public void Display(UnitSO unit)
    {
        if (unit == null)
        {
            Clear();
            return;
        }

        portraitImage.sprite = unit.Portrait;

        nameText.text = unit.Name;
        // descriptionText.text = unit.Description;

        hpText.text = $"HP : {unit.MaxHP}";
        attackText.text = $"ATK : {unit.Attack}";
        defenseText.text = $"DEF : {unit.Defense}";
        speedText.text = $"SPD : {unit.Speed}";
    }

    public void Clear()
    {
        portraitImage.sprite = null;

        nameText.text = "";
        // descriptionText.text = "";

        hpText.text = "";
        attackText.text = "";
        defenseText.text = "";
        speedText.text = "";
    }
}