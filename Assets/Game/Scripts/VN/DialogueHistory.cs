using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Fungus
{
    public class DialogueHistory : MonoBehaviour
    {
        public static DialogueHistory Instance;

        [SerializeField] private GameObject historyPanel;
        [SerializeField] private TMP_Text historyText;

        private readonly List<string> history = new();

        private void Awake()
        {
            Instance = this;

            if (historyPanel != null)
                historyPanel.SetActive(false);
        }

        public void Add(string speaker, string dialogue)
        {
            history.Add($"{speaker}: {dialogue}");
        }

        public void ToggleHistory()
        {
            if (historyPanel == null)
                return;

            historyPanel.SetActive(!historyPanel.activeSelf);

            if (historyPanel.activeSelf)
                Refresh();
        }

        private void Refresh()
        {
            historyText.text = string.Join("\n\n", history);
        }

        public void Clear()
        {
            history.Clear();

            if (historyText != null)
                historyText.text = "";
        }
    }
}