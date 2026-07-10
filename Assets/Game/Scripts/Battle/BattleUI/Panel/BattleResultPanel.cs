using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject root;

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    [SerializeField] private Button continueButton;
    [SerializeField] private Button retryButton;

    private void Awake()
    {
        Hide();
    }

    private void OnEnable()
    {
        BattleEvents.OnVictory += ShowVictory;
        BattleEvents.OnDefeat += ShowDefeat;
    }

    private void OnDisable()
    {
        BattleEvents.OnVictory -= ShowVictory;
        BattleEvents.OnDefeat -= ShowDefeat;
    }

    public void ShowVictory()
    {
        root.SetActive(true);

        titleText.text = "Victory!";
        descriptionText.text = "All enemies have been defeated.";

        continueButton.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(false);
    }

    public void ShowDefeat()
    {
        root.SetActive(true);

        titleText.text = "Defeat";
        descriptionText.text = "Your party has fallen.";

        continueButton.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(true);
    }

    public void Hide()
    {
        root.SetActive(false);
    }

    public void OnContinuePressed()
    {
        Debug.Log("Continue");
    }

    public void OnRetryPressed()
    {
        Scene current = SceneManager.GetActiveScene();

        SceneManager.LoadScene(current.buildIndex);
    }
}