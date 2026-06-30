using System;
using UnityEditor;
using UnityEngine;
using ViewComponents.Presentation;

namespace ViewComponents.Editor.Presentation
{
    static class PresentationInspectorLayout
    {
        internal const float ExpandedStepTopPadding = 6f;
        internal const float ExpandedStepBottomPadding = 8f;
        internal const float NestedReorderableListBottomPadding = 4f;
        private const float AlphaFadeSettingsTopPadding = 6f;
        private const float ReorderableListDragHandleWidth = 16f;
        private const float LabelWidthRatio = 0.42f;

        internal static Rect ApplyDragHandleOffset(Rect rect)
        {
            rect.x += ReorderableListDragHandleWidth;
            rect.width -= ReorderableListDragHandleWidth;

            return rect;
        }

        internal static void ApplyLabelWidth(float availableWidth)
        {
            EditorGUIUtility.labelWidth = availableWidth * LabelWidthRatio;
        }

        internal static float GetStepFieldsHeight(SerializedProperty stepProperty)
        {
            if (UsesRootPropertyDrawer(stepProperty))
            {
                return EditorGUI.GetPropertyHeight(stepProperty, includeChildren: false);
            }

            float height = 0f;

            foreach (SerializedProperty field in stepProperty.EnumerateVisibleChildren())
            {
                if (IsAlphaFadeStep(stepProperty)
                 && IsAlphaFadeTargetsField(field))
                {
                    height += AlphaFadeSettingsTopPadding;
                }

                height += EditorGUI.GetPropertyHeight(field, includeChildren: true) + EditorGUIUtility.standardVerticalSpacing;
            }

            if (height > 0f)
            {
                height -= EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }

        internal static void DrawStepFields(Rect rect, SerializedProperty stepProperty)
        {
            if (UsesRootPropertyDrawer(stepProperty))
            {
                float propertyHeight = EditorGUI.GetPropertyHeight(stepProperty, includeChildren: false);

                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, rect.width, propertyHeight),
                    stepProperty,
                    GUIContent.none,
                    includeChildren: false
                );

                return;
            }

            float y = rect.y;

            foreach (SerializedProperty field in stepProperty.EnumerateVisibleChildren())
            {
                if (IsAlphaFadeStep(stepProperty)
                 && IsAlphaFadeTargetsField(field))
                {
                    y += AlphaFadeSettingsTopPadding;
                }

                float propertyHeight = EditorGUI.GetPropertyHeight(field, includeChildren: true);
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, propertyHeight), field, includeChildren: true);
                y += propertyHeight + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        private static bool UsesRootPropertyDrawer(SerializedProperty stepProperty)
        {
            Type stepType = stepProperty.managedReferenceValue?.GetType();

            return stepType == typeof(MoveToStepDefinition)
                || stepType == typeof(MoveAndTransformStepDefinition)
                || stepType == typeof(TransformToStepDefinition);
        }

        private static bool IsAlphaFadeStep(SerializedProperty stepProperty)
        {
            return stepProperty.managedReferenceValue?.GetType() == typeof(AlphaFadeStepDefinition);
        }

        private static bool IsAlphaFadeTargetsField(SerializedProperty property)
        {
            return property.name == "_spriteRenderers";
        }
    }
}
