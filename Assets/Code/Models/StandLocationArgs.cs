using System;
using UnityEngine;

namespace PrimoVictoria.Assets.Code.Models
{
    public class StandLocationArgs : EventArgs
    {
        public Vector3 StandLocation;
        public Vector3[] StandSocketLocations;

        public StandLocationArgs(Vector3 stand, Vector3[] standSockets)
        {
            StandLocation = stand;
            StandSocketLocations = standSockets;
        }
    }
}
