using Core.Gameplay.Power;
using UnityEngine;

namespace ViewComponents.Interaction
{
    [DisallowMultipleComponent]
    public sealed class DropView : MonoBehaviour
    {
        public Transform DropAnchor => _dropAnchor;

        [SerializeField] private Transform _dropAnchor;

        public void Bind(IPowerRegistry powerRegistry, string entityId)
        {
            if (_dropAnchor == null)
            {
                throw new InvalidDropException(gameObject.name, "Drop anchor is not assigned");
            }
        }
    }
}
