using PrimoVictoria.Models;
using UnityEngine;

namespace PrimoVictoria.Code.Utilities
{
    /// <summary>
    /// Utility class to determine the actual world position of the stand when it moves
    /// </summary>
    public static class StandMovePosition
    {
        /// <summary>
        /// Calculates the move position of the stand based on the pivot stand's location
        /// </summary>
        /// <param name="mouseClickPosition">The position clicked by the player to move the unit to</param>
        /// <param name="stand"></param>
        /// <param name="rank"></param>
        /// <param name="file"></param>
        /// <returns>A Vector3 that represents the position where the stand should be moving to</returns>
        public static Vector3 GetStandMovePosition(Vector3 mouseClickPosition, Stand stand, int rank, int file)
        {
            //the mouse clicked a position on the table.  the stand should move to this position but the FRONT of the stand needs to stop at this
            //that means that the transform position needs to be adjusted so that it stops behind this point 
            var offset = StandPosition.GetStandUnitOffset(stand, rank, file);

            //the value that is had is the transform of the stand itself which is in the middle of the stand.  
            //Offset that value back half the stand's height to get its true position
            var distance = stand.Transform.localScale.x / 2;
            var truePosition = (mouseClickPosition + offset) - (stand.Transform.forward * distance);

            return truePosition;
        }
    }
}
