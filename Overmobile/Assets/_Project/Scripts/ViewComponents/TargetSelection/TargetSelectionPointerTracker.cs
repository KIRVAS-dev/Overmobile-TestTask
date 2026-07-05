using Core;
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
        private readonly ITapIndicatorTargetClickArming _tapIndicatorTargetClickArming;
        private readonly List<Bind> _pointerBinds = new List<Bind>();
        private readonly ReactiveProperty<string> _hoveredEntityId = new ReactiveProperty<string>();

        public TargetSelectionPointerTracker(
            IPlayerPointerInput playerPointerInput,
            IGameplayInputBlock gameplayInputBlock,
            ITapIndicatorTargetClickArming tapIndicatorTargetClickArming)
        {
            _playerPointerInput = playerPointerInput;
            _gameplayInputBlock = gameplayInputBlock;
            _tapIndicatorTargetClickArming = tapIndicatorTargetClickArming;
        }

        void IDisposable.Dispose()
        {
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
                TryPlayDragOverTargetFeedback(targetSelectionView);
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
            pointerDownBind.OnTriggered += () => ApplyTargetPointerDownFeedback(targetSelectionView);
            pointerDownBind.Enable();
            _pointerBinds.Add(pointerDownBind);
        }

        private void ClearHoveredEntityId(string entityId)
        {
            if (_hoveredEntityId.Value != entityId)
            {
                return;
            }

            _hoveredEntityId.Value = null;
        }

        private void TryPlayDragOverTargetFeedback(TargetSelectionView targetSelectionView)
        {
            if (!_playerPointerInput.IsPressed)
            {
                return;
            }

            ApplyTargetPointerDownFeedback(targetSelectionView);
        }

        private void ApplyTargetPointerDownFeedback(TargetSelectionView targetSelectionView)
        {
            if (_gameplayInputBlock.IsBlocked.CurrentValue)
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
