using Core.Animation;
using Core.Gameplay.Entity;
using Input;
using UnityEngine;
using ViewComponents.Animation;
using ViewComponents.Entity;
using ViewComponents.Power;

namespace ViewComponents.Interaction
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EntityKey))]
    [RequireComponent(typeof(EntityRole))]
    [RequireComponent(typeof(EntityPower))]
    public sealed class InteractableTarget : MonoBehaviour
    {
        public string EntityKey => GetComponent<EntityKey>().Key;
        public string PowerId => GetComponent<EntityPower>().PowerId;
        public EntityType Type => GetComponent<EntityRole>().Type;
        public PointArea PointArea => _pointArea;
        public ICharacterAnimationView AnimationView => _characterAnimationView;

        [SerializeField] private PointArea _pointArea;
        [SerializeField] private CharacterAnimationView _characterAnimationView;
    }
}
