using ExtendedExceptions;

namespace ViewComponents.Vfx
{
    public sealed class LoopingOneShotAnimatedVfxException : ExtendedException
    {
        public LoopingOneShotAnimatedVfxException(string gameObjectName)
            : base("vfx-1", $"One-shot animated VFX '{gameObjectName}' uses a looping animation clip") { }
    }
}
