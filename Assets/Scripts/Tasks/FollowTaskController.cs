using System;
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
        var prefab = Instantiate(currentFollowPrefab, spawnParent.transform);
        MixNMatchController.Instance.SetCurrentFollowUpObject(prefab);
        //Instantiate(currentFollowTask, spawnParent.transform);
    }

    public void SetCurrentFollowTask(GameObject currentFollowPrefab)
    {
        this.currentFollowPrefab = currentFollowPrefab;
    }


}
