using Core.Gameplay.Movement;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using ViewComponents.Animation;

namespace ViewComponents.Movement
{
    public sealed class MovementView
        : MonoBehaviour,
          IMovementView
    {
        [SerializeField] private Transform _facingTransform;
        [SerializeField] private float _facingYawOffset = 180f;
        [SerializeField] private PathType _pathType = PathType.Linear;
        [SerializeField] private PathMode _pathMode = PathMode.TopDown2D;
        [SerializeField] private CharacterAnimationView _characterAnimationView;
        [SerializeField] private GameObject _footstepsSfx;

        public async UniTask MoveAlongPathAsync(IReadOnlyList<Vector3> pathPoints, float moveSpeed, float facingRotationDuration,
            Vector3 destinationFacingWorldPosition, CancellationToken cancellationToken)
        {
            if (_facingTransform == null)
            {
                throw new MovementFacingTransformMissingException(gameObject.name);
            }

            Vector3[] path = new Vector3[pathPoints.Count];

            for (int i = 0; i < pathPoints.Count; i++)
            {
                path[i] = pathPoints[i];
            }

            float duration = CalculatePathLength(path) / moveSpeed;

            BeginRunLocomotion();

            try
            {
                StartFacingRotation(path[0], facingRotationDuration);

                Tween moveTween = transform
                   .DOPath(path, duration, _pathType, _pathMode)
                   .SetEase(Ease.Linear)
                   .OnWaypointChange(waypointIndex =>
                        {
                            int nextWaypointIndex = waypointIndex + 1;

                            if (nextWaypointIndex < path.Length)
                            {
                                StartFacingRotation(path[nextWaypointIndex], facingRotationDuration);
                            }
                        }
                    );

                await AwaitTweenAsync(moveTween, cancellationToken);
                EndRunLocomotion();
                await AwaitFacingRotationAsync(destinationFacingWorldPosition, facingRotationDuration, cancellationToken);
            }
            finally
            {
                EndRunLocomotion();
            }
        }

        public async UniTask FaceTowardAsync(Vector3 worldPosition, float facingRotationDuration,
            CancellationToken cancellationToken)
        {
            if (_facingTransform == null)
            {
                throw new MovementFacingTransformMissingException(gameObject.name);
            }

            EndRunLocomotion();
            await AwaitFacingRotationAsync(worldPosition, facingRotationDuration, cancellationToken);
        }

        private void BeginRunLocomotion()
        {
            _characterAnimationView.SetIsMoving(true);
            SetFootstepsActive(true);
        }

        private void EndRunLocomotion()
        {
            _characterAnimationView.SetIsMoving(false);
            SetFootstepsActive(false);
        }

        private void SetFootstepsActive(bool isActive)
        {
            _footstepsSfx.SetActive(isActive);
        }

        private void StartFacingRotation(Vector3 worldTarget, float facingRotationDuration)
        {
            Tween facingTween = CreateFacingRotationTween(worldTarget, facingRotationDuration);

            facingTween?.SetEase(Ease.Linear);
        }

        private async UniTask AwaitFacingRotationAsync(Vector3 worldTarget, float facingRotationDuration,
            CancellationToken cancellationToken)
        {
            Tween facingTween = CreateFacingRotationTween(worldTarget, facingRotationDuration);

            if (facingTween == null)
            {
                return;
            }

            facingTween.SetEase(Ease.Linear);
            await AwaitTweenAsync(facingTween, cancellationToken);
        }

        private Tween CreateFacingRotationTween(Vector3 worldTarget, float facingRotationDuration)
        {
            Vector3 travelDirection = GetWorldTravelDirection(worldTarget);

            if (travelDirection.sqrMagnitude == 0f)
            {
                return null;
            }

            float targetLocalY = CalculateLocalFacingY(worldTarget);
            float currentLocalY = _facingTransform.localEulerAngles.y;
            float shortestTargetLocalY = currentLocalY + Mathf.DeltaAngle(currentLocalY, targetLocalY);

            _facingTransform.DOKill();

            return _facingTransform.DOLocalRotate(new Vector3(x: 0f, shortestTargetLocalY, z: 0f), facingRotationDuration);
        }

        private Vector3 GetWorldTravelDirection(Vector3 worldTarget)
        {
            Vector3 travelDirection = worldTarget - _facingTransform.position;
            travelDirection.z = 0f;

            return travelDirection;
        }

        private async UniTask AwaitTweenAsync(Tween tween, CancellationToken cancellationToken)
        {
            await using CancellationTokenRegistration registration = cancellationToken.Register(() =>
                {
                    transform.DOKill();
                    _facingTransform.DOKill();
                }
            );

            while (tween.IsActive())
            {
                await UniTask.Yield();
            }

            cancellationToken.ThrowIfCancellationRequested();
        }

        private float CalculateLocalFacingY(Vector3 worldTarget)
        {
            Vector3 worldDirection = GetWorldTravelDirection(worldTarget);

            Transform parent = _facingTransform.parent;

            if (parent == null)
            {
                return Mathf.Atan2(worldDirection.x, worldDirection.y) * Mathf.Rad2Deg + _facingYawOffset;
            }

            Vector3 localDirection = parent.InverseTransformDirection(worldDirection.normalized);
            localDirection.z = 0f;
            float localY = Mathf.Atan2(localDirection.x, localDirection.y) * Mathf.Rad2Deg;

            return localY + _facingYawOffset;
        }

        private float CalculatePathLength(IReadOnlyList<Vector3> path)
        {
            float length = 0f;
            Vector3 previous = transform.position;

            foreach (Vector3 pathPoint in path)
            {
                length += Vector3.Distance(previous, pathPoint);
                previous = pathPoint;
            }

            return length;
        }
    }
}
