using UnityEngine;

public class BattleAudioManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private AudioSource audioSourcePrefab;

    [SerializeField] private Transform audioRoot;

    [SerializeField] private float defaultVolume = 1f;

    public AudioSource Play(AudioClip clip)
    {
        return Play(clip, Vector3.zero, false);
    }

    public AudioSource Play(AudioClip clip, Transform followTarget)
    {
        if (followTarget == null)
            return Play(clip);

        AudioSource source = CreateAudioSource();

        source.transform.SetParent(followTarget, false);
        source.transform.localPosition = Vector3.zero;

        source.clip = clip;
        source.volume = defaultVolume;
        source.Play();

        Destroy(source.gameObject, clip.length);

        return source;
    }

    public AudioSource Play(AudioClip clip, Vector3 worldPosition)
    {
        return Play(clip, worldPosition, true);
    }

    private AudioSource Play(AudioClip clip, Vector3 position, bool useWorldPosition)
    {
        if (clip == null)
            return null;

        AudioSource source = CreateAudioSource();

        if (useWorldPosition)
            source.transform.position = position;

        source.clip = clip;
        source.volume = defaultVolume;
        source.Play();

        Destroy(source.gameObject, clip.length);

        return source;
    }

    private AudioSource CreateAudioSource()
    {
        AudioSource source;

        if (audioSourcePrefab != null)
        {
            source = Instantiate(audioSourcePrefab);
        }
        else
        {
            GameObject go = new GameObject("BattleAudio");

            source = go.AddComponent<AudioSource>();
        }

        if (audioRoot != null)
            source.transform.SetParent(audioRoot);

        return source;
    }
}