using UnityEditor;
using ViewComponents.Presentation;

namespace ViewComponents.Editor.Presentation
{
    [CustomEditor(typeof(PresentationSectionMap))]
    public sealed class PresentationSectionMapEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_sections"), includeChildren: true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
