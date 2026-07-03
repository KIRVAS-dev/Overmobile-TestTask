using Core.Gameplay.Character;
using Core.Gameplay.Player;
using DG.Tweening;
using UnityEngine;
using VContainer;
using ViewComponents.Power;
using ViewComponents.Presentation;

namespace ViewComponents.Player
{
    [DisallowMultipleComponent]
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
        private IEntityPowerPanelBinder _entityPowerPanelBinder;
        private IActiveCharacterPresentationProvider _activeCharacterPresentationProvider;
        private IActivePresentationSectionMapProvider _activePresentationSectionMapProvider;
        private IObjectResolver _objectResolver;
        private Transform _currentTierTransform;

        [Inject]
        public void Construct(
            IActiveCharacterViewRegistry activeCharacterViewRegistry,
            IEntityPowerPanelBinder entityPowerPanelBinder,
            IActiveCharacterPresentationProvider activeCharacterPresentationProvider,
            IActivePresentationSectionMapProvider activePresentationSectionMapProvider,
            IObjectResolver objectResolver)
        {
            _activeCharacterViewRegistry = activeCharacterViewRegistry;
            _entityPowerPanelBinder = entityPowerPanelBinder;
            _activeCharacterPresentationProvider = activeCharacterPresentationProvider;
            _activePresentationSectionMapProvider = activePresentationSectionMapProvider;
            _objectResolver = objectResolver;
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
            _activePresentationSectionMapProvider.PlaySection(PresentationSectionKey.Upgrade);
        }

        private void SpawnUpgradeTierAtIndex(int tierIndex, bool replaceCurrent)
        {
            GameObject tierPrefab = PlayerUpgradeTierHelper.ResolveTierPrefab(_upgradeTierPrefabs, tierIndex, gameObject.name);

            (Vector3 worldPosition, Quaternion worldRotation) = ResolveTierSpawnPose(replaceCurrent);

            DestroyCurrentTierIfReplacing(replaceCurrent);

            GameObject tierInstance = PlayerUpgradeTierHelper.InstantiateTier(
                tierPrefab,
                _upgradeTierRoot,
                worldPosition,
                worldRotation,
                _objectResolver
            );

            PlayerUpgradeTierComponents tierComponents =
                PlayerUpgradeTierHelper.ResolveTierComponents(tierInstance, tierPrefab.name);

            ApplyTierRegistration(tierIndex, tierInstance.transform, tierComponents);
        }

        private (Vector3 worldPosition, Quaternion worldRotation) ResolveTierSpawnPose(bool replaceCurrent)
        {
            if (!replaceCurrent
             || _currentTierTransform == null)
            {
                return (_upgradeTierRoot.position, _upgradeTierRoot.rotation);
            }

            return (_currentTierTransform.position, _currentTierTransform.rotation);
        }

        private void DestroyCurrentTierIfReplacing(bool replaceCurrent)
        {
            if (!replaceCurrent)
            {
                return;
            }

            _currentTierTransform.DOKill();
            Destroy(_currentTierTransform.gameObject);
        }

        private void ApplyTierRegistration(
            int tierIndex,
            Transform tierTransform,
            PlayerUpgradeTierComponents tierComponents)
        {
            _currentTierTransform = tierTransform;

            _activeCharacterViewRegistry.SetActiveCharacterView(
                new ActiveCharacterViewBinding(
                    tierComponents.CharacterAnimationView,
                    tierComponents.AttackView,
                    tierComponents.MovementView
                )
            );

            CurrentTierIndex = tierIndex;
            _entityPowerPanelBinder.BindPowerPanel(tierComponents.EntityPowerView);
            _activeCharacterPresentationProvider.Register(tierComponents.ActiveCharacterAnchorView);
            _activePresentationSectionMapProvider.Register(tierComponents.PresentationSectionMap);
        }
    }
}
