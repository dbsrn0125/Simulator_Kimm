#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(TrafficLightGroup))]
public class TrafficLightDurationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var group = (TrafficLightGroup)target;
        var lights = group.TrafficLightsReadonly;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Traffic Light Setting", EditorStyles.boldLabel);

        if (lights != null)
        {
            foreach (var light in lights)
            {
                if (light == null) continue;

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(light.name, EditorStyles.boldLabel);

                // // Duration 설정 필드
                // light.redDuration = EditorGUILayout.FloatField("Red Duration", light.redDuration);
                // light.yellowDuration = EditorGUILayout.FloatField("Yellow Duration", light.yellowDuration);
                // light.greenDuration = EditorGUILayout.FloatField("Green Duration", light.greenDuration);
                // light.leftGreenDuration = EditorGUILayout.FloatField("Left Green Duration", light.leftGreenDuration);
                // light.leftRedDuration = EditorGUILayout.FloatField("Left Red Duration", light.leftRedDuration);
                // light.leftDuration = EditorGUILayout.FloatField("Left Duration", light.leftDuration);

                // SerializedObject를 사용하여 phaseSequence 리스트 표시
                SerializedObject lightSO = new SerializedObject(light);
                SerializedProperty phaseSeqProp = lightSO.FindProperty("statusSequence");

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(phaseSeqProp, new GUIContent("Status Sequence"), true);
                EditorGUI.indentLevel--;

                lightSO.ApplyModifiedProperties();

                EditorGUILayout.Space(10);
            }

            // 저장 필요 시 (e.g. Undo/Dirty)
            if (GUI.changed)
            {
                foreach (var light in lights)
                {
                    if (light != null)
                        EditorUtility.SetDirty(light);
                }
                EditorUtility.SetDirty(target);
            }
        }
    }
}
#endif
