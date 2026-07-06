using Input;
using UnityEngine;
using UnityCamera = UnityEngine.Camera;

namespace ViewComponents.Camera
{
    public static class CameraZoomHelper
    {
        private const float ORTHOGRAPHIC_FULL_SIZE_MULTIPLIER = 2f;
        private const float VIEWPORT_CENTER = 0.5f;
        private const float MIDPOINT_FACTOR = 0.5f;

        public static bool TryGetActivePinch(
            ref int? firstTouchId,
            ref int? secondTouchId,
            out Vector2 firstTouchPosition,
            out Vector2 secondTouchPosition)
        {
            return TouchInputHelper.TryGetActivePinch(
                ref firstTouchId,
                ref secondTouchId,
                out firstTouchPosition,
                out secondTouchPosition
            );
        }

        public static Vector2 GetPinchMidpoint(Vector2 firstTouchPosition, Vector2 secondTouchPosition)
        {
            return (firstTouchPosition + secondTouchPosition) * MIDPOINT_FACTOR;
        }

        public static float CalculatePinchTargetOrthographicSize(
            float currentOrthographicSize,
            float pinchDelta,
            float pinchSensitivity)
        {
            return currentOrthographicSize - pinchDelta * pinchSensitivity;
        }

        public static float CalculateScrollTargetOrthographicSize(
            float currentOrthographicSize,
            float scrollDelta,
            float scrollSensitivity)
        {
            return currentOrthographicSize - scrollDelta * scrollSensitivity;
        }

        public static float ClampOrthographicSize(
            float targetOrthographicSize,
            float minOrthographicSize,
            float maxOrthographicSize)
        {
            return Mathf.Clamp(targetOrthographicSize, minOrthographicSize, maxOrthographicSize);
        }

        public static float CalculateFitOrthographicSize(Bounds bounds, float aspect)
        {
            float heightLimit = bounds.extents.y;
            float widthLimit = bounds.extents.x / aspect;

            return Mathf.Min(heightLimit, widthLimit);
        }

        public static float CalculateMinOrthographicSize(float fitOrthographicSize, float introStartToTargetRatio)
        {
            return fitOrthographicSize * introStartToTargetRatio;
        }

        public static float RescaleOrthographicSizeForFitChange(
            float currentOrthographicSize,
            float previousFitOrthographicSize,
            float newFitOrthographicSize)
        {
            if (previousFitOrthographicSize <= 0f
             || Mathf.Approximately(previousFitOrthographicSize, newFitOrthographicSize))
            {
                return currentOrthographicSize;
            }

            float scale = newFitOrthographicSize / previousFitOrthographicSize;

            return currentOrthographicSize * scale;
        }

        public static Vector3 ConvertScreenPointToWorld(
            UnityCamera outputCamera,
            Transform cameraTransform,
            Vector2 focalScreenPoint,
            float orthographicSize)
        {
            Vector3 viewportPoint = outputCamera.ScreenToViewportPoint(focalScreenPoint);
            float planeDistance = GetScreenToWorldPlaneDistance(cameraTransform);
            float horizontalWorldOffset = GetViewportAxisWorldOffset(viewportPoint.x, orthographicSize, outputCamera.aspect);
            float verticalWorldOffset = GetViewportAxisWorldOffset(viewportPoint.y, orthographicSize, aspect: 1f);

            Vector3 worldOffset = cameraTransform.right * horizontalWorldOffset + cameraTransform.up * verticalWorldOffset;

            return cameraTransform.position + worldOffset + cameraTransform.forward * planeDistance;
        }

        private static float GetViewportAxisWorldOffset(
            float viewportAxis,
            float orthographicSize,
            float aspect)
        {
            return (viewportAxis - VIEWPORT_CENTER) * ORTHOGRAPHIC_FULL_SIZE_MULTIPLIER * orthographicSize * aspect;
        }

        private static float GetScreenToWorldPlaneDistance(Transform cameraTransform)
        {
            return Mathf.Abs(cameraTransform.position.z);
        }

        public static float SmoothOrthographicSize(
            float currentOrthographicSize,
            float targetOrthographicSize,
            float smoothSpeed,
            float deltaTime)
        {
            return Mathf.Lerp(currentOrthographicSize, targetOrthographicSize, smoothSpeed * deltaTime);
        }

        public static Vector3 CalculateFocalPointPositionOffset(Vector3 worldPointBeforeZoom, Vector3 worldPointAfterZoom)
        {
            return worldPointBeforeZoom - worldPointAfterZoom;
        }

        public static Vector3 ClampCameraPositionToBounds(
            Vector3 cameraPosition,
            Bounds bounds,
            float orthographicSize,
            float aspect)
        {
            float halfWidth = orthographicSize * aspect;
            float halfHeight = orthographicSize;
            float minX = bounds.min.x + halfWidth;
            float maxX = bounds.max.x - halfWidth;
            float minY = bounds.min.y + halfHeight;
            float maxY = bounds.max.y - halfHeight;

            float clampedX = minX > maxX
                ? bounds.center.x
                : Mathf.Clamp(cameraPosition.x, minX, maxX);

            float clampedY = minY > maxY
                ? bounds.center.y
                : Mathf.Clamp(cameraPosition.y, minY, maxY);

            return new Vector3(clampedX, clampedY, cameraPosition.z);
        }
    }
}
