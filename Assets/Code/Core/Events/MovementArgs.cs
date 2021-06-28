using System.Collections.Generic;
using PrimoVictoria.Assets.Code.Core.Events;
using UnityEngine;

namespace PrimoVictoria.Core.Events
{
    /// <summary>
    /// EventArgs derivative that holds information on which direction an entity should move
    /// </summary>
    public class MovementArgs : PrimoRecordableEventArgs
    {
        public IEnumerable<Vector3> Directions { get; }

        public MovementArgs(IEnumerable<Vector3> directions) 
        {
            Directions = directions;
        }
    }
}
