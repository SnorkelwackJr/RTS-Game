using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Scriptable Objects/Unit", order = 1)]
public class UnitData : ScriptableObject
{
    public string code;
    public string unitName;
    public string description;
    public int healthpoints;
    public GameObject prefab;
    public List<ResourceValue> cost;

    public bool CanBuy()
    {
        foreach (ResourceValue resource in cost)
            if (Globals.GAME_RESOURCES[resource.code].Amount < resource.amount)
                return false;
        return true;
    }

    public List<ResourceValue> Cost { get { return cost; } }
    public string Code { get { return code; } }
    public string UnitName { get { return unitName; } }
    public string Description { get { return description; } }
    public int Healthpoints { get { return healthpoints; } }
}