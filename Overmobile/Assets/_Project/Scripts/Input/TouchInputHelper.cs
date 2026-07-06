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

            int pressedTouchCount = 0;

            foreach (TouchControl touchControl in touchscreen.touches)
            {
                if (!touchControl.press.isPressed)
                {
                    continue;
                }

                pressedTouchCount++;

                if (pressedTouchCount >= 2)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetActivePinch(out Vector2 firstTouchPosition, out Vector2 secondTouchPosition)
        {
            firstTouchPosition = default;
            secondTouchPosition = default;

            Touchscreen touchscreen = Touchscreen.current;

            if (touchscreen == null)
            {
                return false;
            }

            int pressedTouchCount = 0;

            foreach (TouchControl touchControl in touchscreen.touches)
            {
                if (!touchControl.press.isPressed)
                {
                    continue;
                }

                if (pressedTouchCount == 0)
                {
                    firstTouchPosition = touchControl.position.ReadValue();
                }
                else
                {
                    secondTouchPosition = touchControl.position.ReadValue();

                    return true;
                }

                pressedTouchCount++;
            }

            return false;
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
    }
}
