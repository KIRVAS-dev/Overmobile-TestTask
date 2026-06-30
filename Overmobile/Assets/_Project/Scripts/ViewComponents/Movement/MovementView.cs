using Core.Gameplay.Movement;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using ViewComponents.Animation;

namespace ViewComponents.Movement
{
    [DisallowMultipleComponent]
    public sealed class MovementView
        : MonoBehaviour,
          IMovementView
    {
        [SerializeField] private Transform _facingTransform;
        [SerializeField] private float _facingYawOffset = 180f;
        [SerializeField] private PathType _pathType = PathType.Linear;
        [SerializeField] private PathMode _pathMode = PathMode.TopDown2D;
        [SerializeField] private Ease _movementEase = Ease.Linear;
        [SerializeField] private Ease _facingRotationEase = Ease.Linear;
        [SerializeField] private CharacterAnimationView _characterAnimationView;
        [SerializeField] private GameObject _footstepsSfx;

        public async UniTask MoveAlongPathAsync(
            IReadOnlyList<Vector3> pathPoints,
            float moveSpeed, float facingRotationDuration,
            Vector3 destinationFacingWorldPosition,
            Action<int> onWaypointReached,
            CancellationToken cancellationToken)
        {
            Vector3[] movementPath = MovementHelper.BuildMovementPath(pathPoints);

            float duration = MovementHelper.CalculatePolylineLength(transform.position, movementPath) / moveSpeed;

            BeginRunLocomotion();

            try
            {
                StartFacingRotation(movementPath[0], facingRotationDuration);

                Tween moveTween = transform
                   .DOPath(movementPath, duration, _pathType, _pathMode)
                   .SetEase(_movementEase)
                   .OnWaypointChange(reachedMovementPathIndex => HandleMovementPathWaypointReached(
                            reachedMovementPathIndex,
                            movementPath,
                            facingRotationDuration,
                            onWaypointReached
                        )
                    );

                await AwaitTweenAsync(moveTween, cancellationToken);

                EndRunLocomotion();
                await AwaitFacingRotationAsync(destinationFacingWorldPosition, facingRotationDuration, cancellationToken);
            }
            finally
            {
                if (this != null)
                {
                    EndRunLocomotion();
                }
            }
        }

        public async UniTask FaceTowardAsync(Vector3 worldPosition, float facingRotationDuration,
            CancellationToken cancellationToken)
        {
            EndRunLocomotion();
            await AwaitFacingRotationAsync(worldPosition, facingRotationDuration, cancellationToken);
        }

        private void HandleMovementPathWaypointReached(int reachedMovementPathIndex, Vector3[] movementPath,
            float facingRotationDuration, Action<int> onRouteWaypointReached)
        {
            if (MovementHelper.TryMapMovementPathIndexToRouteIndex(
                reachedMovementPathIndex,
                movementPath.Length,
                out int routeWaypointIndex
            ))
            {
                onRouteWaypointReached?.Invoke(routeWaypointIndex);
            }

            int nextMovementPathIndex = reachedMovementPathIndex + 1;

            if (nextMovementPathIndex < movementPath.Length)
            {
                StartFacingRotation(movementPath[nextMovementPathIndex], facingRotationDuration);
            }
        }

        private void BeginRunLocomotion()
        {
            _characterAnimationView.SetIsMoving(true);
            EnableFootstepsSfx();
        }

        private void EndRunLocomotion()
        {
            _characterAnimationView.SetIsMoving(false);
            DisableFootstepsSfx();
        }

        private void EnableFootstepsSfx()
        {
            _footstepsSfx.SetActive(true);
        }

        private void DisableFootstepsSfx()
        {
            _footstepsSfx.SetActive(false);
        }

        private Tween CreateFacingRotationTween(Vector3 worldTarget, float facingRotationDuration)
        {
            Vector3 travelDirection = MovementHelper.CalculatePlanarTravelDirection(worldTarget, _facingTransform.position);

            if (travelDirection.sqrMagnitude == 0f)
            {
                return null;
            }

            Transform facingParent = _facingTransform.parent;

            Quaternion? parentWorldRotation = facingParent != null
                ? facingParent.rotation
                : null;

            float targetLocalY = MovementHelper.CalculateLocalFacingY(
                worldTarget,
                _facingTransform.position,
                parentWorldRotation,
                _facingYawOffset
            );

            float currentLocalY = _facingTransform.localEulerAngles.y;
            float shortestTargetLocalY = currentLocalY + Mathf.DeltaAngle(currentLocalY, targetLocalY);

            _facingTransform.DOKill();

            return _facingTransform.DOLocalRotate(new Vector3(x: 0f, shortestTargetLocalY, z: 0f), facingRotationDuration);
        }

        private void StartFacingRotation(Vector3 worldTarget, float facingRotationDuration)
        {
            Tween facingTween = CreateFacingRotationTween(worldTarget, facingRotationDuration);

            facingTween?.SetEase(_facingRotationEase);
        }

        private async UniTask AwaitFacingRotationAsync(Vector3 worldTarget, float facingRotationDuration,
            CancellationToken cancellationToken)
        {
            Tween facingTween = CreateFacingRotationTween(worldTarget, facingRotationDuration);

            if (facingTween == null)
            {
                return;
            }

            facingTween.SetEase(_facingRotationEase);
            await AwaitTweenAsync(facingTween, cancellationToken);
        }

        private async UniTask AwaitTweenAsync(Tween tween, CancellationToken cancellationToken)
        {
            Transform movementTransform = transform;
            Transform facingTransform = _facingTransform;

            using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                this.GetCancellationTokenOnDestroy()
            );

            CancellationToken linkedToken = linkedCts.Token;

            await using CancellationTokenRegistration registration = linkedToken.Register(() =>
                {
                    KillRegisteredTweens(tween, movementTransform, facingTransform);
                }
            );

            while (tween.IsActive())
            {
                await UniTask.Yield();
            }

            linkedToken.ThrowIfCancellationRequested();
        }

        private void OnDestroy()
        {
            Transform movementTransform = transform;
            KillRegisteredTweens(tween: null, movementTransform, _facingTransform);
        }

        private void KillRegisteredTweens(Tween tween, Transform movementTransform, Transform facingTransform)
        {
            tween?.Kill();

            if (movementTransform != null)
            {
                movementTransform.DOKill();
            }

            if (facingTransform != null)
            {
                facingTransform.DOKill();
            }
        }
    }
}
