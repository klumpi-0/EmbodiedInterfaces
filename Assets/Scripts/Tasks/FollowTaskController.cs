using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Is the controller which is used to create the following task of the mixNmatch
/// </summary>
public class FollowTaskController : MonoBehaviour
{
    public static FollowTaskController Instance;

    [SerializeField] private GameObject currentFollowPrefab;
    [SerializeField] private GameObject spawnParent;
    [SerializeField] private float growingTime = 4f;
    [SerializeField] private GameObject instanceObject;

    public UnityEvent finishedTaskEvent;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MixNMatchController.Instance.foundMatchEvent.AddListener(SpawnFollowTask);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnFollowTask()
    {
        instanceObject = Instantiate(currentFollowPrefab, spawnParent.transform);
        MixNMatchController.Instance.SetCurrentFollowUpObject(instanceObject);
        SlowlyGrowPlant();
        //Instantiate(currentFollowTask, spawnParent.transform);
    }

    public void SetCurrentFollowTask(GameObject currentFollowPrefab)
    {
        this.currentFollowPrefab = currentFollowPrefab;
    }

    private void SlowlyGrowPlant()
    {
        StartCoroutine(GrowCoroutine());
    }

    private IEnumerator GrowCoroutine()
    {
        float startTime = Time.time;
        float endTime = startTime + growingTime;
        float endScale = instanceObject.transform.localScale.x;
        while(Time.time < endTime)
        {
            float t = (Time.time - startTime) / growingTime; 
            float currentScale = Mathf.Lerp(0f, endScale, t);
            instanceObject.transform.localScale = new Vector3(currentScale, currentScale, currentScale); 
            yield return null;
        }
        instanceObject.transform.localScale = new Vector3(endScale, endScale, endScale);
    }


}
