using UnityEditor;
using UnityEngine;
using ViewComponents.Presentation;

namespace ViewComponents.Editor.Presentation
{
    [CustomPropertyDrawer(typeof(TransformToStepDefinition))]
    public sealed class TransformToStepDefinitionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float previousLabelWidth = EditorGUIUtility.labelWidth;
            PresentationInspectorLayout.ApplyLabelWidth(position.width);

            SerializedProperty movingTransformProperty = property.FindPropertyRelative("_movingTransform");
            SerializedProperty channelsProperty = property.FindPropertyRelative("_channels");
            SerializedProperty targetPositionProperty = property.FindPropertyRelative("_targetPosition");
            SerializedProperty targetRotationProperty = property.FindPropertyRelative("_targetRotation");
            SerializedProperty targetScaleProperty = property.FindPropertyRelative("_targetScale");
            SerializedProperty transitionSpeedProperty = property.FindPropertyRelative("_transitionSpeed");
            SerializedProperty easeProperty = property.FindPropertyRelative("_ease");

            PresentationTransformChannels channels = (PresentationTransformChannels)channelsProperty.intValue;

            float y = position.y;

            y = DrawLine(position, y, movingTransformProperty);
            y = DrawLine(position, y, channelsProperty);

            if (HasChannel(channels, PresentationTransformChannels.Position))
            {
                y = DrawLine(
                    position,
                    y,
                    targetPositionProperty,
                    new GUIContent("Target Position", "Local position in parent space")
                );
            }

            if (HasChannel(channels, PresentationTransformChannels.Rotation))
            {
                y = DrawLine(
                    position,
                    y,
                    targetRotationProperty,
                    new GUIContent("Target Rotation", "Local euler angles in degrees")
                );
            }

            if (HasChannel(channels, PresentationTransformChannels.Scale))
            {
                y = DrawLine(position, y, targetScaleProperty, new GUIContent("Target Scale", "Local scale"));
            }

            y = DrawLine(position, y, transitionSpeedProperty);
            DrawLine(position, y, easeProperty);

            EditorGUIUtility.labelWidth = previousLabelWidth;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty movingTransformProperty = property.FindPropertyRelative("_movingTransform");
            SerializedProperty channelsProperty = property.FindPropertyRelative("_channels");
            SerializedProperty targetPositionProperty = property.FindPropertyRelative("_targetPosition");
            SerializedProperty targetRotationProperty = property.FindPropertyRelative("_targetRotation");
            SerializedProperty targetScaleProperty = property.FindPropertyRelative("_targetScale");
            SerializedProperty transitionSpeedProperty = property.FindPropertyRelative("_transitionSpeed");
            SerializedProperty easeProperty = property.FindPropertyRelative("_ease");

            PresentationTransformChannels channels = (PresentationTransformChannels)channelsProperty.intValue;

            float height = EditorGUI.GetPropertyHeight(movingTransformProperty, includeChildren: false)
              + EditorGUI.GetPropertyHeight(channelsProperty, includeChildren: false)
              + EditorGUI.GetPropertyHeight(transitionSpeedProperty, includeChildren: false)
              + EditorGUI.GetPropertyHeight(easeProperty, includeChildren: false);

            int lineCount = 4;

            if (HasChannel(channels, PresentationTransformChannels.Position))
            {
                height += EditorGUI.GetPropertyHeight(targetPositionProperty, includeChildren: false);
                lineCount++;
            }

            if (HasChannel(channels, PresentationTransformChannels.Rotation))
            {
                height += EditorGUI.GetPropertyHeight(targetRotationProperty, includeChildren: false);
                lineCount++;
            }

            if (HasChannel(channels, PresentationTransformChannels.Scale))
            {
                height += EditorGUI.GetPropertyHeight(targetScaleProperty, includeChildren: false);
                lineCount++;
            }

            return height + (lineCount - 1) * EditorGUIUtility.standardVerticalSpacing;
        }

        private static bool HasChannel(PresentationTransformChannels channels, PresentationTransformChannels channel)
        {
            return (channels & channel) == channel;
        }

        private static float DrawLine(Rect blockRect, float y, SerializedProperty property, GUIContent label = null)
        {
            float height = EditorGUI.GetPropertyHeight(property, includeChildren: false);
            Rect lineRect = new Rect(blockRect.x, y, blockRect.width, height);

            if (label == null)
            {
                EditorGUI.PropertyField(lineRect, property, includeChildren: false);
            }
            else
            {
                EditorGUI.PropertyField(lineRect, property, label, includeChildren: false);
            }

            return y + height + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
