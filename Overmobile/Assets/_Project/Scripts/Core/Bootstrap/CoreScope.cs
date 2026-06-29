using Core.Camera;
using Core.Gameplay.Character;
using Core.Gameplay.Movement;
using Core.Gameplay.Player;
using Core.Input;
using Core.Input.Movement;
using Input;
using UI.TapIndicator;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using ViewComponents.Camera;
using ViewComponents.Interaction;
using ViewComponents.Movement;
using ViewComponents.Player;

namespace Core.Bootstrap
{
    public sealed class CoreScope : LifetimeScope
    {
        [Header("Input")]
        [SerializeField] private PlayerPointerInput _playerPointerInput;

        [Header("Interaction")]
        [SerializeField] private InteractableTargetProvider _interactableTargetProvider;

        [Header("Movement")]
        [SerializeField] private MovementRouteProvider _movementRouteProvider;
        [SerializeField] private MovementConfig _movementConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<CoreEntryPoint>();

            RegisterCamera(builder);
            RegisterPlayer(builder);
            RegisterInput(builder);
            RegisterMovement(builder);
        }

        private void RegisterCamera(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<CameraTransitionView>().As<ICameraTransitionView>();
        }

        private void RegisterPlayer(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<PlayerView>().As<IPlayerUpgradeView>().As<IPlayerSpawnView>();

            builder
               .Register<ActiveCharacterViewRegistry>(Lifetime.Singleton)
               .As<IActiveCharacterViewProvider>()
               .As<IActiveCharacterViewRegistry>();
        }

        private void RegisterInput(IContainerBuilder builder)
        {
            builder.RegisterComponent(_playerPointerInput);
            builder.Register<PlayerPointerInputFilter>(Lifetime.Singleton).As<IPlayerPointerInput>();
            builder.RegisterComponentInHierarchy<TapIndicator>();
        }

        private void RegisterMovement(IContainerBuilder builder)
        {
            builder.RegisterInstance(_movementConfig);
            builder.RegisterInstance(_movementRouteProvider).As<IMovementRouteProvider>();
            builder.RegisterInstance(_interactableTargetProvider).As<IMovementInputTargetProvider>();
            builder.RegisterComponentInHierarchy<MovementRouteWaypointDisplay>().As<IMovementRouteWaypointDisplay>();
            builder.Register<MovementRouteRegistry>(Lifetime.Singleton).As<IMovementRouteRegistry>();
            builder.Register<MovementModel>(Lifetime.Singleton);
            builder.Register<MovementRouteDisplayService>(Lifetime.Singleton);
            builder.Register<MovementService>(Lifetime.Singleton).As<IMovementService>();
            builder.Register<MovementInputHandler>(Lifetime.Singleton);
        }
    }
}
