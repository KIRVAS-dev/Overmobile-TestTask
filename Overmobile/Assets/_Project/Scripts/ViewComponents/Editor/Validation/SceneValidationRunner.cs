using System;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ViewComponents.Editor.Validation
{
    public static class SceneValidationRunner
    {
        [MenuItem("Tools/Overmobile/Validate Loaded Scenes %#t")]
        public static void ValidateLoadedScenes()
        {
            MonoBehaviour[] allComponents = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include);

            int foundCount = 0;
            int validatedCount = 0;
            int errorCount = 0;

            foreach (MonoBehaviour validatableComponent in allComponents)
            {
                if (validatableComponent == null)
                {
                    continue;
                }

                MethodInfo validateMethod = FindValidateMethod(validatableComponent.GetType());

                if (validateMethod == null)
                {
                    continue;
                }

                foundCount++;

                try
                {
                    validateMethod.Invoke(validatableComponent, parameters: null);
                    validatedCount++;
                }
                catch (TargetInvocationException targetInvocationException)
                {
                    Exception innerException = targetInvocationException.InnerException ?? targetInvocationException;
                    string hierarchyPath = BuildHierarchyPath(validatableComponent.transform);
                    string componentTypeName = validatableComponent.GetType().Name;
                    string errorMessage = $"[{hierarchyPath}] ({componentTypeName}): {innerException.Message}";

                    Debug.LogError(errorMessage, validatableComponent);
                    errorCount++;
                }
                catch (Exception exception)
                {
                    string hierarchyPath = BuildHierarchyPath(validatableComponent.transform);
                    string componentTypeName = validatableComponent.GetType().Name;
                    string errorMessage = $"[{hierarchyPath}] ({componentTypeName}): {exception.Message}";

                    Debug.LogError(errorMessage, validatableComponent);
                    errorCount++;
                }
            }

            string summary =
                $"Scene validation complete: {foundCount} components with Validate() found, {validatedCount} passed, {errorCount} errors";

            if (errorCount > 0)
            {
                Debug.LogError(summary);
            }
            else
            {
                Debug.Log(summary);
            }
        }

        private static MethodInfo FindValidateMethod(Type componentType)
        {
            Type currentType = componentType;

            while (currentType != null
             && currentType != typeof(MonoBehaviour))
            {
                MethodInfo method = currentType.GetMethod(
                    "Validate",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly,
                    binder: null,
                    Type.EmptyTypes,
                    modifiers: null
                );

                if (method != null
                 && method.ReturnType == typeof(void))
                {
                    return method;
                }

                currentType = currentType.BaseType;
            }

            return null;
        }

        private static string BuildHierarchyPath(Transform target)
        {
            StringBuilder pathBuilder = new StringBuilder();
            Transform current = target;

            while (current != null)
            {
                if (pathBuilder.Length > 0)
                {
                    pathBuilder.Insert(index: 0, "/");
                }

                pathBuilder.Insert(index: 0, current.name);
                current = current.parent;
            }

            return pathBuilder.ToString();
        }
    }
}
