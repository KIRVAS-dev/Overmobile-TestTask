using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Input
{
    public static class TouchInputHelper
    {
        public static bool IsMultiTouchActive()
        {
            Touchscreen touchscreen = Touchscreen.current;

            if (touchscreen == null)
            {
                return false;
            }

            int inProgressTouchCount = 0;

            foreach (TouchControl touchControl in touchscreen.touches)
            {
                if (!touchControl.isInProgress)
                {
                    continue;
                }

                inProgressTouchCount++;

                if (inProgressTouchCount >= 2)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetActivePinch(
            ref int? firstTouchId,
            ref int? secondTouchId,
            out Vector2 firstTouchPosition,
            out Vector2 secondTouchPosition)
        {
            firstTouchPosition = default;
            secondTouchPosition = default;

            Touchscreen touchscreen = Touchscreen.current;

            if (touchscreen == null)
            {
                ClearPinchTouchIds(out firstTouchId, out secondTouchId);

                return false;
            }

            if (firstTouchId.HasValue
             && secondTouchId.HasValue)
            {
                ScanTrackedPinchTouches(
                    touchscreen,
                    firstTouchId.Value,
                    secondTouchId.Value,
                    out firstTouchPosition,
                    out secondTouchPosition,
                    out bool foundTrackedFirst,
                    out bool foundTrackedSecond,
                    out int inProgressTouchCount
                );

                if (foundTrackedFirst && foundTrackedSecond)
                {
                    return true;
                }

                if (foundTrackedFirst || foundTrackedSecond)
                {
                    if (inProgressTouchCount < 2)
                    {
                        return false;
                    }

                    ClearPinchTouchIds(out firstTouchId, out secondTouchId);
                }
                else
                {
                    ClearPinchTouchIds(out firstTouchId, out secondTouchId);
                }
            }

            if (!TrySelectPinchTouchPair(
                touchscreen,
                out int newFirstTouchId,
                out int newSecondTouchId,
                out firstTouchPosition,
                out secondTouchPosition
            ))
            {
                ClearPinchTouchIds(out firstTouchId, out secondTouchId);

                return false;
            }

            firstTouchId = newFirstTouchId;
            secondTouchId = newSecondTouchId;

            return true;
        }

        public static bool TryGetTouchPosition(InputControl pressControl, out Vector2 screenPosition)
        {
            screenPosition = default;

            if (pressControl.parent is not TouchControl touchControl)
            {
                return false;
            }

            screenPosition = touchControl.position.ReadValue();

            return true;
        }

        private static void ScanTrackedPinchTouches(
            Touchscreen touchscreen,
            int trackedFirstTouchId,
            int trackedSecondTouchId,
            out Vector2 firstTouchPosition,
            out Vector2 secondTouchPosition,
            out bool foundTrackedFirst,
            out bool foundTrackedSecond,
            out int inProgressTouchCount)
        {
            firstTouchPosition = default;
            secondTouchPosition = default;
            foundTrackedFirst = false;
            foundTrackedSecond = false;
            inProgressTouchCount = 0;

            foreach (TouchControl touchControl in touchscreen.touches)
            {
                if (!touchControl.isInProgress)
                {
                    continue;
                }

                inProgressTouchCount++;

                int touchId = touchControl.touchId.ReadValue();
                Vector2 position = touchControl.position.ReadValue();

                if (touchId == trackedFirstTouchId)
                {
                    firstTouchPosition = position;
                    foundTrackedFirst = true;

                    continue;
                }

                if (touchId == trackedSecondTouchId)
                {
                    secondTouchPosition = position;
                    foundTrackedSecond = true;
                }
            }
        }

        private static bool TrySelectPinchTouchPair(
            Touchscreen touchscreen,
            out int firstTouchId,
            out int secondTouchId,
            out Vector2 firstTouchPosition,
            out Vector2 secondTouchPosition)
        {
            firstTouchId = 0;
            secondTouchId = 0;
            firstTouchPosition = default;
            secondTouchPosition = default;

            int? lowestTouchId = null;
            int? secondLowestTouchId = null;
            Vector2 lowestTouchPosition = default;
            Vector2 secondLowestTouchPosition = default;

            foreach (TouchControl touchControl in touchscreen.touches)
            {
                if (!touchControl.isInProgress)
                {
                    continue;
                }

                int touchId = touchControl.touchId.ReadValue();
                Vector2 position = touchControl.position.ReadValue();

                if (!lowestTouchId.HasValue
                 || touchId < lowestTouchId.Value)
                {
                    if (lowestTouchId.HasValue)
                    {
                        secondLowestTouchId = lowestTouchId;
                        secondLowestTouchPosition = lowestTouchPosition;
                    }

                    lowestTouchId = touchId;
                    lowestTouchPosition = position;
                }
                else if (!secondLowestTouchId.HasValue
                 || touchId < secondLowestTouchId.Value)
                {
                    secondLowestTouchId = touchId;
                    secondLowestTouchPosition = position;
                }
            }

            if (!lowestTouchId.HasValue
             || !secondLowestTouchId.HasValue)
            {
                return false;
            }

            firstTouchId = lowestTouchId.Value;
            secondTouchId = secondLowestTouchId.Value;
            firstTouchPosition = lowestTouchPosition;
            secondTouchPosition = secondLowestTouchPosition;

            return true;
        }

        private static void ClearPinchTouchIds(out int? firstTouchId, out int? secondTouchId)
        {
            firstTouchId = null;
            secondTouchId = null;
        }
    }
}
