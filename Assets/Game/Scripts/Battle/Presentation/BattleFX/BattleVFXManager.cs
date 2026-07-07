using System.Collections.Generic;
using UnityEngine;

public class BattleVFXManager : MonoBehaviour
{
    [SerializeField]
    private Transform worldVFXRoot;

    [SerializeField]
    private int preloadPerPrefab = 3;

    private readonly Dictionary<GameObject, Queue<BattleVFXInstance>> pools = new();

    public GameObject Play(GameObject prefab)
    {
        if (prefab == null)
            return null;

        BattleVFXInstance instance = Get(prefab);

        instance.transform.SetParent(worldVFXRoot, true);

        instance.gameObject.SetActive(true);

        return instance.gameObject;
    }

    public GameObject Play(GameObject prefab, Vector3 position)
    {
        if (prefab == null)
            return null;

        BattleVFXInstance instance = Get(prefab);

        instance.transform.SetParent(worldVFXRoot);

        instance.transform.position = position;
        instance.transform.rotation = Quaternion.identity;

        instance.gameObject.SetActive(true);

        return instance.gameObject;
    }

    public GameObject Play(GameObject prefab, Transform follow)
    {
        if (prefab == null || follow == null)
            return null;

        BattleVFXInstance instance = Get(prefab);

        instance.transform.SetParent(follow, false);

        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;

        instance.gameObject.SetActive(true);

        return instance.gameObject;
    }

    private BattleVFXInstance Get(GameObject prefab)
    {
        if (!pools.TryGetValue(prefab, out Queue<BattleVFXInstance> queue))
        {
            queue = new Queue<BattleVFXInstance>();

            pools.Add(prefab, queue);

            for (int i = 0; i < preloadPerPrefab; i++)
                queue.Enqueue(Create(prefab));
        }

        if (queue.Count == 0)
            return Create(prefab);

        return queue.Dequeue();
    }

    private BattleVFXInstance Create(GameObject prefab)
    {
        GameObject go = Instantiate(prefab, worldVFXRoot);

        go.SetActive(false);

        BattleVFXInstance instance =
            go.GetComponent<BattleVFXInstance>();

        if (instance == null)
            instance = go.AddComponent<BattleVFXInstance>();

        instance.Prefab = prefab;

        instance.OnFinished += ReturnToPool;

        return instance;
    }

    private void ReturnToPool(BattleVFXInstance instance)
    {
        instance.gameObject.SetActive(false);

        instance.transform.SetParent(worldVFXRoot);

        pools[instance.Prefab].Enqueue(instance);
    }
}