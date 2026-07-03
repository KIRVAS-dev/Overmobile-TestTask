using UnityEditor;
using UnityEngine;
using ViewComponents.UI.PowerPanel;

namespace ViewComponents.Editor.UI.PowerPanel
{
    [CustomPropertyDrawer(typeof(PowerPanelVisibilityTransition))]
    public sealed class PowerPanelVisibilityTransitionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty modeProperty = property.FindPropertyRelative("_mode");
            SerializedProperty fadeDurationProperty = property.FindPropertyRelative("_fadeDuration");
            SerializedProperty fadeEaseProperty = property.FindPropertyRelative("_fadeEase");

            PowerPanelVisibilityTransitionMode mode = (PowerPanelVisibilityTransitionMode)modeProperty.intValue;

            float y = position.y;

            y = DrawLine(position, y, modeProperty);

            if (mode == PowerPanelVisibilityTransitionMode.Fade)
            {
                y = DrawLine(position, y, fadeDurationProperty);
                DrawLine(position, y, fadeEaseProperty);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty modeProperty = property.FindPropertyRelative("_mode");
            PowerPanelVisibilityTransitionMode mode = (PowerPanelVisibilityTransitionMode)modeProperty.intValue;

            int visibleFieldCount = mode == PowerPanelVisibilityTransitionMode.Fade
                ? 3
                : 1;

            float height = 0f;

            height += EditorGUI.GetPropertyHeight(modeProperty, includeChildren: false);

            if (mode != PowerPanelVisibilityTransitionMode.Fade)
            {
                return height + (visibleFieldCount - 1) * EditorGUIUtility.standardVerticalSpacing;
            }

            SerializedProperty fadeDurationProperty = property.FindPropertyRelative("_fadeDuration");
            SerializedProperty fadeEaseProperty = property.FindPropertyRelative("_fadeEase");

            height += EditorGUI.GetPropertyHeight(fadeDurationProperty, includeChildren: false);
            height += EditorGUI.GetPropertyHeight(fadeEaseProperty, includeChildren: false);

            return height + (visibleFieldCount - 1) * EditorGUIUtility.standardVerticalSpacing;
        }

        private static float DrawLine(Rect blockRect, float y, SerializedProperty property)
        {
            float height = EditorGUI.GetPropertyHeight(property, includeChildren: false);
            Rect lineRect = new Rect(blockRect.x, y, blockRect.width, height);

            EditorGUI.PropertyField(lineRect, property, includeChildren: false);

            return y + height + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
