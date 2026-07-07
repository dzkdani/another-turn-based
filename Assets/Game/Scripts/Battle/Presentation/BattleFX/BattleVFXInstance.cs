using System;
using System.Collections;
using UnityEngine;

public class BattleVFXInstance : MonoBehaviour
{
    public GameObject Prefab { get; set; }

    public event Action<BattleVFXInstance> OnFinished;

    [SerializeField]
    private bool autoRelease = true;

    private ParticleSystem[] particles;
    private Coroutine lifetimeRoutine;

    private void Awake()
    {
        particles = GetComponentsInChildren<ParticleSystem>(true);
    }

    public void Play()
    {
        if (lifetimeRoutine != null)
        {
            StopCoroutine(lifetimeRoutine);
            lifetimeRoutine = null;
        }

        foreach (ParticleSystem ps in particles)
        {
            ps.Clear(true);
            ps.Play(true);
        }

        if (autoRelease)
            lifetimeRoutine = StartCoroutine(WaitUntilFinished());
    }

    private IEnumerator WaitUntilFinished()
    {
        while (AnyParticleAlive())
            yield return null;

        lifetimeRoutine = null;

        OnFinished?.Invoke(this);
    }

    private bool AnyParticleAlive()
    {
        foreach (ParticleSystem ps in particles)
        {
            if (ps != null && ps.IsAlive(true))
                return true;
        }

        return false;
    }

    private void OnDisable()
    {
        if (lifetimeRoutine != null)
        {
            StopCoroutine(lifetimeRoutine);
            lifetimeRoutine = null;
        }
    }
}