using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using ViewComponents.Presentation;

namespace ViewComponents.Editor.Presentation
{
    public sealed class PresentationStepsReorderableList
    {
        private static readonly Type[] StepTypes =
        {
            typeof(SetGameObjectActiveStepDefinition),
            typeof(SetAnimatorActiveStepDefinition),
            typeof(DelayStepDefinition),
            typeof(AlphaFadeStepDefinition),
            typeof(MoveToStepDefinition),
            typeof(MoveAndTransformStepDefinition),
            typeof(TransformToStepDefinition),
            typeof(WaitForSequenceStepDefinition),
            typeof(PlaySequenceStepDefinition),
            typeof(ParallelStepDefinition),
            typeof(SequenceStepDefinition),
            typeof(PlayAnimationStepDefinition),
            typeof(TriggerObjectSpawnerStepDefinition),
            typeof(UpgradeTriggerStepDefinition),
            typeof(ItemPickUpStepDefinition),
            typeof(PowerUpStepDefinition),
            typeof(CameraShakeStepDefinition)
        };

        private static readonly HashSet<Type> NonExpandableStepTypes = new HashSet<Type>
        {
            typeof(UpgradeTriggerStepDefinition),
            typeof(ItemPickUpStepDefinition),
            typeof(PowerUpStepDefinition),
            typeof(CameraShakeStepDefinition)
        };

        private static GUIStyle _boldFoldoutStyle;
        private readonly ReorderableList _reorderableList;
        private readonly Dictionary<string, PresentationStepsReorderableList> _nestedListsByPropertyPath =
            new Dictionary<string, PresentationStepsReorderableList>();
        private readonly SerializedObject _serializedObject;
        private readonly SerializedProperty _stepsProperty;
        private readonly string _foldoutKeyPrefix;
        private readonly string _headerLabel;
        private readonly bool _isNested;

        public PresentationStepsReorderableList(
            SerializedObject serializedObject,
            SerializedProperty stepsProperty,
            string foldoutKeyPrefix,
            string headerLabel = "Steps",
            bool isNested = false)
        {
            _serializedObject = serializedObject;
            _stepsProperty = stepsProperty;
            _foldoutKeyPrefix = foldoutKeyPrefix;
            _headerLabel = headerLabel;
            _isNested = isNested;

            _reorderableList = new ReorderableList(
                serializedObject,
                stepsProperty,
                draggable: true,
                displayHeader: true,
                displayAddButton: true,
                displayRemoveButton: true
            );

            _reorderableList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, headerLabel);
            };

