using UnityEngine;
using UnityCamera = UnityEngine.Camera;

namespace ViewComponents.Camera
{
    public static class CameraConfinerHelper
    {
        public readonly struct OrthoFitSizes
        {
            public OrthoFitSizes(float fitOrthographicSize, float minOrthographicSize)
            {
                FitOrthographicSize = fitOrthographicSize;
                MinOrthographicSize = minOrthographicSize;
            }

            public float FitOrthographicSize { get; }
            public float MinOrthographicSize { get; }
        }

        public static bool IsLandscape(int screenWidth, int screenHeight)
        {
            return screenWidth > screenHeight;
        }

        public static float GetScreenAspect(int screenWidth, int screenHeight)
        {
            return (float)screenWidth / screenHeight;
        }

        public static BoxCollider2D GetBoundsForOrientation(
            BoxCollider2D portraitBounds,
            BoxCollider2D landscapeBounds,
            bool isLandscape)
        {
            return isLandscape
                ? landscapeBounds
                : portraitBounds;
        }

        public static OrthoFitSizes CalculateOrthoFit(
            Bounds bounds,
            float screenAspect,
            float introStartToTargetRatio)
        {
            float fitOrthographicSize = CameraZoomHelper.CalculateFitOrthographicSize(bounds, screenAspect);
            float minOrthographicSize = CameraZoomHelper.CalculateMinOrthographicSize(
                fitOrthographicSize,
                introStartToTargetRatio
            );

            return new OrthoFitSizes(fitOrthographicSize, minOrthographicSize);
        }

        public static float RescaleAndClampOrthographicSize(
            float currentOrthographicSize,
            float previousFitOrthographicSize,
            float newFitOrthographicSize,
            float minOrthographicSize)
        {
            float rescaledOrthographicSize = CameraZoomHelper.RescaleOrthographicSizeForFitChange(
                currentOrthographicSize,
                previousFitOrthographicSize,
                newFitOrthographicSize
            );

            return CameraZoomHelper.ClampOrthographicSize(
                rescaledOrthographicSize,
                minOrthographicSize,
                newFitOrthographicSize
            );
        }

        public static float GetClampAspect(UnityCamera outputCamera, float screenAspect)
        {
            if (outputCamera != null)
            {
                return outputCamera.aspect;
            }

            return screenAspect;
        }

        public static Vector3 CalculateBoundsFollowPosition(
            Vector3 restBoundsPosition,
            Vector3 followPosition,
            Vector3 referencePosition)
        {
            Vector3 offset = new(
                followPosition.x - referencePosition.x,
                followPosition.y - referencePosition.y,
                0f
            );

            return restBoundsPosition + offset;
        }
    }
}
