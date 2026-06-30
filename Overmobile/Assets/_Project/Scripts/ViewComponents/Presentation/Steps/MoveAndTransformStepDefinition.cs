using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;

namespace ViewComponents.Presentation
{
    [Serializable]
    public sealed class MoveAndTransformStepDefinition : PresentationStepDefinition
    {
        [SerializeField] private Transform _movingTransform;
        [SerializeField] private PresentationMoveDestination _destination;
        [SerializeField] private string _anchorKey;
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private float _transitionSpeed = 2f;
        [SerializeField] private bool _isInstant;
        [SerializeField] private Ease _moveEase = Ease.Linear;
        [SerializeField] private PresentationTransformChannels _channels = PresentationTransformChannels.Scale;
        [SerializeField] private Vector3 _targetPosition;
        [SerializeField] private Vector3 _targetRotation;
        [SerializeField] private Vector3 _targetScale = Vector3.zero;
        [SerializeField] private PresentationTransformZoneMode _transformZoneMode = PresentationTransformZoneMode.PathFraction;
        [SerializeField] private float _transformZoneFraction = 0.85f;
        [SerializeField] private float _transformZoneWorldDistance = 2f;
        [SerializeField] private Ease _transformEase = Ease.Linear;

        public override async UniTask ExecuteAsync(PresentationContext context, CancellationToken cancellationToken)
        {
            Transform movingTransform = ResolveMovingTransform(context);
            Vector3 destinationPosition = ResolveDestinationPosition(context);

            ValidateTransformChannels();
            ValidateTransformZone();

            if (_isInstant)
            {
                movingTransform.position = destinationPosition;
                ApplyTargetTransform(movingTransform);

                return;
            }

            if (_transitionSpeed <= 0f)
            {
                throw new InvalidPresentationStepDefinitionException(
                    nameof(MoveAndTransformStepDefinition),
                    "Transition speed must be greater than zero"
                );
            }

            Vector3 startPosition = movingTransform.position;
            float totalDistance = Vector3.Distance(startPosition, destinationPosition);

            if (totalDistance <= 0f)
            {
                movingTransform.position = destinationPosition;
                ApplyTargetTransform(movingTransform);

                return;
            }

            TransformSnapshot startSnapshot = CaptureTransformSnapshot(movingTransform);
            float transformZoneDistance = ResolveTransformZoneDistance(totalDistance);
            float durationSeconds = totalDistance / _transitionSpeed;

            Tween moveTween = movingTransform
               .DOMove(destinationPosition, durationSeconds)
               .SetEase(_moveEase)
               .OnUpdate(() =>
                    {
                        float remainingDistance = Vector3.Distance(movingTransform.position, destinationPosition);
                        float rawTransformProgress = CalculateRawTransformProgress(remainingDistance, transformZoneDistance);

                        float easedTransformProgress = DOVirtual.EasedValue(
                            from: 0f,
                            to: 1f,
                            rawTransformProgress,
                            _transformEase
                        );

                        ApplyTransformSnapshot(movingTransform, startSnapshot, easedTransformProgress);
                    }
                )
               .OnComplete(() => ApplyTargetTransform(movingTransform));

            await PresentationSequenceHelper.AwaitTweenAsync(context.Owner, moveTween, cancellationToken);
        }

        private void ValidateTransformChannels()
        {
            if (_channels == 0)
            {
                throw new InvalidPresentationStepDefinitionException(
                    nameof(MoveAndTransformStepDefinition),
                    "At least one transform channel must be selected"
                );
            }
        }

        private void ValidateTransformZone()
        {
            switch (_transformZoneMode)
            {
                case PresentationTransformZoneMode.PathFraction:
                    if (_transformZoneFraction <= 0f
                     || _transformZoneFraction > 1f)
                    {
                        throw new InvalidPresentationStepDefinitionException(
                            nameof(MoveAndTransformStepDefinition),
                            "Transform zone fraction must be greater than zero and less than or equal to one"
                        );
                    }

                    break;

                case PresentationTransformZoneMode.WorldDistance:
                    if (_transformZoneWorldDistance <= 0f)
                    {
                        throw new InvalidPresentationStepDefinitionException(
                            nameof(MoveAndTransformStepDefinition),
                            "Transform zone world distance must be greater than zero"
                        );
                    }

                    break;

                default:
                    throw new InvalidPresentationStepDefinitionException(
                        nameof(MoveAndTransformStepDefinition),
                        $"Unsupported transform zone mode '{_transformZoneMode}'"
                    );
            }
        }

