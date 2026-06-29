using Core.Animation;
using Input;
using UnityEngine;
using ViewComponents.Animation;

namespace ViewComponents.Interaction
{
    public sealed class InteractableTarget : MonoBehaviour
    {
        public string EntityKey => string.IsNullOrWhiteSpace(_entityKey)
            ? throw new MissingInteractableEntityKeyException(gameObject.name)
            : _entityKey;

        public PointArea PointArea => _pointArea;

        public ICharacterAnimationView AnimationView => _characterAnimationView;

        [SerializeField] private string _entityKey;
        [SerializeField] private PointArea _pointArea;
        [SerializeField] private CharacterAnimationView _characterAnimationView;
    }
}
