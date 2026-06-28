using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PuzzleData))]
public class PuzzleDataEditor : Editor
{
    private readonly string[] dropdownProperties =
    {
        "intro_transforms",
        "plant_transforms",
        "moreInfo_transforms",
        "weiter_transforms"
    };

    public override void OnInspectorGUI()
    {
        SceneTargets sceneTargets = FindFirstObjectByType<SceneTargets>();
        string[] targetNames = GetTargetNames(sceneTargets);

        serializedObject.Update();

        SerializedProperty prop = serializedObject.GetIterator();
        prop.NextVisible(true); // überspringt m_Script

        while (prop.NextVisible(false))
        {
            if (System.Array.Exists(dropdownProperties, p => p == prop.name))
            {
                // Dropdown statt normalem Feld
                EditorGUILayout.LabelField(prop.displayName, EditorStyles.boldLabel);
                DrawIndexDropdowns(prop.name, targetNames);
            }
            else
            {
                // Alles andere normal zeichnen
                EditorGUILayout.PropertyField(prop, true);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
    private void DrawIndexDropdowns(string propertyName, string[] targetNames)
    {
        SerializedProperty array = serializedObject.FindProperty(propertyName);

        if (array == null)
        {
            EditorGUILayout.HelpBox($"Property '{propertyName}' not found!", MessageType.Error);
            return;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Transforms Size");
        array.arraySize = EditorGUILayout.IntField(array.arraySize);
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < array.arraySize; i++)
        {
            SerializedProperty element = array.GetArrayElementAtIndex(i);
            element.intValue = EditorGUILayout.Popup(
                $"Target {i}",
                element.intValue,
                targetNames
            );
        }
    }

    private string[] GetTargetNames(SceneTargets sceneTargets)
    {
        if (sceneTargets == null || sceneTargets.targets == null || sceneTargets.targets.Length == 0)
            return new string[] { "No SceneTargets found in Scene" };

        string[] names = new string[sceneTargets.targets.Length];
        for (int i = 0; i < sceneTargets.targets.Length; i++)
        {
            names[i] = sceneTargets.targets[i] != null
                ? $"{i}: {sceneTargets.targets[i].name}"
                : $"{i}: (empty)";
        }
        return names;
    }
}