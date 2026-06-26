using UnityEngine;

namespace Core.Gameplay.Movement
{
    [CreateAssetMenu(fileName = "MovementConfig", menuName = "Project/Configs/Movement Config")]
    public sealed class MovementConfig : ScriptableObject
    {
        public float MoveSpeed => _moveSpeed;

        [SerializeField] private float _moveSpeed = 1f;
    }
}
