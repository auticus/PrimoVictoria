using System;
using PrimoVictoria.Models;
using UnityEngine;

namespace PrimoVictoria.Assets.Code.Models.Utilities
{
    /// <summary>
    /// Utility class responsible for calculating the position of a stand in relation to its pivot stand (that being the central front stand of a unit)
    /// </summary>
    public class StandPosition
    {
        /// <summary>
        /// Returns the position of a stand based on its row and file position based on the Pivot (or primary) Stand
        /// </summary>
        /// <param name="row"></param>
        /// <param name="file"></param>
        /// <returns>A vector3 representing the offset location</returns>
        public static Vector3 GetStandUnitOffset(Stand pivotStand, int row, int file)
        {
            if (IsPivotStand(row, file))
            {
                return Vector3.zero;
            }

            if (pivotStand?.Transform == null)
            {
                Debug.LogError($"Stand was sent in to get offset that was not pivot stand but the Transform passed was null which is not allowed!");
                throw new ArgumentException("Stand was sent in to get offset that was not pivot stand but the Transform passed was null which is not allowed", nameof(pivotStand));
            }

            //file 1 = center of the row.  2, 4, 6, 8 etc are stands to the left, and 3,5G,7 etc are stands to the right
            //like this:  4 - 2 - 1 - 3 - 5
            var rawOffset = 0.0f;
            var offsetPosition = GetOffsetPosition(file);

            //todo: this is not done yet - need the rank offset as well - this is just testing
            Debug.Log($"offsetPosition = {offsetPosition} :: file = {file}");
            var offset = GetStandFileOffset(pivotStand.Transform, offsetPosition);
            
            return offset;
        }

        public static bool IsPivotStand(int row, int file)
        {
            return (file == 1 && row == 1);
        }

        /// <summary>
        /// Given a file, determine its offset from the central stand
        /// </summary>
        /// <param name="file"></param>
        /// <returns>an int representing how many stands to the right(positive) or left (negative)</returns>
        private static int GetOffsetPosition(int file)
        {
            if (file % 2 == 0) //even number
            {
                return file / 2; //how many stands off of 1 are you?  ie 2 would be 1 off, 4 would be 2 off, etc
            }
            else //odd number
            {
                return ((file - 1) / 2) * -1; //same as above except we have to bring it down 1.  So 3 would be 1 off, 5 would be 2 off, etc
            }
        }

        /// <summary>
        /// Given a filePosition in the unit, determine its world position from the file (horizontal column)
        /// </summary>
        /// <param name="filePosition"></param>
        /// <returns></returns>
        private static Vector3 GetStandFileOffset(Transform stand, int filePosition)
        {
            var direction = filePosition > 0 ? stand.right : stand.right * -1;
            var offset = direction * stand.transform.localScale.x; //this could be an issue if x and y are different, but in CONQUEST its a square
            return offset;
        }

    }
}
