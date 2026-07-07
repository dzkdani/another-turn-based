using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private CharacterDatabase characterDatabase;

    [Header("UI")]
    [SerializeField] private CharacterCardUI cardPrefab;
    [SerializeField] private Transform cardParent;
    [SerializeField] private CharacterPreviewUI previewUI;
    [SerializeField] private Button startButton;

    [Header("Scene")]
    [SerializeField] private string battleSceneName = "Battle";

    private CharacterCardUI selectedCard;
    private UnitSO selectedCharacter;

    private void Start()
    {
        GenerateCards();

        previewUI.Clear();

        startButton.interactable = false;
        startButton.onClick.AddListener(StartBattle);
    }

    private void GenerateCards()
    {
        foreach (UnitSO unit in characterDatabase.Characters)
        {
            CharacterCardUI card =
                Instantiate(cardPrefab, cardParent);

            card.Initialize(unit);

            card.OnSelected += SelectCard;
        }
    }

    public void SelectCard(CharacterCardUI card)
    {
        if (selectedCard == card)
        {
            selectedCard.SetSelected(false);

            selectedCard = null;
            selectedCharacter = null;

            previewUI.Clear();

            startButton.interactable = false;

            return;
        }

        if (selectedCard != null)
            selectedCard.SetSelected(false);

        selectedCard = card;
        selectedCharacter = card.Unit;

        selectedCard.SetSelected(true);

        previewUI.Display(selectedCharacter);

        startButton.interactable = true;
    }

    private void StartBattle()
    {
        BattleSetup.Instance.CreateBattle(
            selectedCharacter,
            characterDatabase);

        SceneManager.LoadScene(battleSceneName);
    }
}