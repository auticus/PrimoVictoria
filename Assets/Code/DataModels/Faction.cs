using UnityEngine;

namespace PrimoVictoria.DataModels
{
    [CreateAssetMenu(fileName = "New Faction", menuName = "New Faction", order = 3)]
    public class Faction : ScriptableObject
    {
        public string Name;
    }
}
