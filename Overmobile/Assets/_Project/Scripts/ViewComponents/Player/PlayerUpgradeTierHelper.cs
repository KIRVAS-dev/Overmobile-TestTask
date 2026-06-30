using UnityEngine;
using VContainer;
using VContainer.Unity;
using ViewComponents.Animation;
using ViewComponents.Movement;
using ViewComponents.Power;
using ViewComponents.Presentation;
using ViewComponents.Presentation.Player;

namespace ViewComponents.Player
{
    public static class PlayerUpgradeTierHelper
    {
        public static GameObject ResolveTierPrefab(GameObject[] tierPrefabs, int tierIndex, string ownerName)
        {
            if (tierIndex < 0
             || tierIndex >= tierPrefabs.Length)
            {
                throw new UpgradeTierIndexOutOfRangeException(ownerName, tierIndex, tierPrefabs.Length);
            }

            GameObject tierPrefab = tierPrefabs[tierIndex];

            return tierPrefab == null
                ? throw new MissingUpgradeTierPrefabException(ownerName, tierIndex)
                : tierPrefab;
        }

        public static GameObject InstantiateTier(GameObject tierPrefab, Transform parent, Vector3 worldPosition,
            Quaternion worldRotation, IObjectResolver objectResolver)
        {
            GameObject tierInstance = Object.Instantiate(tierPrefab, parent);
            tierInstance.transform.SetPositionAndRotation(worldPosition, worldRotation);
            objectResolver.InjectGameObject(tierInstance);

            return tierInstance;
        }

        public static PlayerUpgradeTierComponents ResolveTierComponents(GameObject tierInstance, string prefabName)
        {
            MovementView movementView = tierInstance.GetComponent<MovementView>();

            if (movementView == null)
            {
                throw new MissingCharacterViewComponentException(prefabName, nameof(MovementView));
            }

            CharacterAnimationView characterAnimationView = tierInstance.GetComponent<CharacterAnimationView>();

            if (characterAnimationView == null)
            {
                throw new MissingCharacterViewComponentException(prefabName, nameof(CharacterAnimationView));
            }

            EntityPowerView entityPowerView = tierInstance.GetComponent<EntityPowerView>();

            if (entityPowerView == null)
            {
                throw new MissingCharacterViewComponentException(prefabName, nameof(EntityPowerView));
            }

            ActiveCharacterAnchorView activeCharacterAnchorView = tierInstance.GetComponent<ActiveCharacterAnchorView>();

            if (activeCharacterAnchorView == null)
            {
                throw new MissingActiveCharacterAnchorViewException(prefabName);
            }

            PresentationSectionMap presentationSectionMap =
                tierInstance.GetComponent<PresentationSectionMap>();

            if (presentationSectionMap == null)
            {
                throw new InvalidPresentationSectionMapException(
                    prefabName,
                    $"{nameof(PresentationSectionMap)} is not assigned"
                );
            }

            return new PlayerUpgradeTierComponents(
                movementView,
                characterAnimationView,
                entityPowerView,
                activeCharacterAnchorView,
                presentationSectionMap
            );
        }
    }
}
