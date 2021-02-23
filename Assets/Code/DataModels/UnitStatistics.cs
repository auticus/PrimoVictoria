using System;

namespace PrimoVictoria.DataModels
{
    [Serializable]
    public class UnitStatistics
    {
        public int Movement;
        public int Volley;
        public int Clash;
        public int Attacks;
        public int Wounds;
        public int Resolve;
        public int Defense;
        public int Evasion;
    }
}