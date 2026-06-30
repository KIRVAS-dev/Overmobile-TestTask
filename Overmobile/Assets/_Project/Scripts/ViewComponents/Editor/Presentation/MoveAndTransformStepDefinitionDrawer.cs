using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ViewComponents.Presentation;

namespace ViewComponents.Editor.Presentation
{
    [CustomPropertyDrawer(typeof(MoveAndTransformStepDefinition))]
    public sealed class MoveAndTransformStepDefinitionDrawer : PropertyDrawer
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
            SerializedProperty moveEaseProperty = property.FindPropertyRelative("_moveEase");
            SerializedProperty channelsProperty = property.FindPropertyRelative("_channels");
            SerializedProperty targetPositionProperty = property.FindPropertyRelative("_targetPosition");
            SerializedProperty targetRotationProperty = property.FindPropertyRelative("_targetRotation");
            SerializedProperty targetScaleProperty = property.FindPropertyRelative("_targetScale");
            SerializedProperty transformZoneModeProperty = property.FindPropertyRelative("_transformZoneMode");
            SerializedProperty transformZoneFractionProperty = property.FindPropertyRelative("_transformZoneFraction");
            SerializedProperty transformZoneWorldDistanceProperty = property.FindPropertyRelative("_transformZoneWorldDistance");
            SerializedProperty transformEaseProperty = property.FindPropertyRelative("_transformEase");

            PresentationMoveDestination destination = (PresentationMoveDestination)destinationProperty.intValue;
            PresentationTransformChannels channels = (PresentationTransformChannels)channelsProperty.intValue;
            PresentationTransformZoneMode transformZoneMode = (PresentationTransformZoneMode)transformZoneModeProperty.intValue;

            float y = position.y;

            y = DrawLine(position, y, movingTransformProperty);
            y = DrawLine(position, y, destinationProperty);

            switch (destination)
            {
                case PresentationMoveDestination.ActiveCharacterAnchor:
                    {
                        GUIContent anchorKeyLabel = new GUIContent(
                            "Anchor Key",
                            "Key from Named Anchors on ActiveCharacterAnchorView (knight tier prefab)"
                        );

                        y = DrawLine(position, y, anchorKeyProperty, anchorKeyLabel);
                        break;
                    }

                case PresentationMoveDestination.TargetTransform:
                    y = DrawLine(position, y, targetTransformProperty);
                    break;
            }

            y = DrawLine(position, y, isInstantProperty);

            if (!isInstantProperty.boolValue)
            {
                y = DrawLine(position, y, transitionSpeedProperty);
                y = DrawLine(position, y, moveEaseProperty);
            }

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

            y = DrawLine(position, y, transformZoneModeProperty);

            if (transformZoneMode == PresentationTransformZoneMode.PathFraction)
            {
                y = DrawLine(
                    position,
                    y,
                    transformZoneFractionProperty,
                    new GUIContent(
                        "Transform Zone Fraction",
                        "Share of the path from start where transform stays unchanged; transform runs in the last (1 - fraction) part"
                    )
                );
            }
            else
            {
                y = DrawLine(
                    position,
                    y,
                    transformZoneWorldDistanceProperty,
                    new GUIContent("Transform Zone Distance", "World distance from destination where transform begins")
                );
            }

            DrawLine(position, y, transformEaseProperty);

            EditorGUIUtility.labelWidth = previousLabelWidth;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty destinationProperty = property.FindPropertyRelative("_destination");
            SerializedProperty isInstantProperty = property.FindPropertyRelative("_isInstant");
            SerializedProperty channelsProperty = property.FindPropertyRelative("_channels");
            SerializedProperty transformZoneModeProperty = property.FindPropertyRelative("_transformZoneMode");

            PresentationMoveDestination destination = (PresentationMoveDestination)destinationProperty.intValue;
            PresentationTransformChannels channels = (PresentationTransformChannels)channelsProperty.intValue;
            PresentationTransformZoneMode transformZoneMode = (PresentationTransformZoneMode)transformZoneModeProperty.intValue;
            bool isInstant = isInstantProperty.boolValue;

            string[] visibleFieldNames = GetVisibleFieldNames(destination, isInstant, channels, transformZoneMode);

            float height = 0f;

            foreach (string t in visibleFieldNames)
            {
                SerializedProperty fieldProperty = property.FindPropertyRelative(t);
                height += EditorGUI.GetPropertyHeight(fieldProperty, includeChildren: false);
            }

            return height + (visibleFieldNames.Length - 1) * EditorGUIUtility.standardVerticalSpacing;
        }

        private static string[] GetVisibleFieldNames(PresentationMoveDestination destination, bool isInstant,
            PresentationTransformChannels channels, PresentationTransformZoneMode transformZoneMode)
        {
            List<string> fieldNames = new List<string>
            {
                "_movingTransform",
                "_destination"
            };

            switch (destination)
            {
                case PresentationMoveDestination.ActiveCharacterAnchor:
                    fieldNames.Add("_anchorKey");
                    break;

                case PresentationMoveDestination.TargetTransform:
                    fieldNames.Add("_targetTransform");
                    break;
            }

            fieldNames.Add("_isInstant");

            if (!isInstant)
            {
                fieldNames.Add("_transitionSpeed");
                fieldNames.Add("_moveEase");
            }

            fieldNames.Add("_channels");

            if (HasChannel(channels, PresentationTransformChannels.Position))
            {
                fieldNames.Add("_targetPosition");
            }

            if (HasChannel(channels, PresentationTransformChannels.Rotation))
            {
                fieldNames.Add("_targetRotation");
            }

            if (HasChannel(channels, PresentationTransformChannels.Scale))
            {
                fieldNames.Add("_targetScale");
            }

            fieldNames.Add("_transformZoneMode");

            fieldNames.Add(
                transformZoneMode == PresentationTransformZoneMode.PathFraction
                    ? "_transformZoneFraction"
                    : "_transformZoneWorldDistance"
            );

            fieldNames.Add("_transformEase");

            return fieldNames.ToArray();
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
