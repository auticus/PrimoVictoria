using UnityEngine;

namespace PrimoVictoria.Models
{
    /// <summary>
    /// Structure that holds what type of formation a unit is moving in
    /// </summary>
    public class UnitFormation
    {
        /// <summary>
        /// Defines the types of possible movement formations.
        /// </summary>
        public enum FormationType { RankAndFile, Loose };

        [SerializeField, Tooltip("Type of the movement formation.")]
        public FormationType type;

        [SerializeField, Tooltip("Amount of formation positions per iteration."), Min(1)]
        public int FullRank; //rank and file - how many models make up a rank
        
        [SerializeField, Tooltip("Space between units in the formation will be increased by this value."), Min(0.0f)]
        public float Spacing;
    }
}
