using PrimoVictoria.Controllers;
using PrimoVictoria.Models;
using UnityEngine;

namespace PrimoVictoria.Assets.Code.Models
{
    /// <summary>
    /// Represents a figurine
    /// </summary>
    public class Miniature
    {
        /// <summary>
        /// The graphic representation of the miniature
        /// </summary>
        public GameObject MiniatureMesh { get; set; }

        public UnitMeshController Controller { get; set; }

        /// <summary>
        /// Reference to the stand that the miniature is a part of
        /// </summary>
        public Stand MiniatureStand { get; set; }

        /// <summary>
        /// What socket the miniature belongs to on a stand
        /// </summary>
        public StandSocket Socket { get; set; }

        public Miniature(GameObject mesh, UnitMeshController controller, Stand stand)
        {
            MiniatureMesh = mesh;
            Controller = controller;
            MiniatureStand = stand;
        }
    }
}
