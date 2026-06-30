using Core.Animation;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using ViewComponents.Animation;

namespace ViewComponents.Presentation
{
    [Serializable]
    public sealed class PlayAnimationStepDefinition : PresentationStepDefinition
    {
        [SerializeField] private CharacterAnimationView _animationView;
        [SerializeField] private CharacterAnimationSlot _animationSlot;
        [SerializeField] private bool _waitForCompletion;

        public override async UniTask ExecuteAsync(PresentationContext context, CancellationToken cancellationToken)
        {
            CharacterAnimationView animationView = ResolveAnimationView(context);

            if (_animationSlot == CharacterAnimationSlot.None)
            {
                throw new InvalidPresentationStepDefinitionException(
                    nameof(PlayAnimationStepDefinition),
                    "Animation slot is not assigned"
                );
            }

            if (_waitForCompletion)
            {
                await animationView.FireAnimationAsync(_animationSlot, cancellationToken);
            }
            else
            {
                animationView.FireAnimation(_animationSlot);
            }
        }

        private CharacterAnimationView ResolveAnimationView(PresentationContext context)
        {
            if (_animationView != null)
            {
                return _animationView;
            }

            PresentationStepSequence owner = context.Owner;

            if (owner == null)
            {
                throw new InvalidPresentationStepDefinitionException(
                    nameof(PlayAnimationStepDefinition),
                    "Animation view is not assigned"
                );
            }

            CharacterAnimationView animationView = owner.GetComponent<CharacterAnimationView>();

            if (animationView == null)
            {
                throw new InvalidPresentationStepDefinitionException(
                    nameof(PlayAnimationStepDefinition),
                    "Animation view is not assigned"
                );
            }

            return animationView;
        }
    }
}
