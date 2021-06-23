using System;
using PrimoVictoria.Models;
using UnityEngine;

namespace PrimoVictoria.Utilities
{
    /// <summary>
    /// Utility class responsible for calculating the positions of a stand in relation to the other stands in the unit
    /// </summary>
    public class RankAndFilePosition 
    {
        /// <summary>
        /// Which point of the square base this operation refers to
        /// </summary>
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
        public static Vector3 GetPosition(Stand pivotStand, int row, int file, float spacing)
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
            var offsetPosition = GetOffsetPosition(file); //gives us the number of stands to the right or left (negative value) based on the file
            var fileOffset = GetStandFileOffset(pivotStand, offsetPosition, spacing);
            var finalOffset = GetStandRankOffset(pivotStand, fileOffset, row, spacing);  
            
            return finalOffset;
        }

        /// <summary>
        /// Returns a position where the upper point of the base is located
        /// </summary>
        /// <param name="stand"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 GetUpperPoint(Stand stand, StandPoint point)
        {
            var direction = stand.Transform.right;
            if (point == StandPoint.UpperLeft) direction *= -1;

            //this is going off of square bases where x & y are the same
            var offset = (direction + stand.Transform.forward) * (stand.Transform.localScale.x / 2);
            return stand.Transform.position + offset;
        }

        /// <summary>
        /// Calculates the move position of the stand based on the pivot stand's location
        /// </summary>
        /// <param name="mouseClickPosition">The position clicked by the player to move the unit to</param>
        /// <param name="stand"></param>
        /// <param name="rank"></param>
        /// <param name="file"></param>
        /// <returns>A Vector3 that represents the position where the stand should be moving to</returns>
        public static Vector3 CalculateMovePosition(Vector3 mouseClickPosition, Stand stand, int rank, int file, float spacing)
        {
            //the mouse clicked a position on the table.  the stand should move to this position but the FRONT of the stand needs to stop at this
            //that means that the transform position needs to be adjusted so that it stops behind this point 
            var offset = RankAndFilePosition.GetPosition(stand, rank, file, spacing);

            //the value that is had is the transform of the stand itself which is in the middle of the stand.  
            //Offset that value back half the stand's height to get its true position
            var distance = stand.Transform.localScale.x / 2;
            var truePosition = (mouseClickPosition + offset) - (stand.Transform.forward * distance);

            return truePosition;
        }

        private static bool IsPivotStand(int row, int file)
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
        private static Vector3 GetStandFileOffset(Stand stand, int filePosition, float spacing)
        {
            if (filePosition == 0)
            {
                return Vector3.zero;
            }
            var direction = filePosition > 0 ? stand.Transform.right : stand.Transform.right * -1;

            //this could be an issue if x and y are different but here they are SQUARES
            //abs used because filePosition is negative to indicate left, positive to indicate right, but for offset we dont care about pos/neg
            //stand should represent the pivot stand or central stand
            var padding = Math.Abs(filePosition) * spacing;
            var offset = direction * (stand.Transform.localScale.x * Math.Abs(filePosition) + padding); 
            return offset;
        }

        /// <summary>
        /// Given a stand, its rank position, and its already determined fileOffset, calculate its final position based on rank
        /// </summary>
        /// <param name="stand"></param>
        /// <param name="fileOffset">The adjusted vector3 representing the stand's file position</param>
        /// <param name="rankPosition">A one-based index determining what row the stand is in</param>
        /// <returns></returns>
        private static Vector3 GetStandRankOffset(Stand stand, Vector3 fileOffset, int rankPosition, float spacing)
        {
            if (rankPosition == 1) return fileOffset;

            rankPosition -= 1; //this is used for an offset, rank 1 means dont offset (0)
            var direction = stand.Transform.forward * -1;
            var padding = rankPosition * spacing;
            var offset = fileOffset + (direction * (stand.Transform.localScale.x * rankPosition + padding));
            return offset;
        }
    }
}
