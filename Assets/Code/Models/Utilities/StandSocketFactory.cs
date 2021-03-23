using System;
using System.Collections.Generic;
using PrimoVictoria.DataModels;
using UnityEngine;

namespace PrimoVictoria.Assets.Code.Models.Utilities
{
    public static class StandSocketFactory
    {
        private const int SOLO_MODELCOUNT = 1;
        private const int CONQUEST_INFANTRY_STAND = 4;
        private const double CONQUEST_STAND_DIMENSION = 2.5;
        private const float TOLERANCE = 0.001f;
        
        public static List<StandSocket> CreateStandSocketsForStand(GameObject stand, UnitData unitData)
        {
            if (Math.Abs(stand.transform.localScale.x - CONQUEST_STAND_DIMENSION) < TOLERANCE
                && Math.Abs(stand.transform.localScale.z - CONQUEST_STAND_DIMENSION) < TOLERANCE)
            {
                return CreateConquestStandSockets(stand, unitData);
            }

            return null;
        }

        private static List<StandSocket> CreateConquestStandSockets(GameObject stand, UnitData unitData)
        {
            if (unitData.ModelsPerStand == CONQUEST_INFANTRY_STAND) return CreateConquestInfantrySockets();
            if (unitData.ModelsPerStand == SOLO_MODELCOUNT) return CreateConquestInfantrySoloSocket();

            throw new ArgumentException(
                $"Conquest scale stand does not understand {unitData.ModelsPerStand} model count");
        }

        private static List<StandSocket> CreateConquestInfantrySockets()
        {
            return new List<StandSocket>()
            {
                new StandSocket() {StandPosition = new Vector3(0.3f,0,0.3f)}, 
                new StandSocket() {StandPosition = new Vector3(-0.3f, 0.0f, 0.3f)},
                new StandSocket() {StandPosition = new Vector3(-0.3f, 0.0f, -0.3f)}, 
                new StandSocket() {StandPosition = new Vector3(0.3f, 0.0f, -0.3f)} 
            };
        }

        private static List<StandSocket> CreateConquestInfantrySoloSocket()
        {
            return new List<StandSocket>() {new StandSocket() {StandPosition = new Vector3(1.25f, 1.25f)}};
        }
    }
}
