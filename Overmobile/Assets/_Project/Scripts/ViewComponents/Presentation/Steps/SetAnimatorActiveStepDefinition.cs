using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace ViewComponents.Presentation
{
    [Serializable]
    public sealed class SetAnimatorActiveStepDefinition : PresentationStepDefinition
    {
        [SerializeField] private PresentationActiveMode _mode;
        [SerializeField] private Animator[] _targets = Array.Empty<Animator>();

        public override UniTask ExecuteAsync(PresentationContext context, CancellationToken cancellationToken)
        {
            Validate();

            bool isEnabled = _mode == PresentationActiveMode.Enable;

            foreach (Animator animator in _targets)
            {
                animator.enabled = isEnabled;
            }

            return UniTask.CompletedTask;
        }

        private void Validate()
        {
            if (_targets == null
             || _targets.Length == 0)
            {
                throw new InvalidPresentationStepDefinitionException(
                    nameof(SetAnimatorActiveStepDefinition),
                    "At least one target is required"
                );
            }

            for (int i = 0; i < _targets.Length; i++)
            {
                if (_targets[i] == null)
                {
                    throw new InvalidPresentationStepDefinitionException(
                        nameof(SetAnimatorActiveStepDefinition),
                        $"Target at index {i} is not assigned"
                    );
                }
            }
        }
    }
}
