namespace ViewComponents.Presentation
{
    public interface IActivePresentationSectionMapProvider
    {
        void Register(PresentationSectionMap presentationSectionMap);
        void PlaySection(PresentationSectionKey sectionKey);
    }
}
