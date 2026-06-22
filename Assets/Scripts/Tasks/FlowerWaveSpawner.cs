using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlowerWaveSpawner : MonoBehaviour
{
    public static FlowerWaveSpawner Instance;

    [Header("Prefabs")]
    public GameObject[] flowerPrefabs = new GameObject[1];

    [Header("Wave Settings")]
    public Transform origin;
    public float maxRadius = 10f;
    public float waveSpeed = 5f;

    [Header("Spawn Settings")]
    public int totalFlowers;
    [Obsolete]
    [Range(0f, 1f)] public float stayRatio = 0.3f; // Anteil, der stehen bleibt

    [Header("Scale Animation")]
    public float growDuration = 1f;
    [Range(0f, 1f)] public float shrinkChance = 0.8f;
    public float shrinkDelay = 2f;

    [Header("TimeStuff")]
    [SerializeField] private float startTime;
    [SerializeField] private float maxTime = 5f;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.B))
        {
            StartWaveInputKeyboard();
        }
    }

    public void StartWaveInputKeyboard()
    {
        //SpawnWave();
        StartCoroutine(SpawnWaveTime());
    }

    public void StartWave(GameObject flower, float shrinkChance)
    {
        flowerPrefabs[0] = flower;
        this.shrinkChance = shrinkChance;
        //StartCoroutine(SpawnWave());
        StartCoroutine(SpawnWaveTime());
    }

    private IEnumerator SpawnWaveTime()
    {
        startTime = Time.time;
        int spawned = 0;
        while(spawned < totalFlowers)
        {
            float currentProgess = Mathf.InverseLerp(startTime, startTime + maxTime, Time.time);

            int targetSpawnCount = Mathf.FloorToInt(currentProgess * totalFlowers);
            while(spawned < targetSpawnCount)
            {
                SpawnFlower(currentProgess);
                spawned++;
            }
            yield return null;
        }
    }

    private void SpawnFlower(float currentProgress)
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * currentProgress * maxRadius;
        Vector3 pos = origin.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
        GameObject prefab = flowerPrefabs[Random.Range(0, flowerPrefabs.Length)];
        GameObject flower = Instantiate(prefab, pos, Quaternion.identity);

        flower.transform.localScale = Vector3.zero;
        //spawned.Add(flower);

        float distance = Vector3.Distance(origin.position, pos);
        //float delay = distance / waveSpeed;
        float delay = 0f;

        StartCoroutine(AnimateFlower(flower, delay));

    }

    private void SpawnWave()
    {
        List<GameObject> spawned = new List<GameObject>();
        startTime = Time.time;

        for (int i = 0; i < totalFlowers; i++)
        {
            //Vector2 randomCircle = Random.insideUnitCircle * maxRadius;
            float currentProgess = Mathf.InverseLerp(startTime, startTime + maxTime, Time.time);
            Vector2 randomCircle = Random.insideUnitCircle.normalized * currentProgess * maxRadius;

            Vector3 pos = origin.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
            Debug.Log($"Start position [{pos.x}. {pos.z}], Progress: {currentProgess}");

            GameObject prefab = flowerPrefabs[Random.Range(0, flowerPrefabs.Length)];
            GameObject flower = Instantiate(prefab, pos, Quaternion.identity);

            flower.transform.localScale = Vector3.zero;
            spawned.Add(flower);

            float distance = Vector3.Distance(origin.position, pos);
            //float delay = distance / waveSpeed;
            float delay = 0f;

            StartCoroutine(AnimateFlower(flower, delay));

            //yield return null; // leicht verteilt spawnen (optional)
        }
    }

    //[Obsolete]
    //private IEnumerator SpawnWaveTime()
    //{
    //    List<GameObject> spawned = new List<GameObject>();
    //    startTime = Time.time;
    //    for (int i = 0; i < totalFlowers; i++)
    //    {
    //        float currentProgess = Mathf.InverseLerp(startTime, startTime + maxTime, Time.deltaTime);
    //        Vector2 randomCircle = Random.insideUnitCircle.normalized * maxRadius * currentProgess;
    //        Debug.Log($"Random Circle x:{randomCircle.x}, y:{randomCircle.y}");
    //        Vector3 pos = origin.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

    //        GameObject prefab = flowerPrefabs[Random.Range(0, flowerPrefabs.Length)];
    //        GameObject flower = Instantiate(prefab, pos, Quaternion.identity);

    //        flower.transform.localScale = Vector3.zero;
    //        spawned.Add(flower);

    //        float distance = Vector3.Distance(origin.position, pos);
    //        float delay = distance / waveSpeed;

    //        StartCoroutine(AnimateFlower(flower, 0));

    //        yield return null; // leicht verteilt spawnen (optional)
    //    }
    //}

    private IEnumerator AnimateFlower(GameObject flower, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (flower == null) yield break;

        // Grow
        yield return StartCoroutine(ScaleOverTime(flower.transform, Vector3.zero, Vector3.one, growDuration));

        // optional: manche schrumpfen wieder
        if (/*Random.value > stayRatio &&*/ Random.value < shrinkChance)
        {
            yield return new WaitForSeconds(shrinkDelay);

            if (flower != null)
            {
                yield return StartCoroutine(ScaleOverTime(flower.transform, Vector3.one, Vector3.zero, growDuration));

                Destroy(flower);
            }
        }
        else
        {
            var spawnedObject = flower.GetComponent<SpawnedObject>();
            if (spawnedObject != null)
            {
                spawnedObject.ActivateSpecialThing();
            }
        }
    }

    private IEnumerator ScaleOverTime(Transform t, Vector3 from, Vector3 to, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            float t01 = time / duration;
            t.transform.localScale = Vector3.Lerp(from, to, t01);

            time += Time.deltaTime;
            yield return null;
        }

        t.transform.localScale = to;
    }
}