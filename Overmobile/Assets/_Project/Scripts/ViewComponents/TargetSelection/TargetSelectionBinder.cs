using Core;
using Core.Gameplay.Interaction;
using Core.Gameplay.Power;
using Input;
using System;
using ViewComponents.Interaction;

namespace ViewComponents.TargetSelection
{
    public sealed class TargetSelectionBinder
        : ITargetSelectionBinder,
          IDisposable
    {
        private readonly InteractableTargetProvider _interactableTargetProvider;
        private readonly TargetSelectionPointerTracker _pointerTracker;
        private readonly TargetSelectionHighlightResolver _highlightResolver;

        public TargetSelectionBinder(
            IPowerRegistry powerRegistry,
            IEntityGuardAccessRegistry guardAccessRegistry,
            IInteractionPhaseSource interactionPhaseSource,
            IGameplayInputBlock gameplayInputBlock,
            IPlayerPointerInput playerPointerInput,
            IInteractionService interactionService,
            ITapIndicatorTargetClickArming tapIndicatorTargetClickArming,
            InteractableTargetProvider interactableTargetProvider)
        {
            _interactableTargetProvider = interactableTargetProvider;

            _pointerTracker = new TargetSelectionPointerTracker(
                playerPointerInput,
                gameplayInputBlock,
                interactionService,
                tapIndicatorTargetClickArming
            );

            _highlightResolver = new TargetSelectionHighlightResolver(
                powerRegistry,
                guardAccessRegistry,
                interactionPhaseSource,
                gameplayInputBlock
            );
        }

        public void BindTargetSelection()
        {
            foreach (InteractableTarget interactableTarget in _interactableTargetProvider.InteractableTargets)
            {
                TargetSelectionView targetSelectionView = interactableTarget.GetComponent<TargetSelectionView>();

                if (targetSelectionView == null)
                {
                    throw new MissingTargetSelectionViewException(interactableTarget.gameObject.name);
                }

                _highlightResolver.RegisterView(interactableTarget.EntityId, targetSelectionView);
                _pointerTracker.Track(interactableTarget, targetSelectionView);
            }

            _highlightResolver.Start(_pointerTracker.HoveredEntityId);
        }

        void IDisposable.Dispose()
        {
            ((IDisposable)_highlightResolver).Dispose();
            ((IDisposable)_pointerTracker).Dispose();
        }
    }
}
