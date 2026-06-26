using Core.Gameplay.Movement;
using Core.Input.Movement;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using ViewComponents.Movement;

namespace Core.Bootstrap
{
    public sealed class CoreScope : LifetimeScope
    {
        [SerializeField] private MovementConfig _movementConfig;
        [SerializeField] private MovementRouteProvider _movementRouteProvider;
        [SerializeField] private MovementTargetProvider _movementTargetProvider;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<CoreEntryPoint>();

            RegisterMovement(builder);
        }

        private void RegisterMovement(IContainerBuilder builder)
        {
            builder.RegisterInstance(_movementConfig);
            builder.RegisterInstance(_movementRouteProvider).As<IMovementRouteProvider>();
            builder.RegisterInstance(_movementTargetProvider).As<IMovementInputTargetProvider>();
            builder.RegisterComponentInHierarchy<MovementView>().As<IMovementView>();
            builder.Register<MovementRouteRegistry>(Lifetime.Singleton).As<IMovementRouteRegistry>();
            builder.Register<MovementModel>(Lifetime.Singleton);
            builder.Register<MovementService>(Lifetime.Singleton).As<IMovementService>();
            builder.Register<MovementInputHandler>(Lifetime.Singleton);
        }
    }
}
