using UnityEngine;

namespace Core.Gameplay.Movement
{
    [CreateAssetMenu(fileName = "MovementConfig", menuName = "Project/Configs/Movement Config")]
    public sealed class MovementConfig : ScriptableObject
    {
        public float MoveSpeed => _moveSpeed;
        public float FacingRotationDuration => _facingRotationDuration;

        [SerializeField] private float _moveSpeed = 1f;
        [SerializeField] private float _facingRotationDuration = 0.2f;
    }
}
