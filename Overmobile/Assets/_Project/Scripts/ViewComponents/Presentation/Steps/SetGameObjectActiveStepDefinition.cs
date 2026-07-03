using Cysharp.Threading.Tasks;
using ExtendedExceptions;
using System;
using System.Threading;
using UnityEngine;

namespace ViewComponents.Presentation
{
    [Serializable]
    public sealed class SetGameObjectActiveStepDefinition : PresentationStepDefinition
    {
        [SerializeField] private PresentationActiveMode _mode;
        [SerializeField] private GameObject[] _targets = Array.Empty<GameObject>();

        public override UniTask ExecuteAsync(PresentationContext context, CancellationToken cancellationToken)
        {
            Validate();

            bool isActive = _mode == PresentationActiveMode.Enable;

            foreach (GameObject gameObject in _targets)
            {
                gameObject.SetActive(isActive);
            }

            return UniTask.CompletedTask;
        }

        private void Validate()
        {
            Guard.AgainstNullOrEmpty(
                _targets,
                () => new InvalidPresentationStepDefinitionException(
                    nameof(SetGameObjectActiveStepDefinition),
                    "At least one target is required"
                )
            );

            for (int i = 0; i < _targets.Length; i++)
            {
                if (!_targets[i])
                {
                    throw new InvalidPresentationStepDefinitionException(
                        nameof(SetGameObjectActiveStepDefinition),
                        $"Target at index {i} is not assigned"
                    );
                }
            }
        }
    }
}
