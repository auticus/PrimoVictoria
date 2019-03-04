using UnityEngine;
using PrimoVictoria.DataModels;

namespace PrimoVictoria.Models
{
    /// <summary>
    /// Unit class that represents the entirety of a unit and who belongs to it
    /// </summary>
    public class Unit : MonoBehaviour
    {
        [SerializeField] UnitData Data;
    }
}
