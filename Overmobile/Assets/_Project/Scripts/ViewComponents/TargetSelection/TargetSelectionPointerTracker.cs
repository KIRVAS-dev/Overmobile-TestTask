using Core;
using Core.Gameplay.Interaction;
using Input;
using Input.Binds;
using R3;
using System;
using System.Collections.Generic;
using ViewComponents.Interaction;

namespace ViewComponents.TargetSelection
{
    public sealed class TargetSelectionPointerTracker : IDisposable
    {
        private readonly IPlayerPointerInput _playerPointerInput;
        private readonly IGameplayInputBlock _gameplayInputBlock;
        private readonly IInteractionService _interactionService;
        private readonly ITapIndicatorTargetClickArming _tapIndicatorTargetClickArming;
        private readonly List<Bind> _pointerBinds = new List<Bind>();
        private readonly ReactiveProperty<string> _hoveredEntityId = new ReactiveProperty<string>();

        public TargetSelectionPointerTracker(
            IPlayerPointerInput playerPointerInput,
            IGameplayInputBlock gameplayInputBlock,
            IInteractionService interactionService,
            ITapIndicatorTargetClickArming tapIndicatorTargetClickArming)
        {
            _playerPointerInput = playerPointerInput;
            _gameplayInputBlock = gameplayInputBlock;
            _interactionService = interactionService;
            _tapIndicatorTargetClickArming = tapIndicatorTargetClickArming;

            _playerPointerInput.Released += OnPointerReleased;
        }

        void IDisposable.Dispose()
        {
            _playerPointerInput.Released -= OnPointerReleased;

            foreach (Bind pointerBind in _pointerBinds)
            {
                pointerBind.Disable();
            }

            _pointerBinds.Clear();
            _hoveredEntityId.Dispose();
        }

        public ReadOnlyReactiveProperty<string> HoveredEntityId => _hoveredEntityId;

        public void Track(InteractableTarget interactableTarget, TargetSelectionView targetSelectionView)
        {
            string entityId = interactableTarget.EntityId;

            Bind pointerEnterBind = new Bind(interactableTarget.PointArea.PointerEnter);

            pointerEnterBind.OnTriggered += () =>
            {
                _hoveredEntityId.Value = entityId;
                TryPlayDragOverTargetFeedback(entityId, targetSelectionView);
            };

            pointerEnterBind.Enable();
            _pointerBinds.Add(pointerEnterBind);

            Bind pointerExitBind = new Bind(interactableTarget.PointArea.PointerExit);

            pointerExitBind.OnTriggered += () =>
            {
                if (_playerPointerInput.IsPressed)
                {
                    DisarmTargetClickRelease();
                }

                ClearHoveredEntityId(entityId);
            };

            pointerExitBind.Enable();
            _pointerBinds.Add(pointerExitBind);

            Bind pointerDownBind = new Bind(interactableTarget.PointArea.PointerDown);
            pointerDownBind.OnTriggered += () => ApplyTargetPointerDownFeedback(entityId, targetSelectionView);
            pointerDownBind.Enable();
            _pointerBinds.Add(pointerDownBind);
        }

        private void OnPointerReleased(PointerReleaseType releaseType)
        {
            if (releaseType != PointerReleaseType.MultiTouch)
            {
                return;
            }

            DisarmTargetClickRelease();
            _hoveredEntityId.Value = null;
        }

        private void ClearHoveredEntityId(string entityId)
        {
            if (_hoveredEntityId.Value != entityId)
            {
                return;
            }

            _hoveredEntityId.Value = null;
        }

        private void TryPlayDragOverTargetFeedback(string entityId, TargetSelectionView targetSelectionView)
        {
            if (!_playerPointerInput.IsPressed)
            {
                return;
            }

            ApplyTargetPointerDownFeedback(entityId, targetSelectionView);
        }

        private void ApplyTargetPointerDownFeedback(string entityId, TargetSelectionView targetSelectionView)
        {
            if (_gameplayInputBlock.IsBlocked.CurrentValue
             || !_interactionService.CanInteract(entityId))
            {
                return;
            }

            _tapIndicatorTargetClickArming.ArmTargetClickRelease();
            targetSelectionView.PlayPointerDownSfx();
        }

        private void DisarmTargetClickRelease()
        {
            if (_gameplayInputBlock.IsBlocked.CurrentValue)
            {
                return;
            }

            _tapIndicatorTargetClickArming.DisarmTargetClickRelease();
        }
    }
}