        private float ResolveTransformZoneDistance(float totalDistance)
        {
            float zoneDistance = _transformZoneMode switch
            {
                PresentationTransformZoneMode.PathFraction => totalDistance * (1f - _transformZoneFraction),
                PresentationTransformZoneMode.WorldDistance => _transformZoneWorldDistance,
                _ => throw new InvalidPresentationStepDefinitionException(
                    nameof(MoveAndTransformStepDefinition),
                    $"Unsupported transform zone mode '{_transformZoneMode}'"
                )
            };

            return Mathf.Min(zoneDistance, totalDistance);
        }

        private static float CalculateRawTransformProgress(float remainingDistance, float transformZoneDistance)
        {
            if (transformZoneDistance <= 0f)
            {
                return 1f;
            }

            if (remainingDistance >= transformZoneDistance)
            {
                return 0f;
            }

            return 1f - remainingDistance / transformZoneDistance;
        }

        private TransformSnapshot CaptureTransformSnapshot(Transform movingTransform)
        {
            return new TransformSnapshot
            {
                LocalPosition = movingTransform.localPosition,
                LocalRotation = movingTransform.localRotation,
                LocalScale = movingTransform.localScale
            };
        }

        private void ApplyTransformSnapshot(Transform movingTransform, TransformSnapshot startSnapshot, float easedProgress)
        {
            if (HasChannel(PresentationTransformChannels.Position))
            {
                movingTransform.localPosition = Vector3.LerpUnclamped(
                    startSnapshot.LocalPosition,
                    _targetPosition,
                    easedProgress
                );
            }

            if (HasChannel(PresentationTransformChannels.Rotation))
            {
                Quaternion targetRotation = Quaternion.Euler(_targetRotation);

                movingTransform.localRotation = Quaternion.SlerpUnclamped(
                    startSnapshot.LocalRotation,
                    targetRotation,
                    easedProgress
                );
            }

            if (HasChannel(PresentationTransformChannels.Scale))
            {
                movingTransform.localScale = Vector3.LerpUnclamped(startSnapshot.LocalScale, _targetScale, easedProgress);
            }
        }

        private void ApplyTargetTransform(Transform movingTransform)
        {
            if (HasChannel(PresentationTransformChannels.Position))
            {
                movingTransform.localPosition = _targetPosition;
            }

            if (HasChannel(PresentationTransformChannels.Rotation))
            {
                movingTransform.localRotation = Quaternion.Euler(_targetRotation);
            }

            if (HasChannel(PresentationTransformChannels.Scale))
            {
                movingTransform.localScale = _targetScale;
            }
        }

        private bool HasChannel(PresentationTransformChannels channel)
        {
            return (_channels & channel) == channel;
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
                    nameof(MoveAndTransformStepDefinition),
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
                        nameof(MoveAndTransformStepDefinition),
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
                throw new InvalidPresentationStepDefinitionException(
                    nameof(MoveAndTransformStepDefinition),
                    "Anchor key is not assigned"
                );
            }

            IActiveCharacterPresentationProvider provider = ResolveActiveCharacterPresentationProvider(context);

            return provider.GetAnchor(_anchorKey).position;
        }

        private Vector3 ResolveTargetTransformPosition()
        {
            if (_targetTransform == null)
            {
                throw new InvalidPresentationStepDefinitionException(
                    nameof(MoveAndTransformStepDefinition),
                    "Target transform is not assigned"
                );
            }

            return _targetTransform.position;
        }

        private IActiveCharacterPresentationProvider ResolveActiveCharacterPresentationProvider(PresentationContext context)
        {
            IActiveCharacterPresentationProvider provider = context.ActiveCharacterPresentationProvider;

            if (provider == null)
            {
                throw new InvalidPresentationStepDefinitionException(
                    nameof(MoveAndTransformStepDefinition),
                    "Active character presentation provider is not injected"
                );
            }

            return provider;
        }

        private struct TransformSnapshot
        {
            public Vector3 LocalPosition;
            public Quaternion LocalRotation;
            public Vector3 LocalScale;
        }
    }
}
