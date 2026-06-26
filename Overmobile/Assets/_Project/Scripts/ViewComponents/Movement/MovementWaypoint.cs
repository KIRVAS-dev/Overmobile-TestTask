using UnityEngine;

namespace ViewComponents.Movement
{
    public sealed class MovementWaypoint : MonoBehaviour
    {
        public bool HasEndpointKey => !string.IsNullOrWhiteSpace(_endpointKey);

        public string EndpointKey => _endpointKey;

        [SerializeField] private string _endpointKey;
    }
}
