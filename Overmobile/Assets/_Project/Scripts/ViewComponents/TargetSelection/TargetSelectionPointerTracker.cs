using Core;
using Core.Gameplay.Interaction;
using Input;
using Input.Binds;
using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ViewComponents.Interaction;

namespace ViewComponents.TargetSelection
{
    public sealed class TargetSelectionPointerTracker : IDisposable
    {
        private readonly IPlayerPointerInput _playerPointerInput;
        private readonly IGameplayInputBlock _gameplayInputBlock;
        private readonly IInteractionService _interactionService;
        private readonly ITapIndicatorTargetClickArming _tapIndicatorTargetClickArming;
        private readonly Dictionary<string, InteractableTarget> _interactableTargetsByEntityId =
            new Dictionary<string, InteractableTarget>();
        private readonly List<Bind> _pointerBinds = new List<Bind>();
        private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();
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

            _playerPointerInput.Pressed += OnConfirmedPress;
            _playerPointerInput.Released += OnPointerReleased;
        }

        void IDisposable.Dispose()
        {
            _playerPointerInput.Pressed -= OnConfirmedPress;
            _playerPointerInput.Released -= OnPointerReleased;

            foreach (Bind pointerBind in _pointerBinds)
            {
                pointerBind.Disable();
            }

            _pointerBinds.Clear();
            _interactableTargetsByEntityId.Clear();
            _hoveredEntityId.Dispose();
        }

        public ReadOnlyReactiveProperty<string> HoveredEntityId => _hoveredEntityId;

        public void Track(InteractableTarget interactableTarget, TargetSelectionView targetSelectionView)
        {
            string entityId = interactableTarget.EntityId;

            _interactableTargetsByEntityId[entityId] = interactableTarget;

            Bind pointerEnterBind = new Bind(interactableTarget.PointArea.PointerEnter);

            pointerEnterBind.OnTriggered += () =>
            {
                if (!_playerPointerInput.IsPressed)
                {
                    return;
                }

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

        private void OnConfirmedPress()
        {
            if (!TryResolveEntityAtScreenPosition(_playerPointerInput.ScreenPosition, out string entityId))
            {
                return;
            }

            _hoveredEntityId.Value = entityId;
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

        private bool TryResolveEntityAtScreenPosition(Vector2 screenPosition, out string entityId)
        {
            entityId = null;

            EventSystem eventSystem = EventSystem.current;

            if (eventSystem == null)
            {
                return false;
            }

            PointerEventData pointerEventData = new PointerEventData(eventSystem) { position = screenPosition };

            _raycastResults.Clear();
            eventSystem.RaycastAll(pointerEventData, _raycastResults);

            foreach (RaycastResult raycastResult in _raycastResults)
            {
                InteractableTarget interactableTarget = raycastResult.gameObject.GetComponentInParent<InteractableTarget>();

                if (interactableTarget == null)
                {
                    continue;
                }

                if (!_interactableTargetsByEntityId.ContainsKey(interactableTarget.EntityId))
                {
                    continue;
                }

                entityId = interactableTarget.EntityId;

                return true;
            }

            return false;
        }
    }
}
