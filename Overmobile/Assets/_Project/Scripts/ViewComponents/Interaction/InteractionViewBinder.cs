using Core;
using Core.Gameplay.Interaction;
using Core.Gameplay.Power;
using VContainer;
using VContainer.Unity;

namespace ViewComponents.Interaction
{
    public sealed class InteractionViewBinder : IInteractionViewBinder
    {
        private readonly IObjectResolver _objectResolver;
        private readonly IPowerRegistry _powerRegistry;
        private readonly IGameplayInputBlock _gameplayInputBlock;
        private readonly IInteractionTargetPresentation _interactionTargetPresentation;
        private readonly InteractableTargetProvider _interactableTargetProvider;

        public InteractionViewBinder(
            IObjectResolver objectResolver,
            IPowerRegistry powerRegistry,
            IGameplayInputBlock gameplayInputBlock,
            IInteractionTargetPresentation interactionTargetPresentation,
            InteractableTargetProvider interactableTargetProvider)
        {
            _objectResolver = objectResolver;
            _powerRegistry = powerRegistry;
            _gameplayInputBlock = gameplayInputBlock;
            _interactionTargetPresentation = interactionTargetPresentation;
            _interactableTargetProvider = interactableTargetProvider;
        }

        public void BindInteractionViews()
        {
            foreach (InteractableTarget interactableTarget in _interactableTargetProvider.InteractableTargets)
            {
                InteractionView interactionView = interactableTarget.GetComponent<InteractionView>();

                if (interactionView == null)
                {
                    continue;
                }

                _interactionTargetPresentation.RegisterTargetPresentation(interactableTarget.EntityId);
                _objectResolver.InjectGameObject(interactableTarget.gameObject);
                interactionView.Bind(
                    _powerRegistry,
                    interactableTarget.EntityId,
                    _gameplayInputBlock,
                    _interactionTargetPresentation
                );
            }
        }
    }
}
