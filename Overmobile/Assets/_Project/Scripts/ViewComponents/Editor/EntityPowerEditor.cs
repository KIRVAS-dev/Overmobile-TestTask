using UnityEditor;
using UnityEngine;
using ViewComponents.Power;

namespace ViewComponents.Editor
{
    [CustomEditor(typeof(EntityPower))]
    public sealed class EntityPowerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EntityPower entityPower = (EntityPower)target;
            SerializedProperty powerIdProperty = serializedObject.FindProperty("_powerId");

            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_initialPower"));

            GUI.enabled = false;
            EditorGUILayout.PropertyField(powerIdProperty, new GUIContent("Power ID"));
            GUI.enabled = true;

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Save ID"))
            {
                bool hasPowerId = !string.IsNullOrWhiteSpace(powerIdProperty.stringValue);

                bool confirmed = !hasPowerId
                 || EditorUtility.DisplayDialog(
                        "Save ID",
                        "Are you sure you want to save a new power id?\nThe old value will be lost.",
                        "Save",
                        "Cancel"
                    );

                if (confirmed)
                {
                    Undo.RecordObject(entityPower, "Save Power ID");

                    entityPower.SavePowerId();

                    EditorUtility.SetDirty(entityPower);
                    serializedObject.Update();
                }
            }

            GUI.enabled = !string.IsNullOrWhiteSpace(powerIdProperty.stringValue);

            if (GUILayout.Button("Reset ID"))
            {
                bool confirmed = EditorUtility.DisplayDialog(
                    "Reset ID",
                    "Are you sure you want to clear the power id?",
                    "Reset",
                    "Cancel"
                );

                if (confirmed)
                {
                    Undo.RecordObject(entityPower, "Reset Power ID");

                    entityPower.ClearPowerId();

                    EditorUtility.SetDirty(entityPower);
                    serializedObject.Update();
                }
            }

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
