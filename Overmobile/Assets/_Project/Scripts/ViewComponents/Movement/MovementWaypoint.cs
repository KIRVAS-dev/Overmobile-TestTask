using UnityEngine;

namespace ViewComponents.Movement
{
    public sealed class MovementWaypoint : MonoBehaviour
    {
        public bool HasEndpointKey => !string.IsNullOrWhiteSpace(_endpointKey);
        public string EndpointKey => _endpointKey;

        [SerializeField] private string _endpointKey;
        [SerializeField] private SpriteRenderer _sprite;

        public void EnableVisual()
        {
            _sprite.enabled = true;
        }

        public void DisableVisual()
        {
            _sprite.enabled = false;
        }
    }
}
