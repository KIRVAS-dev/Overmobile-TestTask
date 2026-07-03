using Core.Camera;
using Core.Gameplay.Player;
using Core.Input.Gameplay;
using Cysharp.Threading.Tasks;
using System;
using ViewComponents.Interaction;
using ViewComponents.Power;
using VContainer.Unity;

namespace Core.Bootstrap
{
    public sealed class CoreEntryPoint : IStartable, IDisposable
    {
        private readonly ICameraTransitionView _cameraTransitionView;
        private readonly IPlayerSpawnView _playerSpawnView;
        private readonly IDropBinder _dropBinder;
        private readonly IInteractionViewBinder _interactionViewBinder;
        private readonly IEntityPowerViewsBinder _entityPowerViewsBinder;
        private readonly IGameplayInputBlock _gameplayInputBlock;
        private readonly GameplayInputHandler _gameplayInputHandler;
        private readonly CoreCancellationSource _coreCancellation;

        public CoreEntryPoint(
            ICameraTransitionView cameraTransitionView,
            IPlayerSpawnView playerSpawnView,
            IDropBinder dropBinder,
            IInteractionViewBinder interactionViewBinder,
            IEntityPowerViewsBinder entityPowerViewsBinder,
            IGameplayInputBlock gameplayInputBlock,
            GameplayInputHandler gameplayInputHandler,
            CoreCancellationSource coreCancellation)
        {
            _cameraTransitionView = cameraTransitionView;
            _playerSpawnView = playerSpawnView;
            _dropBinder = dropBinder;
            _interactionViewBinder = interactionViewBinder;
            _entityPowerViewsBinder = entityPowerViewsBinder;
            _gameplayInputBlock = gameplayInputBlock;
            _gameplayInputHandler = gameplayInputHandler;
            _coreCancellation = coreCancellation;
        }

        void IStartable.Start()
        {
            _playerSpawnView.Spawn(0);
            _dropBinder.BindLootDrops();
            _interactionViewBinder.BindInteractionViews();
            _entityPowerViewsBinder.BindEntityPowerViews();

            StartIntroAsync().Forget();
        }

        void IDisposable.Dispose()
        {
            _gameplayInputHandler.StopListening();
        }

        private async UniTaskVoid StartIntroAsync()
        {
            _gameplayInputBlock.Block();
            bool isCancelled = false;

            try
            {
                await _cameraTransitionView.PlayTransitionAsync(_coreCancellation.Token);
            }
            catch (OperationCanceledException)
            {
                isCancelled = true;
            }
            finally
            {
                _gameplayInputBlock.Unblock();
            }

            if (!isCancelled)
            {
                _gameplayInputHandler.StartListening();
            }
        }
    }
}
