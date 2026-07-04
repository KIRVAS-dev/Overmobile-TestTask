using Core;
using Core.Gameplay.Interaction;
using Core.Gameplay.Power;
using R3;
using System;
using System.Collections.Generic;

namespace ViewComponents.TargetSelection
{
    public sealed class TargetSelectionHighlightResolver : IDisposable
    {
        private readonly IPowerRegistry _powerRegistry;
        private readonly IEntityGuardAccessRegistry _guardAccessRegistry;
        private readonly IInteractionPhaseSource _interactionPhaseSource;
        private readonly IGameplayInputBlock _gameplayInputBlock;
        private readonly Dictionary<string, TargetSelectionView> _viewsByEntityId = new Dictionary<string, TargetSelectionView>();

        private IDisposable _globalVisibilitySubscription;
        private IDisposable _entityVisibilitySubscription;
        private TargetSelectionView _activeView;
        private string _hoveredEntityId;

        public TargetSelectionHighlightResolver(
            IPowerRegistry powerRegistry,
            IEntityGuardAccessRegistry guardAccessRegistry,
            IInteractionPhaseSource interactionPhaseSource,
            IGameplayInputBlock gameplayInputBlock)
        {
            _powerRegistry = powerRegistry;
            _guardAccessRegistry = guardAccessRegistry;
            _interactionPhaseSource = interactionPhaseSource;
            _gameplayInputBlock = gameplayInputBlock;
        }

        void IDisposable.Dispose()
        {
            _globalVisibilitySubscription?.Dispose();
            _globalVisibilitySubscription = null;

            _entityVisibilitySubscription?.Dispose();
            _entityVisibilitySubscription = null;

            if (_activeView == null)
            {
                return;
            }

            _activeView.Hide();
            _activeView = null;
        }

        public void RegisterView(string entityId, TargetSelectionView targetSelectionView)
        {
            _viewsByEntityId[entityId] = targetSelectionView;
        }

        public void Start(ReadOnlyReactiveProperty<string> hoveredEntityId)
        {
            _globalVisibilitySubscription = _interactionPhaseSource
               .CurrentPhase
               .CombineLatest(
                    _interactionPhaseSource.CurrentTargetEntityId,
                    _gameplayInputBlock.IsBlocked,
                    hoveredEntityId,
                    static (
                        _,
                        _,
                        _,
                        hoveredId) => hoveredId
                )
               .Subscribe(hoveredId =>
                    {
                        _hoveredEntityId = hoveredId;
                        RefreshHighlightVisibility();
                    }
                );
        }

        private void RefreshHighlightVisibility()
        {
            _entityVisibilitySubscription?.Dispose();
            _entityVisibilitySubscription = null;

            string highlightEntityId = TargetSelectionHighlightRules.ResolveHighlightEntityId(
                _interactionPhaseSource.CurrentPhase.CurrentValue,
                _interactionPhaseSource.CurrentTargetEntityId.CurrentValue,
                _hoveredEntityId
            );

            if (highlightEntityId == null)
            {
                UpdateActiveView(view: null, shouldShow: false);

                return;
            }

            IPowerEntity powerEntity = _powerRegistry.Get(highlightEntityId);
            TargetSelectionView highlightView = _viewsByEntityId[highlightEntityId];

            _entityVisibilitySubscription = BuildShouldShowObservable(highlightEntityId, powerEntity)
               .Subscribe(shouldShow => UpdateActiveView(highlightView, shouldShow));
        }

        private Observable<bool> BuildShouldShowObservable(string entityId, IPowerEntity powerEntity)
        {
            ReadOnlyReactiveProperty<bool> isResolved = powerEntity.IsResolved;
            ReadOnlyReactiveProperty<InteractionPhase> currentPhase = _interactionPhaseSource.CurrentPhase;
            ReadOnlyReactiveProperty<string> currentTargetEntityId = _interactionPhaseSource.CurrentTargetEntityId;
            ReadOnlyReactiveProperty<bool> isBlocked = _gameplayInputBlock.IsBlocked;

            if (!_guardAccessRegistry.HasGuards(entityId))
            {
                return isResolved.CombineLatest(
                    currentPhase,
                    currentTargetEntityId,
                    isBlocked,
                    (
                        resolved,
                        phase,
                        interactionTargetEntityId,
                        blocked) => TargetSelectionHighlightRules.IsHighlightVisible(
                        phase,
                        interactionTargetEntityId,
                        entityId,
                        resolved,
                        areGuardsBlocking: false,
                        blocked
                    )
                );
            }

            {
                ReadOnlyReactiveProperty<bool> areGuardsBlocking = _guardAccessRegistry.GetAreGuardsBlocking(entityId);

                return isResolved.CombineLatest(
                    currentPhase,
                    currentTargetEntityId,
                    areGuardsBlocking,
                    isBlocked,
                    (
                        resolved,
                        phase,
                        interactionTargetEntityId,
                        guardsBlocking,
                        blocked) => TargetSelectionHighlightRules.IsHighlightVisible(
                        phase,
                        interactionTargetEntityId,
                        entityId,
                        resolved,
                        guardsBlocking,
                        blocked
                    )
                );
            }
        }

        private void UpdateActiveView(TargetSelectionView view, bool shouldShow)
        {
            if (shouldShow)
            {
                if (_activeView != null
                 && _activeView != view)
                {
                    _activeView.Hide();
                }

                if (_activeView == view)
                {
                    return;
                }

                view.Show();
                _activeView = view;

                return;
            }

            if (_activeView == null)
            {
                return;
            }

            _activeView.Hide();
            _activeView = null;
        }
    }
}
