using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SceneTargets))]
public class SceneTargetsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SceneTargets sceneTargets = (SceneTargets)target;

        EditorGUI.BeginChangeCheck();
        sceneTargets.targetsParent = (Transform)EditorGUILayout.ObjectField(
            "Targets Parent",
            sceneTargets.targetsParent,
            typeof(Transform),
            true
        );

        // Automatisch neu füllen sobald Parent geändert wird
        if (EditorGUI.EndChangeCheck() && sceneTargets.targetsParent != null)
        {
            FillFromParent(sceneTargets);
        }

        EditorGUILayout.Space();

        // Button als manuelle Option falls Children sich geändert haben
        if (GUILayout.Button("Refresh from Parent"))
        {
            FillFromParent(sceneTargets);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Targets", EditorStyles.boldLabel);

        // Liste read-only anzeigen
        GUI.enabled = false;
        foreach (var t in sceneTargets.targets)
        {
            EditorGUILayout.ObjectField(t, typeof(Transform), true);
        }
        GUI.enabled = true;
    }

    private void FillFromParent(SceneTargets sceneTargets)
    {
        sceneTargets.targets = new Transform[sceneTargets.targetsParent.childCount];
        for (int i = 0; i < sceneTargets.targetsParent.childCount; i++)
        {
            sceneTargets.targets[i] = sceneTargets.targetsParent.GetChild(i);
        }
        EditorUtility.SetDirty(sceneTargets);
    }
}