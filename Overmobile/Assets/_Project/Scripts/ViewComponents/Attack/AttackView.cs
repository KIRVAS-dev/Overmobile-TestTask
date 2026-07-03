using Core.Animation;
using Core.Gameplay.Attack;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using ViewComponents.Animation;

namespace ViewComponents.Attack
{
    [DisallowMultipleComponent]
    public sealed class AttackView
        : MonoBehaviour,
          IAttackView
    {
        [SerializeField] private CharacterAnimationView _characterAnimationView;
        [SerializeField] private GameObject _attackSfx;
        [Range(min: 0f, max: 1f)]
        [SerializeField] private float _phaseEndNormalizedTime = 0.5f;

        public async UniTask PlayAttackAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _characterAnimationView.FireAnimationAsync(
                    CharacterAnimationSlot.Attack,
                    _phaseEndNormalizedTime,
                    cancellationToken
                );

                _attackSfx.SetActive(true);
            }
            finally
            {
                if (this != null)
                {
                    _attackSfx.SetActive(false);
                }
            }
        }
    }
}
