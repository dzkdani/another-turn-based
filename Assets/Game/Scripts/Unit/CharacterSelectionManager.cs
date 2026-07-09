using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("Encounter")]
    [SerializeField] private EncounterDataSO currentEncounter;

    [Header("Database")]
    [SerializeField] private CharacterDatabase characterDatabase;

    [Header("UI")]
    [SerializeField] private CharacterCardUI cardPrefab;
    [SerializeField] private Transform cardParent;

    [SerializeField] private CharacterPreviewUI previewUI;

    [SerializeField] private Button addPlayerButton;
    [SerializeField] private Button startButton;

    [Header("Scene")]
    [SerializeField] private string battleSceneName = "Battle";

    private readonly List<UnitSO> selectedPlayers = new();

    private CharacterCardUI selectedCard;
    private UnitSO selectedCharacter;

    private void Start()
    {
        BattleSetup.Instance.Clear();

        GenerateCards();

        previewUI.Clear();

        addPlayerButton.onClick.AddListener(AddPlayer);
        startButton.onClick.AddListener(StartBattle);

        RefreshUI();
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

            RefreshUI();

            return;
        }

        if (selectedCard != null)
            selectedCard.SetSelected(false);

        selectedCard = card;
        selectedCharacter = card.Unit;

        selectedCard.SetSelected(true);

        previewUI.Display(selectedCharacter);

        RefreshUI();
    }

    private void AddPlayer()
    {
        if (selectedCharacter == null)
            return;

        if (selectedPlayers.Contains(selectedCharacter))
            return;

        if (selectedPlayers.Count >= 2)
            return;

        selectedPlayers.Add(selectedCharacter);

        RefreshUI();
    }

    private void RefreshUI()
    {
        addPlayerButton.interactable =
            selectedCharacter != null &&
            !selectedPlayers.Contains(selectedCharacter) &&
            selectedPlayers.Count < 2;

        startButton.interactable =
            selectedPlayers.Count == 2;
    }

    private void StartBattle()
    {
        BattleSetup.Instance.CreateBattle(
            selectedPlayers,
            currentEncounter,
            characterDatabase);

        SceneManager.LoadScene(battleSceneName);
    }
}