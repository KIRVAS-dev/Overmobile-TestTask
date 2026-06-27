using Core.Gameplay.Animation;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ViewComponents.Animation
{
    public sealed class CharacterAnimationView
        : MonoBehaviour,
          ICharacterAnimationView
    {
        private const int AnimatorBaseLayer = 0;
        private const float AnimationEndNormalizedTimeThreshold = 0.99f;

        [Serializable]
        private struct TriggerNamesMapItem
        {
            public CharacterAnimationSlot AnimationSlot;
            public string TriggerName;
        }

        public CharacterAnimationSlot CurrentAnimationSlot { get; private set; }

        [Header("Animator parameters")]
        [SerializeField] private string _isMovingBoolName = "IsMoving";

        [SerializeField] private Animator _animator;
        [SerializeField] private TriggerNamesMapItem[] _rawTriggersMap;

        private Dictionary<CharacterAnimationSlot, TriggerNamesMapItem> _triggersMap;
        private bool _hasEnteredActionState;

        private void Awake()
        {
            _triggersMap = BuildTriggersMap();
        }

        public void SetIsMoving(bool value)
        {
            _animator.SetBool(_isMovingBoolName, value);
        }

        public void FireAnimation(CharacterAnimationSlot slot)
        {
            TriggerNamesMapItem mapItem = ResolveMapItem(slot);

            _animator.SetTrigger(mapItem.TriggerName);
            _animator.Update(deltaTime: 0);
            CurrentAnimationSlot = slot;
        }

        public async UniTask FireAnimationAsync(CharacterAnimationSlot slot, CancellationToken cancellationToken)
        {
            TriggerNamesMapItem mapItem = ResolveMapItem(slot);
            int stateHashBeforeTrigger = _animator.GetCurrentAnimatorStateInfo(AnimatorBaseLayer).fullPathHash;
            _hasEnteredActionState = false;

            try
            {
                FireAnimation(slot);

                await UniTask.WaitUntil(
                    () => IsActionAnimationComplete(stateHashBeforeTrigger),
                    PlayerLoopTiming.Update,
                    cancellationToken
                );
            }
            finally
            {
                _animator.ResetTrigger(mapItem.TriggerName);
                CurrentAnimationSlot = CharacterAnimationSlot.None;
            }
        }

        private bool IsActionAnimationComplete(int stateHashBeforeTrigger)
        {
            if (_animator.IsInTransition(AnimatorBaseLayer))
            {
                return false;
            }

            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(AnimatorBaseLayer);

            if (!_hasEnteredActionState)
            {
                if (stateInfo.fullPathHash != stateHashBeforeTrigger)
                {
                    _hasEnteredActionState = true;
                }

                return false;
            }

            if (stateInfo.fullPathHash == stateHashBeforeTrigger)
            {
                return true;
            }

            if (stateInfo.loop)
            {
                return false;
            }

            return stateInfo.normalizedTime >= AnimationEndNormalizedTimeThreshold;
        }

        private TriggerNamesMapItem ResolveMapItem(CharacterAnimationSlot slot)
        {
            return !_triggersMap.TryGetValue(slot, out TriggerNamesMapItem mapItem)
                ? throw new CharacterAnimationSlotNotMappedException(slot, gameObject.name)
                : mapItem;
        }

        private Dictionary<CharacterAnimationSlot, TriggerNamesMapItem> BuildTriggersMap()
        {
            if (_rawTriggersMap == null
             || _rawTriggersMap.Length == 0)
            {
                throw new CharacterAnimationTriggerMapMissingException(gameObject.name);
            }

            Dictionary<CharacterAnimationSlot, TriggerNamesMapItem> triggersMap =
                new Dictionary<CharacterAnimationSlot, TriggerNamesMapItem>(_rawTriggersMap.Length);

            foreach (TriggerNamesMapItem mapItem in _rawTriggersMap)
            {
                if (triggersMap.ContainsKey(mapItem.AnimationSlot))
                {
                    throw new DuplicateCharacterAnimationSlotException(gameObject.name, mapItem.AnimationSlot.ToString());
                }

                if (string.IsNullOrWhiteSpace(mapItem.TriggerName))
                {
                    throw new CharacterAnimationTriggerNameMissingException(gameObject.name, mapItem.AnimationSlot.ToString());
                }

                triggersMap.Add(mapItem.AnimationSlot, mapItem);
            }

            return triggersMap;
        }
    }
}
