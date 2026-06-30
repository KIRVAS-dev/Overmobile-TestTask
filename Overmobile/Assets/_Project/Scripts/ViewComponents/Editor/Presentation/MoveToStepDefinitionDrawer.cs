using UnityEditor;
using UnityEngine;
using ViewComponents.Presentation;

namespace ViewComponents.Editor.Presentation
{
    [CustomPropertyDrawer(typeof(MoveToStepDefinition))]
    public sealed class MoveToStepDefinitionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float previousLabelWidth = EditorGUIUtility.labelWidth;
            PresentationInspectorLayout.ApplyLabelWidth(position.width);

            SerializedProperty movingTransformProperty = property.FindPropertyRelative("_movingTransform");
            SerializedProperty destinationProperty = property.FindPropertyRelative("_destination");
            SerializedProperty anchorKeyProperty = property.FindPropertyRelative("_anchorKey");
            SerializedProperty targetTransformProperty = property.FindPropertyRelative("_targetTransform");
            SerializedProperty isInstantProperty = property.FindPropertyRelative("_isInstant");
            SerializedProperty transitionSpeedProperty = property.FindPropertyRelative("_transitionSpeed");
            SerializedProperty easeProperty = property.FindPropertyRelative("_ease");

            float y = position.y;

            y = DrawLine(position, y, movingTransformProperty);
            y = DrawLine(position, y, destinationProperty);

            PresentationMoveDestination destination = (PresentationMoveDestination)destinationProperty.intValue;

            if (destination == PresentationMoveDestination.ActiveCharacterAnchor)
            {
                GUIContent anchorKeyLabel = new GUIContent(
                    "Anchor Key",
                    "Key from Named Anchors on ActiveCharacterAnchorView (knight tier prefab)"
                );

                y = DrawLine(position, y, anchorKeyProperty, anchorKeyLabel);
            }
            else if (destination == PresentationMoveDestination.TargetTransform)
            {
                y = DrawLine(position, y, targetTransformProperty);
            }

            y = DrawLine(position, y, isInstantProperty);

            if (!isInstantProperty.boolValue)
            {
                y = DrawLine(position, y, transitionSpeedProperty);
                DrawLine(position, y, easeProperty);
            }

            EditorGUIUtility.labelWidth = previousLabelWidth;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty movingTransformProperty = property.FindPropertyRelative("_movingTransform");
            SerializedProperty destinationProperty = property.FindPropertyRelative("_destination");
            SerializedProperty anchorKeyProperty = property.FindPropertyRelative("_anchorKey");
            SerializedProperty targetTransformProperty = property.FindPropertyRelative("_targetTransform");
            SerializedProperty isInstantProperty = property.FindPropertyRelative("_isInstant");
            SerializedProperty transitionSpeedProperty = property.FindPropertyRelative("_transitionSpeed");
            SerializedProperty easeProperty = property.FindPropertyRelative("_ease");

            PresentationMoveDestination destination = (PresentationMoveDestination)destinationProperty.intValue;
            bool isInstant = isInstantProperty.boolValue;

            float height = EditorGUI.GetPropertyHeight(movingTransformProperty, includeChildren: false)
              + EditorGUI.GetPropertyHeight(destinationProperty, includeChildren: false)
              + EditorGUI.GetPropertyHeight(isInstantProperty, includeChildren: false);

            if (!isInstant)
            {
                height += EditorGUI.GetPropertyHeight(transitionSpeedProperty, includeChildren: false)
                  + EditorGUI.GetPropertyHeight(easeProperty, includeChildren: false);
            }

            if (destination == PresentationMoveDestination.ActiveCharacterAnchor)
            {
                height += EditorGUI.GetPropertyHeight(anchorKeyProperty, includeChildren: false);
            }
            else if (destination == PresentationMoveDestination.TargetTransform)
            {
                height += EditorGUI.GetPropertyHeight(targetTransformProperty, includeChildren: false);
            }

            int lineCount = destination switch
            {
                PresentationMoveDestination.ActiveCharacterAnchor => isInstant ? 4 : 6,
                PresentationMoveDestination.TargetTransform => isInstant ? 4 : 6,
                _ => isInstant ? 3 : 5
            };

            return height + (lineCount - 1) * EditorGUIUtility.standardVerticalSpacing;
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
