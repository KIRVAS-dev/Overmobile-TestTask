using UnityEngine;

namespace Rendering.TargetSelection
{
    public static class TargetSelectionShaderProperties
    {
        public static readonly int FillColor = Shader.PropertyToID("_TargetSelectionFillColor");
        public static readonly int OutlineColor = Shader.PropertyToID("_TargetSelectionOutlineColor");
        public static readonly int Progress = Shader.PropertyToID("_TargetSelectionProgress");

        internal static readonly int FillTexture = Shader.PropertyToID("_TargetSelectionFill");
        internal static readonly int OutlineTexture = Shader.PropertyToID("_TargetSelectionOutline");
        internal static readonly int TargetDepthTexture = Shader.PropertyToID("_TargetSelectionTargetDepth");
        internal static readonly int OutlineThickness = Shader.PropertyToID("_TargetSelectionOutlineThickness");
    }
}
