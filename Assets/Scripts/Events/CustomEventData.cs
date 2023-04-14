using UnityEngine;

public class CustomEventData
{
    public UnitData unitData;
    public Unit unit;

    public CustomEventData(UnitData unitData)
    {
        this.unitData = unitData;
        this.unit = null;
    }

    public CustomEventData(Unit unit)
    {
        this.unitData = null;
        this.unit = unit;
    }
}
