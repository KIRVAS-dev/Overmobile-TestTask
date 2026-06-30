using UnityEditor;
using ViewComponents.Presentation;

namespace ViewComponents.Editor.Presentation
{
    [CustomEditor(typeof(PresentationObjectSpawner))]
    public sealed class PresentationObjectSpawnerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty prefabsProperty = serializedObject.FindProperty("_prefabs");
            SerializedProperty spawnRandomlyProperty = serializedObject.FindProperty("_spawnRandomly");
            SerializedProperty spawnCountProperty = serializedObject.FindProperty("_spawnCount");

            EditorGUILayout.PropertyField(prefabsProperty);
            EditorGUILayout.PropertyField(spawnRandomlyProperty);

            if (spawnRandomlyProperty.boolValue)
            {
                EditorGUILayout.PropertyField(spawnCountProperty);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
