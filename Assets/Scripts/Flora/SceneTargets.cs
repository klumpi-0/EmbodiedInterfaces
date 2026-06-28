using System;
using UnityEngine;

public class SceneTargets : MonoBehaviour
{
    public static SceneTargets Instance;

    private void Awake() => Instance = this;

    [Header("Flora Targets")]
    public Transform targetsParent;
    public Transform[] targets;

    public Transform GetTarget(int index)
    {
        return targets[index];        
    }

    public Transform[] GetTargets(int[] indices)
    {
        return Array.ConvertAll(indices, i => GetTarget(i));
    }
}