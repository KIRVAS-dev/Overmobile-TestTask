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
        private readonly List<Bind> _pointerBinds = new List<Bind>();
        private readonly ReactiveProperty<string> _hoveredEntityId = new ReactiveProperty<string>();

        public TargetSelectionPointerTracker(IPlayerPointerInput playerPointerInput, IGameplayInputBlock gameplayInputBlock)
        {
            _playerPointerInput = playerPointerInput;
            _gameplayInputBlock = gameplayInputBlock;
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
                TryPlayDragSelectionSfx(targetSelectionView);
            };

            pointerEnterBind.Enable();
            _pointerBinds.Add(pointerEnterBind);

            Bind pointerExitBind = new Bind(interactableTarget.PointArea.PointerExit);
            pointerExitBind.OnTriggered += () => ClearHoveredEntityId(entityId);
            pointerExitBind.Enable();
            _pointerBinds.Add(pointerExitBind);

            Bind pointerDownBind = new Bind(interactableTarget.PointArea.PointerDown);
            pointerDownBind.OnTriggered += () => PlayPointerDownSfx(targetSelectionView);
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

        private void TryPlayDragSelectionSfx(TargetSelectionView targetSelectionView)
        {
            if (!_playerPointerInput.IsPressed)
            {
                return;
            }

            PlayPointerDownSfx(targetSelectionView);
        }

        private void PlayPointerDownSfx(TargetSelectionView targetSelectionView)
        {
            if (_gameplayInputBlock.IsBlocked.CurrentValue)
            {
                return;
            }

            targetSelectionView.PlayPointerDownSfx();
        }
    }
}
