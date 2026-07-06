using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Input
{
    public static class PointerIntentHelper
    {
        public static bool ShouldUseInstantConfirm(PlayerPointerInputConfig config, InputControl pressControl)
        {
            return config.MouseUsesInstantConfirm && pressControl.parent is not TouchControl;
        }

        public static bool IsSlopExceeded(
            Vector2 pendingScreenPosition,
            Vector2 currentScreenPosition,
            float touchSlopPixels)
        {
            return Vector2.Distance(pendingScreenPosition, currentScreenPosition) > touchSlopPixels;
        }

        public static Vector2 ReadScreenPosition(InputAction pointAction, InputControl activePressControl)
        {
            if (activePressControl == null)
            {
                throw new ActivePlayerPointerNotAssignedException();
            }

            return TouchInputHelper.TryGetTouchPosition(activePressControl, out Vector2 touchPosition)
                ? touchPosition
                : pointAction.ReadValue<Vector2>();
        }
    }
}
