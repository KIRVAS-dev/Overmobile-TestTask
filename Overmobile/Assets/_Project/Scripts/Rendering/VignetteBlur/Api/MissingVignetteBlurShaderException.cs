using ExtendedExceptions;

namespace Rendering.VignetteBlur.Api
{
    public sealed class MissingVignetteBlurShaderException : ExtendedException
    {
        public MissingVignetteBlurShaderException(string featureName)
            : base(
                "Missing_vignette_blur_shader",
                $"Vignette blur shader is not assigned on '{featureName}'."
            ) { }
    }
}
