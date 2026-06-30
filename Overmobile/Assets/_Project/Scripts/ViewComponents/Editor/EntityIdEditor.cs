using UnityEditor;
using UnityEngine;
using EntityId = ViewComponents.Entity.EntityId;

namespace ViewComponents.Editor
{
    [CustomEditor(typeof(EntityId))]
    public sealed class EntityIdEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EntityId entityId = (EntityId)target;
            SerializedProperty idProperty = serializedObject.FindProperty("_id");

            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.PropertyField(idProperty, new GUIContent("Entity ID"));
            GUI.enabled = true;

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Generate ID"))
            {
                bool hasId = !string.IsNullOrWhiteSpace(idProperty.stringValue);

                bool confirmed = !hasId
                 || EditorUtility.DisplayDialog(
                        "Save ID",
                        "Are you sure you want to save a new entity id?\nThe old value will be lost.",
                        "Save",
                        "Cancel"
                    );

                if (confirmed)
                {
                    Undo.RecordObject(entityId, "Save Entity ID");

                    entityId.SaveId();

                    EditorUtility.SetDirty(entityId);
                    serializedObject.Update();
                }
            }

            GUI.enabled = !string.IsNullOrWhiteSpace(idProperty.stringValue);

            if (GUILayout.Button("Reset ID"))
            {
                bool confirmed = EditorUtility.DisplayDialog(
                    "Reset ID",
                    "Are you sure you want to clear the entity id?",
                    "Reset",
                    "Cancel"
                );

                if (confirmed)
                {
                    Undo.RecordObject(entityId, "Reset Entity ID");

                    entityId.ClearId();

                    EditorUtility.SetDirty(entityId);
                    serializedObject.Update();
                }
            }

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
