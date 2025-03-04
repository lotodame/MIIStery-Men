using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapArea))]
public class MapAreaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        int totalChanceGrass = serializedObject.FindProperty("totalChance").intValue;
        int totalChanceWater = serializedObject.FindProperty("totalChanceWater").intValue;

        if (totalChanceGrass != 100 && totalChanceGrass != -1)
            EditorGUILayout.HelpBox("The total chance percentage of pokemon in grass is not 100", MessageType.Error);

        if (totalChanceWater != 100 && totalChanceWater != -1)
            EditorGUILayout.HelpBox("The total chance percentage of pokemon in water is not 100", MessageType.Error);
    }
}
