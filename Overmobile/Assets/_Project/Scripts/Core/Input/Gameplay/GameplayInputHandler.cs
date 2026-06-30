using Core.Gameplay.Interaction;
using Core.Gameplay.Movement;
using Cysharp.Threading.Tasks;
using Input;
using Input.Binds;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Core.Input.Gameplay
{
    public sealed class GameplayInputHandler
    {
        private readonly ICoreScopeCancellation _coreScopeCancellation;
        private readonly IPlayerPointerInput _pointerInput;
        private readonly IGameplayInputBlock _gameplayInputBlock;
        private readonly IGameplayInputTargetProvider _gameplayInputTargetProvider;
        private readonly IInteractionService _interactionService;
        private readonly MovementRouteDisplayService _routeDisplayService;
        private readonly List<Bind> _binds = new List<Bind>();

        private GameplayInputTarget? _pendingTarget;
        private GameplayInputTarget? _pointerDownTarget;
        private CancellationTokenSource _interactionCancellation;
        private bool _isPointerPressed;
        private bool _isPointerOverTarget;

        public GameplayInputHandler(
            ICoreScopeCancellation coreScopeCancellation,
            IPlayerPointerInput pointerInput,
            IGameplayInputBlock gameplayInputBlock,
            IGameplayInputTargetProvider gameplayInputTargetProvider,
            IInteractionService interactionService,
            MovementRouteDisplayService routeDisplayService)
        {
            _coreScopeCancellation = coreScopeCancellation;
            _pointerInput = pointerInput;
            _gameplayInputBlock = gameplayInputBlock;
            _gameplayInputTargetProvider = gameplayInputTargetProvider;
            _interactionService = interactionService;
            _routeDisplayService = routeDisplayService;
        }

        public void StartListening()
        {
            if (_interactionCancellation != null)
            {
                throw new GameplayInputHandlerAlreadyListeningException();
            }

            _interactionCancellation = CancellationTokenSource.CreateLinkedTokenSource(_coreScopeCancellation.Token);

            _pointerInput.Pressed += OnGlobalPointerPressed;
            _pointerInput.Released += OnPointerUp;

            IReadOnlyList<GameplayInputTarget> inputTargets = _gameplayInputTargetProvider.GetGameplayInputTargets();

            foreach (GameplayInputTarget inputTarget in inputTargets)
            {
                GameplayInputTarget capturedTarget = inputTarget;

                AddBind(capturedTarget.Movement.PointerDown, () => OnPointerDown(capturedTarget));
                AddBind(capturedTarget.Movement.PointerEnter, () => OnPointerEnter(capturedTarget));
                AddBind(capturedTarget.Movement.PointerExit, () => OnPointerExit(capturedTarget));
            }
        }

        public void StopListening()
        {
            _pointerInput.Pressed -= OnGlobalPointerPressed;
            _pointerInput.Released -= OnPointerUp;

            foreach (Bind bind in _binds)
            {
                bind.Disable();
            }

            _binds.Clear();

            _interactionCancellation?.Cancel();
            _interactionCancellation?.Dispose();
            _interactionCancellation = null;

            ResetPointerState();
        }

        private void AddBind(ITrigger trigger, Action handler)
        {
            Bind bind = new Bind(trigger);

            bind.OnTriggered += () =>
            {
                if (_gameplayInputBlock.IsBlocked)
                {
                    return;
                }

                handler();
            };

            bind.Enable();
            _binds.Add(bind);
        }

        private void OnGlobalPointerPressed()
        {
            if (_gameplayInputBlock.IsBlocked)
            {
                return;
            }

            _isPointerPressed = true;
        }

        private void OnPointerDown(GameplayInputTarget inputTarget)
        {
            if (!_interactionService.CanInteract(inputTarget.EntityId))
            {
                return;
            }

            _pointerDownTarget = inputTarget;
            _isPointerOverTarget = true;
            _pendingTarget = inputTarget;
            _routeDisplayService.PreviewRouteTo(inputTarget.Movement.EndpointKey);
        }

        private void OnPointerEnter(GameplayInputTarget inputTarget)
        {
            if (!_isPointerPressed)
            {
                return;
            }

            if (!_interactionService.CanInteract(inputTarget.EntityId))
            {
                return;
            }

            _isPointerOverTarget = true;
            _pendingTarget = inputTarget;
            _routeDisplayService.PreviewRouteTo(inputTarget.Movement.EndpointKey);
        }

        private void OnPointerExit(GameplayInputTarget inputTarget)
        {
            if (_isPointerPressed)
            {
                _isPointerOverTarget = false;
                _routeDisplayService.ClearPreview();
                return;
            }

            if (_pendingTarget.HasValue
             && _pendingTarget.Value.EntityId == inputTarget.EntityId)
            {
                ClearPendingPreview();
            }
        }

        private void OnPointerUp()
        {
            _isPointerPressed = false;

            if (_gameplayInputBlock.IsBlocked)
            {
                ClearPendingPreview();
                _pointerDownTarget = null;
                return;
            }

            bool canInteract = _pendingTarget.HasValue
             && (_isPointerOverTarget
                 || _pointerDownTarget.HasValue && _pointerDownTarget.Value.EntityId == _pendingTarget.Value.EntityId);

            if (!canInteract)
            {
                ClearPendingPreview();
                _pointerDownTarget = null;
                return;
            }

            GameplayInputTarget pendingTarget = _pendingTarget.Value;

            if (!_interactionService.CanInteract(pendingTarget.EntityId))
            {
                ClearPendingPreview();
                _pointerDownTarget = null;
                return;
            }

            _pendingTarget = null;
            _pointerDownTarget = null;

            InteractWithTargetAsync(pendingTarget).Forget();
        }

        private void ClearPendingPreview()
        {
            _pendingTarget = null;
            _routeDisplayService.ClearPreview();
        }

        private void ResetPointerState()
        {
            _isPointerPressed = false;
            _isPointerOverTarget = false;
            _pendingTarget = null;
            _pointerDownTarget = null;
        }

        private async UniTaskVoid InteractWithTargetAsync(GameplayInputTarget inputTarget)
        {
            try
            {
                await _interactionService.InteractAsync(
                    inputTarget.Movement.EndpointKey,
                    inputTarget.EntityId,
                    inputTarget.Movement.FacingWorldPosition,
                    _interactionCancellation.Token
                );
            }
            catch (MovementInProgressException) { }
            catch (OperationCanceledException) { }
        }
    }
}
