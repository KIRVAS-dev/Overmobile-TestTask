using ExtendedExceptions;

namespace Rendering.VignetteBlur
{
    public sealed class MissingVignetteBlurMaterialException : ExtendedException
    {
        public MissingVignetteBlurMaterialException(string sourceName)
            : base("vignette-blur-1", $"Vignette blur material is not created for '{sourceName}'") { }
    }

    public sealed class MissingVignetteBlurShaderException : ExtendedException
    {
        public MissingVignetteBlurShaderException(string featureName)
            : base("vignette-blur-2", $"Vignette blur shader is not assigned on '{featureName}'") { }
    }

    public sealed class MissingVignetteVolumeException : ExtendedException
    {
        public MissingVignetteVolumeException(string cameraName)
            : base(
                "vignette-blur-3",
                $"Vignette volume component is required in the active volume stack for camera '{cameraName}'"
            ) { }
    }
}
