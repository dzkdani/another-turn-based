using UnityEngine;
using Fungus;

public class DialogueHistoryListener : MonoBehaviour, IWriterListener
{
    private SayDialog sayDialog;
    private DialogInput dialogInput;

    private void Awake()
    {
        sayDialog = GetComponent<SayDialog>();
        dialogInput = GetComponent<DialogInput>();

        if (sayDialog == null)
            sayDialog = GetComponentInParent<SayDialog>();
        if (dialogInput == null)
            dialogInput = GetComponentInParent<DialogInput>();
    }

    public void OnStart(AudioClip clip)
    {
    }

    public void OnPause()
    {
    }

    public void OnResume()
    {
    }

    public void OnGlyph()
    {
    }

    public void OnInput()
    {
    }

    public void OnAllWordsWritten()
    {
    }

    public void OnEnd(bool stopAudio)
    {
        if (DialogueHistory.Instance == null)
            return;

        if (sayDialog == null)
            return;

        DialogueHistory.Instance.Add(
            sayDialog.NameText,
            sayDialog.StoryText);
    }

    public void OnVoiceover(AudioClip voiceoverClip)
    {
        throw new System.NotImplementedException();
    }

    public void OnFastForward()
    {
        dialogInput.SetNextLineFlag();
    }
}