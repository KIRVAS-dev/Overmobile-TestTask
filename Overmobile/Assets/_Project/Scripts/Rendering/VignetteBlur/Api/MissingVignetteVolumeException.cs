using ExtendedExceptions;

namespace Rendering.VignetteBlur.Api
{
    public sealed class MissingVignetteVolumeException : ExtendedException
    {
        public MissingVignetteVolumeException(string cameraName)
            : base(
                "Missing_vignette_volume",
                $"Vignette volume component is required in the active volume stack for camera '{cameraName}'."
            ) { }
    }
}
