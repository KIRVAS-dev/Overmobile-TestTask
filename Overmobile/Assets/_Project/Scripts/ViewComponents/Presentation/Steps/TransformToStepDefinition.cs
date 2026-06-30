using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ViewComponents.Presentation
{
    [Serializable]
    public sealed class TransformToStepDefinition : PresentationStepDefinition
    {
        [SerializeField] private Transform _movingTransform;
        [SerializeField] private PresentationTransformChannels _channels = PresentationTransformChannels.Scale;
        [SerializeField] private Vector3 _targetPosition;
        [SerializeField] private Vector3 _targetRotation;
        [SerializeField] private Vector3 _targetScale = Vector3.one;
        [SerializeField] private float _transitionSpeed = 2f;
        [SerializeField] private Ease _ease = Ease.Linear;

        public override async UniTask ExecuteAsync(PresentationContext context, CancellationToken cancellationToken)
        {
            Transform movingTransform = ResolveMovingTransform(context);

            if (_channels == 0)
            {
                throw new InvalidPresentationStepDefinitionException(
                    nameof(TransformToStepDefinition),
                    "At least one transform channel must be selected"
                );
            }

            if (_transitionSpeed <= 0f)
            {
                throw new InvalidPresentationStepDefinitionException(
                    nameof(TransformToStepDefinition),
                    "Transition speed must be greater than zero"
                );
            }

            float durationSeconds = CalculateTransitionDuration(movingTransform);

            if (durationSeconds <= 0f)
            {
                ApplyTargetTransform(movingTransform);

                return;
            }

            List<Tween> tweens = CreateTransformTweens(movingTransform, durationSeconds);

            await PresentationSequenceHelper.AwaitTweensAsync(context.Owner, tweens, cancellationToken);
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
                    nameof(TransformToStepDefinition),
                    "Moving transform is not assigned"
                );
            }

            return owner.transform;
        }

        private float CalculateTransitionDuration(Transform movingTransform)
        {
            float transitionDelta = 0f;

            if (HasChannel(PresentationTransformChannels.Position))
            {
                transitionDelta = Mathf.Max(transitionDelta, Vector3.Distance(movingTransform.localPosition, _targetPosition));
            }

            if (HasChannel(PresentationTransformChannels.Rotation))
            {
                Quaternion targetRotation = Quaternion.Euler(_targetRotation);

                transitionDelta = Mathf.Max(transitionDelta, Quaternion.Angle(movingTransform.localRotation, targetRotation));
            }

            if (HasChannel(PresentationTransformChannels.Scale))
            {
                transitionDelta = Mathf.Max(transitionDelta, Vector3.Distance(movingTransform.localScale, _targetScale));
            }

            return transitionDelta / _transitionSpeed;
        }

        private List<Tween> CreateTransformTweens(Transform movingTransform, float durationSeconds)
        {
            List<Tween> tweens = new List<Tween>();

            if (HasChannel(PresentationTransformChannels.Position))
            {
                tweens.Add(movingTransform.DOLocalMove(_targetPosition, durationSeconds).SetEase(_ease));
            }

            if (HasChannel(PresentationTransformChannels.Rotation))
            {
                tweens.Add(movingTransform.DOLocalRotate(_targetRotation, durationSeconds).SetEase(_ease));
            }

            if (HasChannel(PresentationTransformChannels.Scale))
            {
                tweens.Add(movingTransform.DOScale(_targetScale, durationSeconds).SetEase(_ease));
            }

            return tweens;
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
    }
}
