using Core.Gameplay.Power;
using System;
using UnityEngine;

namespace ViewComponents.Interaction
{
    [DisallowMultipleComponent]
    public sealed class DropView : MonoBehaviour
    {
        private IDisposable _binding;

        public Transform DropAnchor => _dropAnchor;

        [SerializeField] private Transform _dropAnchor;
        [SerializeField] private GameObject _hideOnLoot;

        public void Bind(IPowerRegistry powerRegistry, string entityId)
        {
            _binding?.Dispose();
            _binding = new DropBinding(powerRegistry, entityId, this);
        }

        internal void HideLootSource()
        {
            if (_hideOnLoot != null)
            {
                _hideOnLoot.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            _binding?.Dispose();
        }
    }
}
