using UnityEngine;

namespace PrimoVictoria.UI
{
    /// <summary>
    /// Component responsible for rendering projector images to the tabletop
    /// </summary>
    public class PrimoProjector : MonoBehaviour
    {
        public static void DrawFriendlyProjector(Projectors projectors, float projectorSize, Transform parent)
        {
            DrawProjector(projectors, projectorSize, parent, isFriendly: true, ghost: false);
        }

        public static void DrawFriendlyGhostedProjector(Projectors projectors, float projectorSize, Transform parent)
        {
            DrawProjector(projectors, projectorSize, parent, isFriendly: true, ghost: true);
        }

        public static void DrawEnemyProjector(Projectors projectors, float projectorSize, Transform parent)
        {
            DrawProjector(projectors, projectorSize, parent, isFriendly: false, ghost: false);
        }

        public static void DrawEnemyGhostedProjector(Projectors projectors, float projectorSize, Transform parent)
        {
            DrawProjector(projectors, projectorSize, parent, isFriendly: false, ghost: true);
        }

        /// <summary>
        /// Draws selection projector around each model mesh
        /// </summary>
        /// <param name="projectors"></param>
        /// <param name="projectorSize">The orthographic size of the projector</param>
        /// <param name="parent">The parent transform that the projector will be attached to</param>
        /// <param name="isFriendly">If the transform being projected is friendly or not</param>
        private static void DrawProjector(Projectors projectors, float projectorSize, Transform parent, bool isFriendly, bool ghost)
        {
            //draw the projector prefab (the circle under the models) under the models
            var selectionObject = PrimoProjector.GetSelectionProjector(projectors, isFriendly, ghost, projectorSize);
            if (selectionObject == null) return;

            selectionObject.transform.SetParent(parent, worldPositionStays: false); //worldPositionStays = false otherwise who knows where the circle goes
        }

        /// <summary>
        /// Gets the appropriate selection projector
        /// </summary>
        /// <param name="projectors">A collection of projectors</param>
        /// <param name="isFriendly">If the friendly projector should be used</param>
        /// <param name="ghost">If the projectors should have a ghost effect applied to them</param>
        /// <param name="orthoSize">The size of the projector</param>
        /// <returns></returns>
        private static GameObject GetSelectionProjector(Projectors projectors, bool isFriendly, bool ghost, float orthoSize)
        {
            var projectorPrefab = GetActiveProjectorPrefab(projectors, isFriendly: isFriendly);

            if (projectorPrefab == null) return null;

            var selectionObject = Instantiate(projectorPrefab);
            var projector = selectionObject.GetComponentInChildren<UnityEngine.Projector>();
            projector.orthographicSize = orthoSize;

            //set the transparency depending on if we're ghosting or not
            var falloff = ghost ? projectors.GhostSelectionTransparency : projectors.FullSelectionTransparency; //transparency value
            var color = projector.material.color;
            projector.material.color = new Color(color.r, color.g, color.b, falloff);

            return selectionObject;
        }

        private static GameObject GetActiveProjectorPrefab(Projectors projector, bool isFriendly)
        {
            return isFriendly ? projector.FriendlySelection : projector.EnemySelection;
        }
    }
}
