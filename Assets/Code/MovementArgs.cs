using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrimoVictoria
{
    /// <summary>
    /// EventArgs derivative that holds information on which direction an entity should move
    /// </summary>
    public class MovementArgs : EventArgs
    {
        public IEnumerable<Vector3> Directions { get; }

        public MovementArgs(IEnumerable<Vector3> directions)
        {
            Directions = directions;
        }
    }
}
