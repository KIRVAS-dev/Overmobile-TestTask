using System;
using UnityEngine;

namespace Input
{
    public interface IPlayerPointerInput
    {
        Vector2 ScreenPosition { get; }
        bool IsPressed { get; }
        event Action Pressed;
        event Action<PointerReleaseType> Released;
    }
}
