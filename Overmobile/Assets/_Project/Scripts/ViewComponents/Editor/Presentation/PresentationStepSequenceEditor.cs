using UnityEditor;
using ViewComponents.Presentation;

namespace ViewComponents.Editor.Presentation
{
    [CustomEditor(typeof(PresentationStepSequence))]
    public sealed class PresentationStepSequenceEditor : UnityEditor.Editor
    {
        private PresentationStepsReorderableList _stepsReorderableList;

        private void OnEnable()
        {
            SerializedProperty stepsProperty = serializedObject.FindProperty("_steps");

            _stepsReorderableList = new PresentationStepsReorderableList(
                serializedObject,
                stepsProperty,
                ResolveFoldoutKeyPrefix()
            );
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_playOnEnable"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_playOnStart"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_stopOnDisable"));

            EditorGUILayout.Space();

            _stepsReorderableList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        private string ResolveFoldoutKeyPrefix()
        {
            GlobalObjectId globalObjectId = GlobalObjectId.GetGlobalObjectIdSlow(target);

            return $"{nameof(PresentationStepSequenceEditor)}.{globalObjectId}";
        }
    }
}
