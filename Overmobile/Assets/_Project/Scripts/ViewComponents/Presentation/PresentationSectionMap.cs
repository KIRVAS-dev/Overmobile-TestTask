using Cysharp.Threading.Tasks;
using ExtendedExceptions;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ViewComponents.Presentation
{
    [DisallowMultipleComponent]
    public sealed class PresentationSectionMap : MonoBehaviour
    {
        [SerializeField] private PresentationSectionBinding[] _sections = Array.Empty<PresentationSectionBinding>();

        public async UniTask PlaySectionAsync(PresentationSectionKey key, CancellationToken cancellationToken)
        {
            PresentationStepSequence sequence = ResolveSection(key);

            await sequence.StartPresentationAsync(cancellationToken);
        }

        private void Awake()
        {
            Validate();
        }

        private PresentationStepSequence ResolveSection(PresentationSectionKey key)
        {
            foreach (PresentationSectionBinding section in _sections)
            {
                if (section.Key != key)
                {
                    continue;
                }

                return section.Sequence;
            }

            throw new InvalidPresentationSectionMapException(gameObject.name, $"Section '{key}' is not assigned");
        }

        private void Validate()
        {
            Guard.AgainstNullOrEmpty(
                _sections,
                () => new InvalidPresentationSectionMapException(gameObject.name, "Sections are not assigned")
            );

            HashSet<PresentationSectionKey> assignedKeys = new HashSet<PresentationSectionKey>();

            for (int i = 0; i < _sections.Length; i++)
            {
                PresentationSectionBinding section = _sections[i];

                if (section == null)
                {
                    throw new InvalidPresentationSectionMapException(gameObject.name, $"Section at index {i} is missing");
                }

                if (!assignedKeys.Add(section.Key))
                {
                    throw new InvalidPresentationSectionMapException(gameObject.name, $"Duplicate section key '{section.Key}'");
                }

                if (section.Sequence == null)
                {
                    throw new InvalidPresentationSectionMapException(
                        gameObject.name,
                        $"Section '{section.Key}' sequence is not assigned"
                    );
                }
            }
        }
    }
}
