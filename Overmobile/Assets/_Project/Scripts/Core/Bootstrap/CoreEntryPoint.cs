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
        private readonly IEntityPowerPanelBinder _entityPowerPanelBinder;
        private readonly IGameplayInputBlock _gameplayInputBlock;
        private readonly GameplayInputHandler _gameplayInputHandler;
        private readonly EntityPowerProvider _entityPowerProvider;
        private readonly CoreCancellationSource _coreCancellation;

        public CoreEntryPoint(
            ICameraTransitionView cameraTransitionView,
            IPlayerSpawnView playerSpawnView,
            IDropBinder dropBinder,
            IEntityPowerPanelBinder entityPowerPanelBinder,
            IGameplayInputBlock gameplayInputBlock,
            GameplayInputHandler gameplayInputHandler,
            EntityPowerProvider entityPowerProvider,

            CoreCancellationSource coreCancellation)
        {
            _cameraTransitionView = cameraTransitionView;
            _playerSpawnView = playerSpawnView;
            _dropBinder = dropBinder;
            _entityPowerPanelBinder = entityPowerPanelBinder;
            _gameplayInputBlock = gameplayInputBlock;
            _gameplayInputHandler = gameplayInputHandler;
            _entityPowerProvider = entityPowerProvider;
            _coreCancellation = coreCancellation;
        }

        void IStartable.Start()
        {
            _playerSpawnView.Spawn(0);
            _dropBinder.BindLootDrops();

            foreach (EntityPowerView entityPowerView in _entityPowerProvider.GetInteractableEntityPowerViews())
            {
                _entityPowerPanelBinder.BindPowerPanel(entityPowerView);
            }

            StartIntroAsync().Forget();
        }

        void IDisposable.Dispose()
        {
            _gameplayInputHandler.StopListening();
        }

        private async UniTaskVoid StartIntroAsync()
        {
            _gameplayInputBlock.Block();

            try
            {
                await _cameraTransitionView.PlayTransitionAsync(_coreCancellation.Token);
            }
            finally
            {
                _gameplayInputBlock.Unblock();
            }

            _gameplayInputHandler.StartListening();
        }
    }
}
