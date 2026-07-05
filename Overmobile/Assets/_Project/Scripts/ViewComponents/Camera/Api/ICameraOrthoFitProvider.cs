using System;

namespace ViewComponents.Camera
{
    public interface ICameraOrthoFitProvider
    {
        event Action OrthoFitChanged;
        float FitOrthographicSize { get; }
        float MinOrthographicSize { get; }
        void RefreshOrthoFit();
    }
}
