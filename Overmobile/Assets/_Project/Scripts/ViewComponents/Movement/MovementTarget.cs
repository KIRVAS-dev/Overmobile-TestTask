using Input;
using UnityEngine;

namespace ViewComponents.Movement
{
    public sealed class MovementTarget : MonoBehaviour
    {
        public string EndpointKey => _endpointKey;

        public PointArea PointArea => _pointArea;

        [SerializeField] private string _endpointKey;
        [SerializeField] private PointArea _pointArea;
    }
}
