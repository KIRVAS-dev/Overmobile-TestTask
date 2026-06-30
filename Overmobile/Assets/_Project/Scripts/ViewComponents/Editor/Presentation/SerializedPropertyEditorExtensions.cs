using System.Collections.Generic;
using UnityEditor;

namespace ViewComponents.Editor.Presentation
{
    static class SerializedPropertyEditorExtensions
    {
        internal static IEnumerable<SerializedProperty> EnumerateVisibleChildren(this SerializedProperty property)
        {
            SerializedProperty iterator = property.Copy();
            SerializedProperty endProperty = iterator.GetEndProperty();

            if (!iterator.NextVisible(true))
            {
                yield break;
            }

            while (!SerializedProperty.EqualContents(iterator, endProperty))
            {
                yield return iterator.Copy();

                if (!iterator.NextVisible(false))
                {
                    yield break;
                }
            }
        }
    }
}
