using System.Collections.Generic;
using PrimoVictoria.Utilities;
using UnityEngine;

namespace PrimoVictoria.UI
{
    /// <summary>
    /// Component responsible for rendering projector images to the tabletop
    /// </summary>
    public class PrimoProjector : MonoBehaviour
    {
        /// <summary>
        /// Draws a single Friendly Projector around the transform sent
        /// </summary>
        /// <param name="projectors"></param>
        /// <param name="projectorSize"></param>
        /// <param name="parent"></param>
        public static void DrawFriendlyProjector(Projectors projectors, float projectorSize, Transform parent)
        {
            DrawProjector(projectors, projectorSize, parent, isFriendly: true, ghost: false);
        }

        /// <summary>
        /// Draws a single Ghosted Friendly Projector around the transform sent
        /// </summary>
        /// <param name="projectors"></param>
        /// <param name="projectorSize"></param>
        /// <param name="parent"></param>
        public static void DrawFriendlyGhostedProjector(Projectors projectors, float projectorSize, Transform parent)
        {
            DrawProjector(projectors, projectorSize, parent, isFriendly: true, ghost: true);
        }

        /// <summary>
        /// Draws a series of Ghosted Friendly Projectors at the world positions passed
        /// </summary>
        /// <param name="projectors"></param>
        /// <param name="projectorSize"></param>
        /// <param name="worldPositions"></param>
        public static void DrawUnboundFriendlyProjectors(Projectors projectors, float projectorSize, IEnumerable<Vector3> worldPositions, Quaternion rotation)
        {
            DrawUnboundProjectors(projectors, projectorSize, worldPositions, rotation, isFriendly: true, ghost: false);
        }

        /// <summary>
        /// Draws a single Enemy projector around the transform sent
        /// </summary>
        /// <param name="projectors"></param>
        /// <param name="projectorSize"></param>
        /// <param name="parent"></param>
        public static void DrawEnemyProjector(Projectors projectors, float projectorSize, Transform parent)
        {
            DrawProjector(projectors, projectorSize, parent, isFriendly: false, ghost: false);
        }

        /// <summary>
        /// Draws a single Enemy ghosted projector around the transform sent
        /// </summary>
        /// <param name="projectors"></param>
        /// <param name="projectorSize"></param>
        /// <param name="parent"></param>
        public static void DrawEnemyGhostedProjector(Projectors projectors, float projectorSize, Transform parent)
        {
            DrawProjector(projectors, projectorSize, parent, isFriendly: false, ghost: true);
        }

        /// <summary>
        /// Removes gameobjects associated with projectors based on the tag passed in
        /// </summary>
        /// <param name="tag"></param>
        public static void RemoveProjectors(string tag)
        {
            //this may not be the best way to go about this but doing research, the performance shouldn't be bad since Unity keeps a list of all actual tagged objects
            //if performance is an issue, try keeping the projectors in an arraylist and just hit that
            //we created a series of projectors around the meshes, now find them and destroy them
            //(alternatively we have them drawn in worldspace somewhere as a unit footprint)
            var projectors = GameObject.FindGameObjectsWithTag(tag);
            foreach (var projector in projectors)
            {
                Destroy(projector);
            }
        }

        /// <summary>
        /// Removes all Unbound Projectors from the scene
        /// </summary>
        public static void RemoveUnboundProjectors()
        {
            var projectors = GameObject.FindGameObjectsWithTag(StaticResources.UNBOUND_DECORATOR_TAG);
            foreach (var projector in projectors)
            {
                Destroy(projector);
            }
        }

        /// <summary>
        /// Draws selection projector around each model mesh
        /// </summary>
        /// <param name="projectors"></param>
        /// <param name="projectorSize">The orthographic size of the projector</param>
        /// <param name="parent">The parent transform that the projector will be attached to</param>
        /// <param name="isFriendly">If the transform being projected is friendly or not</param>
        /// <param name="ghost">If the projectors should be ghosted</param>
        private static void DrawProjector(Projectors projectors, float projectorSize, Transform parent, bool isFriendly, bool ghost)
        {
            //draw the projector prefab (the circle under the models) under the models
            var selectionObject = PrimoProjector.GetSelectionProjector(projectors, isFriendly, ghost, projectorSize);
            if (selectionObject == null) return;

            selectionObject.transform.SetParent(parent, worldPositionStays: false); //worldPositionStays = false otherwise who knows where the circle goes
        }

        /// <summary>
        /// Draws selection projector around a space defined in worldPosition tagged with UNBOUND_DECORATOR_TAG.  Called UNBOUND because not bound to a mesh
        /// </summary>
        /// <param name="projectors"></param>
        /// <param name="projectorSize">The orthographic size of the projector</param>
        /// <param name="worldPositions">The locations where the projector should draw</param>
        /// <param name="isFriendly">If the transform being projected is friendly or not</param>
        /// <param name="ghost">If the projectors should be ghosted</param>
        private static void DrawUnboundProjectors(Projectors projectors, float projectorSize, IEnumerable<Vector3> worldPositions, Quaternion rotation,
            bool isFriendly, bool ghost)
        {
            //draw the projector prefab (the circle under the models) at the locations given
            var selectionObjectBase = PrimoProjector.GetSelectionProjector(projectors, isFriendly, ghost, projectorSize);
            if (selectionObjectBase == null) return;
            selectionObjectBase.tag = StaticResources.UNBOUND_DECORATOR_TAG;

            foreach (var position in worldPositions)
            {
                var projector = Instantiate(selectionObjectBase, position, rotation);
                projector.tag = StaticResources.UNBOUND_DECORATOR_TAG;
            }
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
