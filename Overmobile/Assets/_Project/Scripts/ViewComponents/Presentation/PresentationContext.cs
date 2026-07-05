using Core.Gameplay.Player;
using System.Threading;
using ViewComponents.Camera;

namespace ViewComponents.Presentation
{
    public readonly struct PresentationContext
    {
        public PresentationContext(
            PresentationStepSequence owner,
            ICameraShakeView cameraShakeView,
            IActiveCharacterPresentationProvider activeCharacterPresentationProvider,
            IActivePresentationSectionMapProvider activePresentationSectionMapProvider,
            IPlayerUpgradeService playerUpgradeService,
            CancellationToken cancellationToken)
        {
            Owner = owner;
            CameraShakeView = cameraShakeView;
            ActiveCharacterPresentationProvider = activeCharacterPresentationProvider;
            ActivePresentationSectionMapProvider = activePresentationSectionMapProvider;
            PlayerUpgradeService = playerUpgradeService;
            CancellationToken = cancellationToken;
        }

        public PresentationStepSequence Owner { get; }
        public ICameraShakeView CameraShakeView { get; }
        public IActiveCharacterPresentationProvider ActiveCharacterPresentationProvider { get; }
        public IActivePresentationSectionMapProvider ActivePresentationSectionMapProvider { get; }
        public IPlayerUpgradeService PlayerUpgradeService { get; }
        public CancellationToken CancellationToken { get; }
    }
}