            _reorderableList.drawElementCallback = DrawStepElement;
            _reorderableList.elementHeightCallback = GetStepElementHeight;
            _reorderableList.onAddDropdownCallback = ShowAddStepMenu;
            _reorderableList.onReorderCallback = _ => PruneStaleNestedLists();
        }

        public void DoLayoutList()
        {
            PruneStaleNestedLists();
            _reorderableList.DoLayoutList();
        }

        private void DoList(Rect rect)
        {
            _reorderableList.DoList(rect);
        }

        private float GetHeight()
        {
            float height = _reorderableList.GetHeight();

            if (_isNested)
            {
                height += PresentationInspectorLayout.NestedReorderableListBottomPadding;
            }

            return height;
        }

        private void PruneStaleNestedLists()
        {
            if (_nestedListsByPropertyPath.Count == 0)
            {
                return;
            }

            List<string> stalePropertyPaths = null;

            foreach (KeyValuePair<string, PresentationStepsReorderableList> entry in _nestedListsByPropertyPath)
            {
                SerializedProperty nestedStepsProperty = _serializedObject.FindProperty(entry.Key);

                if (nestedStepsProperty == null
                 || nestedStepsProperty.serializedObject != _serializedObject)
                {
                    stalePropertyPaths ??= new List<string>();
                    stalePropertyPaths.Add(entry.Key);
                }
            }

            if (stalePropertyPaths == null)
            {
                return;
            }

            foreach (string staleProperty in stalePropertyPaths)
            {
                _nestedListsByPropertyPath.Remove(staleProperty);
            }
        }

        private void DrawStepElement(
            Rect rect,
            int index,
            bool isActive,
            bool isFocused)
        {
            SerializedProperty elementProperty = _stepsProperty.GetArrayElementAtIndex(index);
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float previousLabelWidth = EditorGUIUtility.labelWidth;

            rect.y += 2f;
            Rect contentRect = PresentationInspectorLayout.ApplyDragHandleOffset(rect);
            string stepTypeName = ResolveStepTypeName(elementProperty);

            if (elementProperty.managedReferenceValue == null)
            {
                contentRect.height = lineHeight;

                if (GUI.Button(contentRect, "Select Step Type"))
                {
                    ShowSetStepTypeMenu(elementProperty);
                }

                return;
            }

            if (!IsExpandableStep(elementProperty))
            {
                contentRect.height = lineHeight;
                EditorGUI.LabelField(contentRect, $"Step {index} · {stepTypeName}", EditorStyles.boldLabel);

                return;
            }

            contentRect.height = lineHeight;

            bool expanded = EditorGUI.Foldout(
                contentRect,
                IsStepExpanded(index),
                $"Step {index} · {stepTypeName}",
                toggleOnLabelClick: true,
                BoldFoldoutStyle
            );

            SetStepExpanded(index, expanded);

            if (!expanded)
            {
                return;
            }

            PresentationInspectorLayout.ApplyLabelWidth(contentRect.width);

            if (HasNestedSteps(elementProperty))
            {
                Rect nestedListRect = contentRect;

                nestedListRect.y += lineHeight
                  + EditorGUIUtility.standardVerticalSpacing
                  + PresentationInspectorLayout.ExpandedStepTopPadding;

                nestedListRect.height = GetNestedList(elementProperty).GetHeight();
                nestedListRect.width -= PresentationInspectorLayout.NestedReorderableListBottomPadding;

                GetNestedList(elementProperty).DoList(nestedListRect);

                EditorGUIUtility.labelWidth = previousLabelWidth;

                return;
            }

            Rect fieldsRect = contentRect;

            fieldsRect.y += lineHeight
              + EditorGUIUtility.standardVerticalSpacing
              + PresentationInspectorLayout.ExpandedStepTopPadding;

            fieldsRect.height = PresentationInspectorLayout.GetStepFieldsHeight(elementProperty);

            PresentationInspectorLayout.DrawStepFields(fieldsRect, elementProperty);
            EditorGUIUtility.labelWidth = previousLabelWidth;
        }

        private float GetStepElementHeight(int index)
        {
            SerializedProperty elementProperty = _stepsProperty.GetArrayElementAtIndex(index);

            if (elementProperty.managedReferenceValue == null)
            {
                return EditorGUIUtility.singleLineHeight * 2f + 6f;
            }

            if (!IsExpandableStep(elementProperty)
             || !IsStepExpanded(index))
            {
                return EditorGUIUtility.singleLineHeight + 4f;
            }

            float height = EditorGUIUtility.singleLineHeight
              + EditorGUIUtility.standardVerticalSpacing
              + PresentationInspectorLayout.ExpandedStepTopPadding
              + PresentationInspectorLayout.ExpandedStepBottomPadding;

            if (HasNestedSteps(elementProperty))
            {
                height += GetNestedList(elementProperty).GetHeight();

                return height;
            }

            height += PresentationInspectorLayout.GetStepFieldsHeight(elementProperty);

            return height;
        }

        private PresentationStepsReorderableList GetNestedList(SerializedProperty nestedStepProperty)
        {
            SerializedProperty nestedStepsProperty = nestedStepProperty.FindPropertyRelative("_steps");
            string nestedStepsPropertyPath = nestedStepsProperty.propertyPath;
            string nestedHeaderLabel = ResolveNestedStepsHeaderLabel(nestedStepProperty);

            if (_nestedListsByPropertyPath.TryGetValue(nestedStepsPropertyPath, out PresentationStepsReorderableList nestedList))
            {
                return nestedList;
            }

            nestedList = new PresentationStepsReorderableList(
                _serializedObject,
                nestedStepsProperty,
                $"{_foldoutKeyPrefix}.nested.{nestedStepsPropertyPath}",
                nestedHeaderLabel,
                isNested: true
            );

            _nestedListsByPropertyPath[nestedStepsPropertyPath] = nestedList;

            return nestedList;
        }

        private void ShowAddStepMenu(Rect buttonRect, ReorderableList list)
        {
            GenericMenu menu = CreateStepTypeMenu(stepType =>
                {
                    AddStep(list.serializedProperty, stepType);
                }
            );

            menu.DropDown(buttonRect);
        }

        private void ShowSetStepTypeMenu(SerializedProperty elementProperty)
        {
            GenericMenu menu = CreateStepTypeMenu(stepType =>
                {
                    elementProperty.managedReferenceValue = Activator.CreateInstance(stepType);
                    _serializedObject.ApplyModifiedProperties();
                }
            );

            menu.ShowAsContext();
        }

        private static GenericMenu CreateStepTypeMenu(Action<Type> onStepTypeSelected)
        {
            GenericMenu menu = new GenericMenu();

            foreach (Type stepType in GetStepTypesSortedByLabel())
            {
                Type capturedStepType = stepType;
                string label = ResolveStepTypeLabel(capturedStepType);
                menu.AddItem(new GUIContent(label), on: false, () => onStepTypeSelected(capturedStepType));
            }

            return menu;
        }

        private static IReadOnlyList<Type> GetStepTypesSortedByLabel()
        {
            Type[] sortedStepTypes = (Type[])StepTypes.Clone();

            Array.Sort(
                sortedStepTypes,
                (left, right) => string.Compare(
                    ResolveStepTypeLabel(left),
                    ResolveStepTypeLabel(right),
                    StringComparison.OrdinalIgnoreCase
                )
            );

            return sortedStepTypes;
        }

        private static string ResolveStepTypeLabel(Type stepType)
        {
            if (stepType == typeof(UpgradeTriggerStepDefinition))
            {
                return "Upgrade Trigger";
            }

            if (stepType == typeof(ItemPickUpStepDefinition))
            {
                return "Item Pick Up Trigger";
            }

            if (stepType == typeof(PowerUpStepDefinition))
            {
                return "Power Up Trigger";
            }

            if (stepType == typeof(CameraShakeStepDefinition))
            {
                return "Camera Shake Trigger";
            }

            return stepType == typeof(TriggerObjectSpawnerStepDefinition)
                ? "Object Spawner Trigger"
                : stepType.Name.Replace("StepDefinition", string.Empty);
        }

        private void AddStep(SerializedProperty targetStepsProperty, Type stepType)
        {
            int index = targetStepsProperty.arraySize;
            targetStepsProperty.arraySize++;
            targetStepsProperty.GetArrayElementAtIndex(index).managedReferenceValue = Activator.CreateInstance(stepType);
            _serializedObject.ApplyModifiedProperties();
        }

        private static bool IsExpandableStep(SerializedProperty elementProperty)
        {
            Type stepType = elementProperty.managedReferenceValue?.GetType();

            return stepType == null || !NonExpandableStepTypes.Contains(stepType);
        }

        private static bool HasNestedSteps(SerializedProperty elementProperty)
        {
            Type stepType = elementProperty.managedReferenceValue?.GetType();

            return stepType == typeof(ParallelStepDefinition) || stepType == typeof(SequenceStepDefinition);
        }

        private static string ResolveNestedStepsHeaderLabel(SerializedProperty elementProperty)
        {
            Type stepType = elementProperty.managedReferenceValue?.GetType();

            return stepType == typeof(SequenceStepDefinition)
                ? "Steps"
                : "Parallel Steps";
        }

        private bool IsStepExpanded(int index)
        {
            return SessionState.GetBool(GetStepFoldoutKey(index), defaultValue: false);
        }

        private void SetStepExpanded(int index, bool expanded)
        {
            SessionState.SetBool(GetStepFoldoutKey(index), expanded);
        }

        private string GetStepFoldoutKey(int index)
        {
            return $"{_foldoutKeyPrefix}.step.{index}";
        }

        private static string ResolveStepTypeName(SerializedProperty elementProperty)
        {
            return elementProperty.managedReferenceValue == null
                ? "Unassigned"
                : ResolveStepTypeLabel(elementProperty.managedReferenceValue.GetType());
        }

        private static GUIStyle BoldFoldoutStyle
        {
            get
            {
                _boldFoldoutStyle ??= new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold };

                return _boldFoldoutStyle;
            }
        }
    }
}
