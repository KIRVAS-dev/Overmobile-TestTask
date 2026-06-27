using UnityEngine;

namespace ViewComponents.Movement
{
    public sealed class MovementTarget : MonoBehaviour
    {
        public string EndpointKey => _endpointKey;

        [SerializeField] private string _endpointKey;
    }
}
