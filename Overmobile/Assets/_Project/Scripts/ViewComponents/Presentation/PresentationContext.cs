using Core.Gameplay.Player;
using System.Threading;

namespace ViewComponents.Presentation
{
    public readonly struct PresentationContext
    {
        public PresentationContext(
            PresentationStepSequence owner,
            IActiveCharacterPresentationProvider activeCharacterPresentationProvider,
            IActivePresentationSectionMapProvider activePresentationSectionMapProvider,
            IPlayerUpgradeService playerUpgradeService,
            CancellationToken cancellationToken)
        {
            Owner = owner;
            ActiveCharacterPresentationProvider = activeCharacterPresentationProvider;
            ActivePresentationSectionMapProvider = activePresentationSectionMapProvider;
            PlayerUpgradeService = playerUpgradeService;
            CancellationToken = cancellationToken;
        }

        public PresentationStepSequence Owner { get; }
        public IActiveCharacterPresentationProvider ActiveCharacterPresentationProvider { get; }
        public IActivePresentationSectionMapProvider ActivePresentationSectionMapProvider { get; }
        public IPlayerUpgradeService PlayerUpgradeService { get; }
        public CancellationToken CancellationToken { get; }
    }
}
