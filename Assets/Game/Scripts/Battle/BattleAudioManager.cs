using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BattleAudioManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource audioSourcePrefab;
    [SerializeField] private Transform audioRoot;
    [SerializeField] private AudioMixerGroup outputMixer;

    [Header("Settings")]
    [SerializeField] private float defaultVolume = 1f;

    [SerializeField] private bool randomizePitch = true;
    [SerializeField] private Vector2 pitchRange = new(0.97f, 1.03f);

    [Header("Pool")]
    [SerializeField] private int initialPoolSize = 10;

    private readonly Queue<AudioSource> pool = new();

    private void Awake()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            AudioSource source = CreateNewSource();
            source.gameObject.SetActive(false);
            pool.Enqueue(source);
        }
    }

    public AudioSource Play(AudioClip clip)
    {
        return Play(clip, Vector3.zero, false);
    }

    public AudioSource Play(AudioClip clip, Transform followTarget)
    {
        if (followTarget == null)
            return Play(clip);

        AudioSource source = GetSource();

        source.transform.SetParent(followTarget, false);
        source.transform.localPosition = Vector3.zero;

        PlayInternal(source, clip);

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

        AudioSource source = GetSource();

        source.transform.SetParent(audioRoot);

        if (useWorldPosition)
            source.transform.position = position;

        PlayInternal(source, clip);

        return source;
    }

    private void PlayInternal(AudioSource source, AudioClip clip)
    {
        source.gameObject.SetActive(true);

        source.clip = clip;
        source.volume = defaultVolume;

        source.pitch = randomizePitch
            ? Random.Range(pitchRange.x, pitchRange.y)
            : 1f;

        source.Play();

        StartCoroutine(ReturnToPool(source));
    }

    private IEnumerator ReturnToPool(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying);

        source.Stop();
        source.clip = null;
        source.pitch = 1f;

        source.transform.SetParent(audioRoot);

        source.gameObject.SetActive(false);

        pool.Enqueue(source);
    }

    private AudioSource GetSource()
    {
        if (pool.Count == 0)
            return CreateNewSource();

        return pool.Dequeue();
    }

    private AudioSource CreateNewSource()
    {
        AudioSource source;

        if (audioSourcePrefab != null)
        {
            source = Instantiate(audioSourcePrefab, audioRoot);
        }
        else
        {
            GameObject go = new("BattleAudio");
            go.transform.SetParent(audioRoot);

            source = go.AddComponent<AudioSource>();
        }

        source.playOnAwake = false;

        if (outputMixer != null)
            source.outputAudioMixerGroup = outputMixer;

        return source;
    }
}