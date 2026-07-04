namespace Rendering.TargetSelection
{
    public static class TargetSelectionRenderingLayers
    {
        private const int MeshLayerIndex = 8;
        private const int SpriteLayerIndex = 9;

        public const uint Mesh = 1u << MeshLayerIndex;
        public const uint Sprite = 1u << SpriteLayerIndex;
    }
}
