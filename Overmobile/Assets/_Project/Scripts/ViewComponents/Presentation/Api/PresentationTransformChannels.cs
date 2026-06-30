using System;

namespace ViewComponents.Presentation
{
    [Flags]
    public enum PresentationTransformChannels
    {
        Position = 1,
        Rotation = 2,
        Scale = 4
    }
}
