using System;
using UnityEngine;

namespace ViewComponents.Presentation
{
    [Serializable]
    public sealed class PresentationSectionBinding
    {
        [SerializeField] private PresentationSectionKey _key;
        [SerializeField] private PresentationStepSequence _sequence;

        public PresentationSectionKey Key => _key;

        public PresentationStepSequence Sequence => _sequence;
    }
}
