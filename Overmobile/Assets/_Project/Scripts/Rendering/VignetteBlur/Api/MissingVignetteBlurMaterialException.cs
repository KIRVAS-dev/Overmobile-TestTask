using ExtendedExceptions;

namespace Rendering.VignetteBlur.Api
{
    public sealed class MissingVignetteBlurMaterialException : ExtendedException
    {
        public MissingVignetteBlurMaterialException(string sourceName)
            : base(
                "Missing_vignette_blur_material",
                $"Vignette blur material is not created for '{sourceName}'."
            ) { }
    }
}
