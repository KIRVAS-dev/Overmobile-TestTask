using Core.Gameplay.Animation;
using Input;
using UnityEngine;
using ViewComponents.Animation;
using ViewComponents.Movement;

namespace ViewComponents.Interaction
{
    public sealed class InteractableTarget : MonoBehaviour
    {
        public string EndpointKey => _movementTarget.EndpointKey;

        public PointArea PointArea => _pointArea;

        public ICharacterAnimationView AnimationView => _characterAnimationView;

        [SerializeField] private PointArea _pointArea;
        [SerializeField] private MovementTarget _movementTarget;
        [SerializeField] private CharacterAnimationView _characterAnimationView;
    }
}
