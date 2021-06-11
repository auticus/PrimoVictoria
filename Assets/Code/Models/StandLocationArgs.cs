using System;
using UnityEngine;

namespace PrimoVictoria.Assets.Code.Models
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
