using Core.Gameplay.Character;
using Core.Gameplay.Interaction;
using Core.Gameplay.Inventory;
using Core.Gameplay.Movement;
using Core.Gameplay.Player;
using Core.Gameplay.Power;
using Core.Input;
using Core.Input.Gameplay;
using Input;
using UI.TapIndicator;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using ViewComponents.Camera;
using ViewComponents.Interaction;
using ViewComponents.Movement;
using ViewComponents.Player;
using ViewComponents.Power;
using ViewComponents.Presentation;
using ViewComponents.Presentation.Player;
using ViewComponents.TargetSelection;
using ViewComponents.UI.PowerPanel;

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

        [Header("Power")]
        [SerializeField] private EntityPowerProvider _entityPowerProvider;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<CoreEntryPoint>();

            RegisterCamera(builder);
            RegisterInput(builder);
            RegisterInteraction(builder);
            RegisterInventory(builder);
            RegisterMovement(builder);
            RegisterPlayer(builder);
            RegisterPower(builder);
            RegisterScopeCancellation(builder);
            RegisterTargetSelection(builder);
        }

        private void RegisterCamera(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<CameraTransitionView>().As<ICameraTransitionView>();
            builder.RegisterComponentInHierarchy<CameraShakeView>().As<ICameraShakeView>();
        }

        private void RegisterInput(IContainerBuilder builder)
        {
            builder.RegisterComponent(_playerPointerInput).As<IPlayerPointerInput>().As<IPlayerPointerInputActivation>();
            builder.RegisterComponentInHierarchy<TapIndicator>();
            builder.Register<GameplayInputBlock>(Lifetime.Singleton).As<IGameplayInputBlock>();
            builder.Register<GameplayInputHandler>(Lifetime.Singleton);
        }

        private void RegisterInteraction(IContainerBuilder builder)
        {
            builder
               .RegisterInstance(_interactableTargetProvider)
               .As<IInteractableTargetProvider>()
               .As<IGameplayInputTargetProvider>();

            builder.Register<EntityGuardAccessRegistry>(Lifetime.Singleton).As<IEntityGuardAccessRegistry>();

            builder
               .Register<InteractionPipeline>(Lifetime.Singleton)
               .As<IInteractionPipeline>()
               .As<IInteractionPhaseSource>()
               .As<IInteractionTargetPresentation>();

            builder.Register<InteractionService>(Lifetime.Singleton).As<IInteractionService>();
            builder.Register<DropBinder>(Lifetime.Singleton).As<IDropBinder>();
            builder.Register<InteractionViewBinder>(Lifetime.Singleton).As<IInteractionViewBinder>();
        }

        private void RegisterInventory(IContainerBuilder builder)
        {
            builder.Register<InventoryModel>(Lifetime.Singleton);
            builder.Register<InventoryService>(Lifetime.Singleton).As<IInventory>();
        }

        private void RegisterMovement(IContainerBuilder builder)
        {
            builder.RegisterInstance(_movementConfig);
            builder.RegisterInstance(_movementRouteProvider).As<IMovementRouteProvider>();
            builder.RegisterComponentInHierarchy<MovementRouteWaypointDisplay>().As<IMovementRouteWaypointDisplay>();
            builder.Register<MovementRouteRegistry>(Lifetime.Singleton).As<IMovementRouteRegistry>();
            builder.Register<MovementModel>(Lifetime.Singleton);
            builder.Register<MovementRouteDisplayService>(Lifetime.Singleton);
            builder.Register<MovementService>(Lifetime.Singleton).As<IMovementService>();
        }

        private void RegisterPlayer(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<PlayerView>().As<IPlayerUpgradeView>().As<IPlayerSpawnView>();
            builder.Register<PlayerUpgradeService>(Lifetime.Singleton).As<IPlayerUpgradeService>();

            builder
               .Register<ActiveCharacterViewRegistry>(Lifetime.Singleton)
               .As<IActiveCharacterViewProvider>()
               .As<IActiveCharacterViewRegistry>();

            builder
               .Register<ActiveCharacterPresentationProvider>(Lifetime.Singleton)
               .As<IActiveCharacterPresentationProvider>()
               .AsSelf();

            builder
               .Register<ActivePresentationSectionMapProvider>(Lifetime.Singleton)
               .As<IActivePresentationSectionMapProvider>()
               .AsSelf();
        }

        private void RegisterPower(IContainerBuilder builder)
        {
            builder.RegisterInstance(_entityPowerProvider).As<IEntityPowerProvider>().AsSelf();
            builder.Register<PowerRegistry>(Lifetime.Singleton).As<IPowerRegistry>().AsSelf();
            builder.Register<PowerService>(Lifetime.Singleton).As<IPowerService>();
            builder.Register<PlayerPowerDisplayState>(Lifetime.Singleton).As<IPlayerPowerDisplayState>();
            builder.Register<EntityPowerPanelBinder>(Lifetime.Singleton).As<IEntityPowerPanelBinder>();
            builder.Register<EntityGuardPowerPanelBinder>(Lifetime.Singleton).As<IEntityGuardPowerPanelBinder>();
            builder.Register<EntityPowerViewsBinder>(Lifetime.Singleton).As<IEntityPowerViewsBinder>();
        }

        private void RegisterScopeCancellation(IContainerBuilder builder)
        {
            builder.Register<CoreCancellationSource>(Lifetime.Singleton).As<ICoreScopeCancellation>().AsSelf();
        }

        private void RegisterTargetSelection(IContainerBuilder builder)
        {
            builder.Register<TargetSelectionBinder>(Lifetime.Singleton).As<ITargetSelectionBinder>();
        }
    }
}
