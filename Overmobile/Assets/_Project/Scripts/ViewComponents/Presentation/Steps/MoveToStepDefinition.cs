using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;

namespace ViewComponents.Presentation
{
    [Serializable]
    public sealed class MoveToStepDefinition : PresentationStepDefinition
    {
        [SerializeField] private Transform _movingTransform;
        [SerializeField] private PresentationMoveDestination _destination;
        [SerializeField] private string _anchorKey;
        [SerializeField] private Transform _targetTransform;

        [FormerlySerializedAs("_durationSeconds")]
        [SerializeField] private float _transitionSpeed = 2f;

        [SerializeField] private bool _isInstant;
        [SerializeField] private Ease _ease = Ease.Linear;

        public override async UniTask ExecuteAsync(PresentationContext context, CancellationToken cancellationToken)
        {
            Transform movingTransform = ResolveMovingTransform(context);
            Vector3 destinationPosition = ResolveDestinationPosition(context);

            if (_isInstant)
            {
                movingTransform.position = destinationPosition;

                return;
            }

            if (_transitionSpeed <= 0f)
            {
                throw new InvalidPresentationStepDefinitionException(
                    nameof(MoveToStepDefinition),
                    "Transition speed must be greater than zero"
                );
            }

            float durationSeconds = Vector3.Distance(movingTransform.position, destinationPosition) / _transitionSpeed;
            Tween moveTween = movingTransform.DOMove(destinationPosition, durationSeconds).SetEase(_ease);

            await PresentationSequenceHelper.AwaitTweenAsync(context.Owner, moveTween, cancellationToken);
        }

        private Transform ResolveMovingTransform(PresentationContext context)
        {
            if (_movingTransform != null)
            {
                return _movingTransform;
            }

            PresentationStepSequence owner = context.Owner;

            if (owner == null)
            {
                throw new InvalidPresentationStepDefinitionException(
                    nameof(MoveToStepDefinition),
                    "Moving transform is not assigned"
                );
            }

            return owner.transform;
        }

        private Vector3 ResolveDestinationPosition(PresentationContext context)
        {
            switch (_destination)
            {
                case PresentationMoveDestination.ActiveCharacterPowerPanel:
                    return ResolveActiveCharacterPowerPanelPosition(context);

                case PresentationMoveDestination.ActiveCharacterAnchor:
                    return ResolveActiveCharacterAnchorPosition(context);

                case PresentationMoveDestination.TargetTransform:
                    return ResolveTargetTransformPosition();

                default:
                    throw new InvalidPresentationStepDefinitionException(
                        nameof(MoveToStepDefinition),
                        $"Unsupported destination '{_destination}'"
                    );
            }
        }

        private Vector3 ResolveActiveCharacterPowerPanelPosition(PresentationContext context)
        {
            IActiveCharacterPresentationProvider provider = ResolveActiveCharacterPresentationProvider(context);

            return provider.ActivePowerPanelTransform.position;
        }

        private Vector3 ResolveActiveCharacterAnchorPosition(PresentationContext context)
        {
            if (string.IsNullOrWhiteSpace(_anchorKey))
            {
                throw new InvalidPresentationStepDefinitionException(nameof(MoveToStepDefinition), "Anchor key is not assigned");
            }

            IActiveCharacterPresentationProvider provider = ResolveActiveCharacterPresentationProvider(context);

            return provider.GetAnchor(_anchorKey).position;
        }

        private Vector3 ResolveTargetTransformPosition()
        {
            if (_targetTransform == null)
            {
                throw new InvalidPresentationStepDefinitionException(
                    nameof(MoveToStepDefinition),
                    "Target transform is not assigned"
                );
            }

            return _targetTransform.position;
        }

        private IActiveCharacterPresentationProvider ResolveActiveCharacterPresentationProvider(
            PresentationContext context)
        {
            IActiveCharacterPresentationProvider provider = context.ActiveCharacterPresentationProvider;

            if (provider == null)
            {
                throw new InvalidPresentationStepDefinitionException(
                    nameof(MoveToStepDefinition),
                    "Active character presentation provider is not injected"
                );
            }

            return provider;
        }
    }
}
