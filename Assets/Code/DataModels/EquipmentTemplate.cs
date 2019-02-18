using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "New Equipment", order = 2)]
public class EquipmentTemplate: ScriptableObject
{
    public enum EquipmentType
    {
        Weapon,
        Armor,
        Misc
    }

    public string Name;
    public EquipmentType Type;
}

