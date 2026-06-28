using Core.Animation;
using Core.Gameplay.Character;
using Core.Gameplay.Player;
using DG.Tweening;
using UnityEngine;
using VContainer;
using ViewComponents.Animation;
using ViewComponents.Movement;

namespace ViewComponents.Player
{
    public sealed class PlayerView
        : MonoBehaviour,
          IPlayerUpgradeView,
          IPlayerSpawnView
    {
        public int CurrentTierIndex { get; private set; }
        public bool CanUpgrade => CurrentTierIndex < _upgradeTierPrefabs.Length - 1;

        [SerializeField] private Transform _upgradeTierRoot;
        [SerializeField] private GameObject[] _upgradeTierPrefabs;

        private IActiveCharacterViewRegistry _activeCharacterViewRegistry;
        private Transform _currentTierTransform;

        [Inject]
        public void Construct(IActiveCharacterViewRegistry activeCharacterViewRegistry)
        {
            _activeCharacterViewRegistry = activeCharacterViewRegistry;
        }

        public void Spawn(int tierIndex)
        {
            SpawnUpgradeTierAtIndex(tierIndex, replaceCurrent: false);
        }

        public void Upgrade()
        {
            if (!CanUpgrade)
            {
                return;
            }

            SpawnUpgradeTierAtIndex(CurrentTierIndex + 1, replaceCurrent: true);
        }

        private void SpawnUpgradeTierAtIndex(int tierIndex, bool replaceCurrent)
        {
            if (tierIndex < 0
             || tierIndex >= _upgradeTierPrefabs.Length)
            {
                throw new UpgradeTierIndexOutOfRangeException(gameObject.name, tierIndex, _upgradeTierPrefabs.Length);
            }

            GameObject tierPrefab = _upgradeTierPrefabs[tierIndex];

            if (tierPrefab == null)
            {
                throw new MissingUpgradeTierPrefabException(gameObject.name, tierIndex);
            }

            Vector3 worldPosition = _upgradeTierRoot.position;
            Quaternion worldRotation = _upgradeTierRoot.rotation;

            if (replaceCurrent)
            {
                worldPosition = _currentTierTransform.position;
                worldRotation = _currentTierTransform.rotation;

                _currentTierTransform.DOKill();
                Destroy(_currentTierTransform.gameObject);
            }

            GameObject tierInstance = Instantiate(tierPrefab, _upgradeTierRoot);
            tierInstance.transform.SetPositionAndRotation(worldPosition, worldRotation);

            MovementView movementView = tierInstance.GetComponent<MovementView>();

            if (movementView == null)
            {
                throw new MissingCharacterViewComponentException(tierPrefab.name, nameof(MovementView));
            }

            CharacterAnimationView characterAnimationView = tierInstance.GetComponent<CharacterAnimationView>();

            if (characterAnimationView == null)
            {
                throw new MissingCharacterViewComponentException(tierPrefab.name, nameof(CharacterAnimationView));
            }

            _currentTierTransform = tierInstance.transform;
            _activeCharacterViewRegistry.SetActiveCharacterView(
                new CharacterViewBinding(characterAnimationView, movementView)
            );
            CurrentTierIndex = tierIndex;

            if (replaceCurrent)
            {
                characterAnimationView.FireAnimation(CharacterAnimationSlot.Upgrade);
            }
        }
    }
}
