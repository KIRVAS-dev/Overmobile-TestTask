using Core.Animation;
using Core.Gameplay.Character;
using Core.Gameplay.Movement;
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
        private IActiveCharacterViewProvider _activeCharacterViewProvider;

        [Inject]
        public void Construct(
            IActiveCharacterViewRegistry activeCharacterViewRegistry,
            IActiveCharacterViewProvider activeCharacterViewProvider)
        {
            _activeCharacterViewRegistry = activeCharacterViewRegistry;
            _activeCharacterViewProvider = activeCharacterViewProvider;
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
            GameObject tierPrefab = _upgradeTierPrefabs[tierIndex];
            Vector3 worldPosition = _upgradeTierRoot.position;
            Quaternion worldRotation = _upgradeTierRoot.rotation;

            if (replaceCurrent)
            {
                MovementView currentMovementView =
                    (MovementView)_activeCharacterViewProvider.ActiveCharacterView.MovementView;
                Transform currentTierTransform = currentMovementView.transform;
                worldPosition = currentTierTransform.position;
                worldRotation = currentTierTransform.rotation;

                currentTierTransform.DOKill();
                Destroy(currentTierTransform.gameObject);
            }

            GameObject tierInstance = Instantiate(tierPrefab, _upgradeTierRoot);
            tierInstance.transform.SetPositionAndRotation(worldPosition, worldRotation);

            MovementView movementView = tierInstance.GetComponent<MovementView>();
            CharacterAnimationView characterAnimationView = tierInstance.GetComponent<CharacterAnimationView>();

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
