using Cysharp.Threading.Tasks;

namespace ViewComponents.Presentation.Player
{
    public sealed class ActivePresentationSectionMapProvider : IActivePresentationSectionMapProvider
    {
        private PresentationSectionMap _presentationSectionMap;

        public void Register(PresentationSectionMap presentationSectionMap)
        {
            _presentationSectionMap = presentationSectionMap
             ?? throw new InvalidPresentationSectionMapException(
                    string.Empty,
                    "Presentation section map is not assigned"
                );
        }

        public void PlaySection(PresentationSectionKey sectionKey)
        {
            PresentationSectionMap activePresentationSectionMap = ResolvePresentationSectionMap();

            activePresentationSectionMap
               .PlaySectionAsync(sectionKey, activePresentationSectionMap.GetCancellationTokenOnDestroy())
               .Forget();
        }

        private PresentationSectionMap ResolvePresentationSectionMap()
        {
            return _presentationSectionMap
             ?? throw new InvalidPresentationSectionMapException(
                    string.Empty,
                    "Active presentation section map is not registered"
                );
        }
    }
}
