using System;
using UnityEngine;

namespace PrimoVictoria.Models
{
    public class StandLocationArgs : EventArgs
    {
        public Vector3 StandLocation;

        public StandLocationArgs(Vector3 stand)
        {
            StandLocation = stand;
        }
    }
}
