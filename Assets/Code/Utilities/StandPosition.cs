using System;
using PrimoVictoria.Models;
using UnityEngine;

namespace PrimoVictoria.Code.Utilities
{
    /// <summary>
    /// Utility class responsible for calculating the positions of a stand
    /// </summary>
    public class StandPosition
    {
        public enum StandPoint
        {
            UpperLeft = 0,
            UpperRight
        }

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

            if (pivotStand.Transform == null)
            {
                Debug.LogError($"Stand was sent in to get offset that was not pivot stand but the Transform passed was null which is not allowed!");
                throw new ArgumentException("Stand was sent in to get offset that was not pivot stand but the Transform passed was null which is not allowed", nameof(pivotStand));
            }

            //file 1 = center of the row.  2, 4, 6, 8 etc are stands to the left, and 3,5,7 etc are stands to the right
            //like this:  4 - 2 - 1 - 3 - 5
            var offsetPosition = GetOffsetPosition(file);
            var fileOffset = GetStandFileOffset(pivotStand, offsetPosition);
            var finalOffset = GetStandRankOffset(pivotStand, fileOffset, row);  
            
            //Debug.Log($"GetStandUnitOffset - row={row}::file={file}:: fileOffset={fileOffset}:: finalOffset={finalOffset}");
            return finalOffset;
        }

        public static bool IsPivotStand(int row, int file)
        {
            return (file == 1 && row == 1);
        }

        public static Vector3 GetUpperPoint(Stand stand, StandPoint point)
        {
            var direction = stand.Transform.right;
            if (point == StandPoint.UpperLeft) direction *= -1;

            //this is going off of squares where x & y are the same
            var offset = (direction + stand.Transform.forward) * (stand.Transform.localScale.x / 2);
            return stand.Transform.position + offset;
        }

        /// <summary>
        /// Given a file, determine its offset from the central stand
        /// </summary>
        /// <param name="file"></param>
        /// <returns>an int representing how many stands to the right(positive) or left (negative)</returns>
        private static int GetOffsetPosition(int file)
        {
            if (file % 2 == 0) //even numbers (go to the left so should also be negative)
            {
                return (file / 2) * -1; //how many stands off of 1 are you?  ie 2 would be 1 off, 4 would be 2 off, etc
            }
            else //odd number (go to the right so should be positive)
            {
                return (file - 1) / 2; //same as above except we have to bring it down 1.  So 3 would be 1 off, 5 would be 2 off, etc
            }
        }

        /// <summary>
        /// Given a filePosition in the unit, determine its world position from the file (horizontal column)
        /// </summary>
        /// <param name="filePosition">One based index where 1 is the center pivot stand, negative numbers are left, positive to the right</param>
        /// <returns></returns>
        private static Vector3 GetStandFileOffset(Stand stand, int filePosition)
        {
            if (filePosition == 0)
            {
                return Vector3.zero;
            }
            var direction = filePosition > 0 ? stand.Transform.right : stand.Transform.right * -1;

            //this could be an issue if x and y are different, but in CONQUEST its a square
            //abs used because filePosition is negative to indicate left, positive to indicate right, but for offset we dont care about pos/neg
            var offset = direction * (stand.Transform.localScale.x * Math.Abs(filePosition)); 
            return offset;
        }

        /// <summary>
        /// Given a stand, its rank position, and its already determined fileOffset, calculate its final position based on rank
        /// </summary>
        /// <param name="stand"></param>
        /// <param name="fileOffset">The adjusted vector3 representing the stand's file position</param>
        /// <param name="rankPosition">A one-based index determining what row the stand is in</param>
        /// <returns></returns>
        private static Vector3 GetStandRankOffset(Stand stand, Vector3 fileOffset, int rankPosition)
        {
            if (rankPosition == 1) return fileOffset;

            rankPosition -= 1; //this is used for an offset, rank 1 means dont offset (0)
            var direction = stand.Transform.forward * -1;
            var offset = fileOffset + (direction * (stand.Transform.localScale.x * rankPosition));
            return offset;
        }

    }
}
