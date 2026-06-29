using Core.Animation;

namespace Core.Gameplay.Interaction
{
    public readonly struct InteractableTargetData
    {
        public string EntityKey { get; }
        public ICharacterAnimationView AnimationView { get; }

        public InteractableTargetData(string entityKey, ICharacterAnimationView animationView)
        {
            EntityKey = entityKey;
            AnimationView = animationView;
        }
    }
}
